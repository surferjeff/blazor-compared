

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Linq;
using System.IO;

record struct FSEvent(DateTime timestamp, FileSystemEventArgs args);

public class HotTypeScript(ILogger<HotTypeScript> logger) : IHostedService
{
    object _lock = new Object(); // Protects all members.
    FileSystemWatcher? watcher;
    string esbuildPath = "";
    string myDir = "";
    // For debouncing events.
    List<FSEvent> events = [];
    System.Timers.Timer? timer = null;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var myPath = GetThisFilePath();
        if (null == myPath) {
            throw new HotTypeScriptError("Failed to find path to HotTypeScript.cs");
        }
        var myDir = Path.GetDirectoryName(myPath);
        var esbuildPath = Path.Join(myDir, "scripts", "node_modules", ".bin",
            "esbuild");
        if (!Path.Exists(esbuildPath)) {
            throw new HotTypeScriptError($"esbuild not found at {esbuildPath}\n" +
                "Run 'npm ci --no-audit --ignore-scripts' in the scripts " +
                "directory to install esbuild."
            );
        }
        lock(_lock) {
            this.esbuildPath = esbuildPath;
            this.myDir = myDir ?? "";
            StartWatchingLocked(Path.Join(myDir, "scripts"));
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        lock(_lock) {
            ClearTimerLocked();
            if (null != watcher) {
                watcher.Dispose();
                watcher = null;
            }
        }
        return Task.CompletedTask;
    }

    private static string? GetThisFilePath([CallerFilePath] string? path = null)
    {
        return path;
    }

    void StartWatchingLocked(string watchPath)
    {
        Debug.Assert(null == watcher);
        watcher = new FileSystemWatcher();
        watcher.Path = watchPath;

        watcher.NotifyFilter = NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.FileName
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.Size
                                | NotifyFilters.CreationTime
                                | NotifyFilters.Attributes;

        watcher.IncludeSubdirectories = true;

        watcher.Filter = "*.ts";

        // Add event handlers.
        watcher.Changed += OnChanged;
        watcher.Created += OnChanged;
        watcher.Deleted += OnChanged;
        watcher.Renamed += OnChanged;
        watcher.Error += OnError;

        // Begin watching.
        watcher.EnableRaisingEvents = true;
    }

    // Define the event handlers.
    private void OnChanged(object source, FileSystemEventArgs e)
    {
        // Ignore all changes in node_modules.
        var path = e.FullPath;
        while (string.IsNullOrEmpty(path)) {
            var name = Path.GetFileName(path);
            if ("node_modules" == name?.ToLowerInvariant()) {
                return;
            }
            path = Path.GetDirectoryName(path);
        }

        lock (events) {
            // Debounce.
            ClearTimerLocked();
            events.Add(new FSEvent(DateTime.Now, e));
            timer = new System.Timers.Timer(500);
            timer.Elapsed += OnTimer;
            timer.Start();
        }
    }

    private void ClearTimerLocked() {
        if (null != timer) {
            timer.Dispose();
            timer = null;
        }
    }

    private void OnTimer(object? sender, ElapsedEventArgs args)
    {
        // Copy everything we need that's protected by the lock
        // and release the lock quickly to avoid contention.
        string esbuildPath = "";
        string watchPath = "";
        string myDir = "";
        List<FSEvent> events = [];
        lock (_lock) {
            ClearTimerLocked();
            if (null == watcher) {
                return;
            }
            events = this.events;
            esbuildPath = this.esbuildPath;
            this.events = [];
            watchPath = watcher.Path;
            myDir = this.myDir;
        }

        string JsPathFrom(string tsPath) {
            var relPath = Path.GetRelativePath(watchPath, tsPath);
            var relJsPath = relPath.Substring(0, relPath.Length - 3) + ".js";
            return Path.Join(myDir, "wwwroot", "ts", relJsPath);
        }

        // Collect files that need to be compiled and delete files.
        var changed = new HashSet<string>();
        var toBeCompiled = new List<string>();
        foreach (var e in events.OrderBy(e => e.timestamp).Reverse()) {
            if (changed.Contains(e.args.FullPath)) {
                continue;
            }
            changed.Add(e.args.FullPath);
            switch (e.args.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    toBeCompiled.Add(e.args.FullPath);
                    break;
                case WatcherChangeTypes.Created:
                    toBeCompiled.Add(e.args.FullPath);
                    break;
                case WatcherChangeTypes.Deleted:
                    Delete(JsPathFrom(e.args.FullPath));
                    break;
                case WatcherChangeTypes.Renamed:
                    RenamedEventArgs re = (RenamedEventArgs)e.args;
                    Delete(JsPathFrom(re.OldFullPath));
                    changed.Add(re.OldFullPath);
                    toBeCompiled.Add(e.args.FullPath);
                    break;
            }            
        }

        // Invoke esbuild to compile.
        var esbuildArgs = toBeCompiled.Select(
            tsPath => Path.GetRelativePath(myDir, tsPath)).ToList();
        esbuildArgs.Add("--outdir");
        esbuildArgs.Add(Path.Join(myDir, "wwwroot", "ts"));
        using Process process = new Process();
        process.StartInfo = new ProcessStartInfo(esbuildPath, esbuildArgs)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = myDir
        };
        process.Start();
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            logger.LogError("Error compiling {}",
                string.Join(" ", esbuildArgs));
        }
    }

    private void Delete(string tsPath) {
        try {
            File.Delete(tsPath);
        } catch (Exception e) {
            logger.LogError("Failed to delete {}\n{}", tsPath, e.ToString());
        }

    }

    private void OnError(object source, ErrorEventArgs e)
    {
        logger.LogError("While watching {}\n{}", watcher?.Path, e.ToString());
    }
}

public class HotTypeScriptError : Exception {
    public HotTypeScriptError(string message) : base(message) {}
    public HotTypeScriptError(string message, Exception inner) : base(message, inner) {}
    
}