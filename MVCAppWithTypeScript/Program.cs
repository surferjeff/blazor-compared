using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using MVCApp.Data;

/// <summary>
/// Doesn't serve /js/** from the file system because we want to proxy those
/// requests to vite.
/// </summary>
internal class KnockoutJs : IFileProvider
{
    IFileProvider inner;

    public KnockoutJs(IFileProvider inner)
    {
        this.inner = inner;
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        Console.WriteLine($"GetDirectoryContents({subpath})");
        var contents = inner.GetDirectoryContents(subpath);
        if (!contents.Exists)
        {
            return contents;
        }

        // Filter out the excluded directory
        var filteredContents = contents.Where(f => !f.Name.ToLowerInvariant().StartsWith("/js/"));
        return new KnockoutJsContents(filteredContents);
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        Console.WriteLine($"GetFileInfo({subpath})");
        if (subpath.ToLowerInvariant().StartsWith("/js/"))
        {
            return new NotFoundFileInfo(subpath);
        }
        return inner.GetFileInfo(subpath);
    }

    public IChangeToken Watch(string filter)
    {
        Console.WriteLine($"Watch({filter})");
        return inner.Watch(filter);
    }
}

// Helper class for filtered directory contents
public class KnockoutJsContents : IDirectoryContents
{
    private readonly IEnumerable<IFileInfo> infos;

    public KnockoutJsContents(IEnumerable<IFileInfo> fileInfos)
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

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Add services to the container.
        builder.Services.AddSingleton<WeatherForecastService>();

        // Configure YARP
        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

        if ((Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "")
        .ToLower() != "development")
        {
            var port = Environment.GetEnvironmentVariable("PORT");
            if (port != null)
            {
                builder.WebHost.UseUrls($"http://*:{port}");
            }
        }

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseRouting();

        if (app.Environment.IsDevelopment())
        {
            // Serve /js files from the vite proxy, not from /wwwroot.
            var options = new StaticFileOptions
            {
                FileProvider = new KnockoutJs(app.Environment.WebRootFileProvider)
            };
            app.UseStaticFiles(options);
            app.MapReverseProxy();
        }
        else
        {
            app.UseStaticFiles();
        }

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}