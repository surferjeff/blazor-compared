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
#if WINDOWS
using Meziantou.Framework.Win32;
#endif

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

    public static bool LaunchVite(ILogger<ViteProxy> logger)
    {
        try
        {
            var self = new ViteProxy(logger);
            return self.Launch();
        }
        catch (Exception e)
        {
            logger.LogWarning("Hot reloading TypeScript files is DISABLED because\n{}", e.Message);
            return false;
        }
    }    

    bool Launch()
    {
        var currentFilePath = GetThisFilePath();
        var currentDirectory = Path.GetDirectoryName(currentFilePath);
        var scriptsDirectory = Path.Join(currentDirectory, "scripts");

        var npmCmd = "";
        foreach (var path in Environment.GetEnvironmentVariable("PATH")!.Split(";"))
        {
            var testPath = Path.Join(path, "npm.cmd");
            if (Path.Exists(testPath))
            {
                npmCmd = testPath;
                break;
            }
        }


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
                logger.LogWarning("Hot reloading TypeScript files is DISABLED because npm isn't installed or maybe it's just not working. {}",
                    error.ToString());
                return false;
            }
            logger.LogInformation("Found npm version {}.", output.ToString().Trim());
        }

        // Confirm vite has been installed into node_modules.
        var nodeModulesDirectory = Path.Join(scriptsDirectory, "node_modules");
        if (!Directory.Exists(nodeModulesDirectory))
        {
            logger.LogWarning(
                "Hot reloading TypeScript files is DISABLED because vite hasn't been installed in {}.\n" +
                "Run the command 'npm ci --ignore-scripts --no-audit' in the /scripts directory to install vite.",
                nodeModulesDirectory
            );
            return false;
        }

        #if WINDOWS
        // Create the Job object and assign it to the current process
        using var job = new JobObject();
        job.SetLimits(new JobObjectLimits()
        {
            Flags = JobObjectLimitFlags.DieOnUnhandledException |
                    JobObjectLimitFlags.KillOnJobClose,
        });

        job.AssignProcess(Process.GetCurrentProcess());
        #endif

        // Finally, launch vite.
        using (Process process = new Process())
        {
            process.StartInfo = new ProcessStartInfo(npmCmd, ["run", "start"])
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = Path.Join(scriptsDirectory),
            };

            StringBuilder output = new StringBuilder();
            var started = false;
            var exited = false;

            process.OutputDataReceived += (sender, args) =>
            {
                if (!started)
                {
                    output.AppendLine(args.Data);
                    if (output.ToString().Contains("Starting the development server"))
                    {
                        started = true;
                    }
                }
                logger.LogInformation("{}", args.Data);
            };
            process.ErrorDataReceived += (sender, args) =>
            {
                logger.LogWarning(args.Data);

            };
            process.Exited += (sender, args) =>
            {
                logger.LogWarning("Process exited.");
                exited = true;
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            do
            {
                Thread.Sleep(50);
                if (exited)
                {
                    logger.LogWarning("Hot reloading TypeScript files is DISABLED because npm exited unexpectedly.");
                    return false;
                }
            } while (!started);
            logger.LogInformation("Hot reloading TypeScript files is enabled.  Process {}", process.Id);
            return true;
        }
    }
}    
 