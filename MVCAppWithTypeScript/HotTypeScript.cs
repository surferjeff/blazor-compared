using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

record struct FSEvent(DateTime timestamp, FileSystemEventArgs args);

// Watches typescript files and compiles them when they change.
public class HotTypeScript(ILogger<HotTypeScript> logger) : IHostedService
{
    // Keep esbuild's stdin open.  The process exits when it's closed.
    private StreamWriter? stdin;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var myPath = GetThisFilePath();
        if (null == myPath)
        {
            throw new HotTypeScriptError("Failed to find path to HotTypeScript.cs");
        }
        var myDir = Path.GetDirectoryName(myPath);
        var esbuildBin = "esbuild";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            esbuildBin = "esbuild.cmd";
        }
        var esbuildPath = Path.Join(myDir, "scripts", "node_modules", ".bin",
            esbuildBin);
        if (!Path.Exists(esbuildPath))
        {
            logger.LogError($"Hot TypeScript reloading is DISABLED because esbuild not found at {esbuildPath}\n" +
                "Run 'npm ci --no-audit --ignore-scripts' in the scripts " +
                "directory to install esbuild."
            );
            return Task.CompletedTask;
        }
        var watchPath = Path.Join(myDir, "scripts", "**", "*.ts");
        List<string> esbuildArgs = [watchPath, "--watch",
            $"--outdir={Path.Join(myDir, "wwwroot", "ts")}"];
        using Process process = new Process();
        process.StartInfo = new ProcessStartInfo(esbuildPath, esbuildArgs)
        {
            UseShellExecute = false,
            RedirectStandardInput = true,
            WorkingDirectory = myDir,
        };
        process.Start();
        stdin = process.StandardInput;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (null != stdin)
        {
            stdin.Close();
            stdin = null;
        }
        return Task.CompletedTask;
    }

    private static string? GetThisFilePath([CallerFilePath] string? path = null)
    {
        return path;
    }
}

public class HotTypeScriptError : Exception {
    public HotTypeScriptError(string message) : base(message) {}
    public HotTypeScriptError(string message, Exception inner) : base(message, inner) {}
    
}