// Stuff needed to serve /ts from the Vite proxy.
//
// DotNet builds and serves typescript just fine, but it doesn't do hot
// reloading.  That makes editing TypeScript quite painful.
//
// This code runs the vite proxy for TypeScript files so that hot reloading works.

using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Runtime.CompilerServices;
using System.Net.Sockets;

/// <summary>
/// Doesn't serve /ts/** from the file system because we want to proxy those
/// requests to vite.
/// </summary>
public class KnockoutTs : IFileProvider
{
    IFileProvider inner;

    public KnockoutTs(IFileProvider inner)
    {
        this.inner = inner;
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        var contents = inner.GetDirectoryContents(subpath);
        if (!contents.Exists)
        {
            return contents;
        }

        // Filter out the excluded directory
        var filteredContents = contents.Where(f => !f.Name.ToLowerInvariant().StartsWith("/ts/"));
        return new KnockoutTsContents(filteredContents);
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        if (subpath.ToLowerInvariant().StartsWith("/ts/"))
        {
            return new NotFoundFileInfo(subpath);
        }
        return inner.GetFileInfo(subpath);
    }

    public IChangeToken Watch(string filter)
    {
        return inner.Watch(filter);
    }
}

// Helper class for filtered directory contents
public class KnockoutTsContents : IDirectoryContents
{
    private readonly IEnumerable<IFileInfo> infos;

    public KnockoutTsContents(IEnumerable<IFileInfo> fileInfos)
    {
        infos = fileInfos;
    }

    public bool Exists => infos.Any();

    public IEnumerator<IFileInfo> GetEnumerator() => infos.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return infos.GetEnumerator();
    }
}

public class ViteProxy
{
    private readonly ILogger<ViteProxy> logger;

    public ViteProxy(ILogger<ViteProxy> logger)
    {
        this.logger = logger;
    }

    // This method will automatically receive the file path of where it's called from
    private static string? GetThisFilePath([CallerFilePath] string? path = null)
    {
        return path;
    }

    public static void LaunchVite(ILogger<ViteProxy> logger, IConfiguration proxyConfig)
    {
        var self = new ViteProxy(logger);
        self.Launch(proxyConfig);
    }

    void Launch(IConfiguration proxyConfig)
    {
        var proxyUri = ProxyAddressFromConfig(proxyConfig);
        if (null == proxyUri)
        {
            throw new ViteProxyError("Failed to find proxy's address in configuration.");
        }
        if (SomethingIsListeningToUri(proxyUri))
        {
            logger.LogInformation("Sounds like an instance of Vite is already listening to {}:{}",
                proxyUri.Host, proxyUri.Port);
            return;
        }

        var currentFilePath = GetThisFilePath();
        var currentDirectory = Path.GetDirectoryName(currentFilePath);
        var scriptsDirectory = Path.Join(currentDirectory, "scripts");

        var npmCmd = FindNpm();

        // Confirm npm is installed.
        using (Process process = new Process())
        {
            process.StartInfo = new ProcessStartInfo(npmCmd, ["-v"])
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = scriptsDirectory
            };

            StringBuilder output = new StringBuilder();
            StringBuilder error = new StringBuilder();

            process.OutputDataReceived += (sender, args) => output.AppendLine(args.Data);
            process.ErrorDataReceived += (sender, args) => error.AppendLine(args.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new ViteProxyError($"npm isn't working.\n{error}"); ;
            }
            logger.LogInformation("Using {} version {}.", npmCmd, output.ToString().Trim());
        }

        // Confirm vite has been installed into node_modules.
        var nodeModulesDirectory = Path.Join(scriptsDirectory, "node_modules");
        if (!Directory.Exists(nodeModulesDirectory))
        {
            throw new ViteProxyError(
                $"Vite hasn't been installed in {nodeModulesDirectory}.\n" +
                "Run the command 'npm ci --ignore-scripts --no-audit' in the /scripts directory to install vite."
            );
        }

        // Finally, launch vite.
        using (Process process = new Process())
        {
            process.StartInfo = new ProcessStartInfo(npmCmd, ["run", "start"])
            {
                UseShellExecute = true,
                WorkingDirectory = Path.Join(scriptsDirectory),
            };

            process.Start();

            var connected = false;
            do
            {
                Thread.Sleep(50);
                connected = SomethingIsListeningToUri(proxyUri);
            } while (!connected);
        }
    }

    public static Uri? ProxyAddressFromConfig(IConfiguration? config)
    {
        if (null == config)
        {
            return null;
        }
        var sections = new List<string>();
        foreach (var kv in config.GetChildren())
        {
            if ("Address" == kv.Key && null != kv.Value)
            {
                return new Uri(kv.Value);
            }
            else if (kv.Value == null)
            {
                sections.Add(kv.Key);
            }
        }
        foreach (var key in sections)
        {
            var uri = ProxyAddressFromConfig(config.GetSection(key));
            if (null != uri)
            {
                return uri;
            }
        }
        return null;
    }

    public static bool SomethingIsListeningToUri(Uri uri)
    {
        using (Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            try
            {
                clientSocket.ConnectAsync(uri.Host, uri.Port).Wait();
                return true;
            }
            catch (Exception)
            {
                return false;   
            }
        }
    }

    public static string FindNpm() {
        #if WINDOWS
        var splitter = ";";
        var npm = "npm.cmd";
        #else
        var splitter = ":";
        var npm = "npm";
        #endif
        foreach (var path in Environment.GetEnvironmentVariable("PATH")!.Split(splitter))
        {
            var testPath = Path.Join(path, npm);
            if (Path.Exists(testPath))
            {
                return testPath;
            }
        }
        throw new ViteProxyError($"Failed to find {npm} in PATH.");
    }
}

public class ViteProxyError : Exception {
    public ViteProxyError(string message) : base(message) { }

    public ViteProxyError(string message, Exception inner) : base(message, inner) { }
}