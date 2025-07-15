using Microsoft.Extensions.Options;
using MVCApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add services to the container.
builder.Services.AddSingleton<WeatherForecastService>();

// Configure YARP
var proxyConfig = builder.Configuration.GetSection("ReverseProxy");
builder.Services.AddReverseProxy().LoadFromConfig(proxyConfig);

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

var staticFileOptions = new StaticFileOptions();
if (app.Environment.IsDevelopment()) {
    // Serve /ts files from the vite proxy, not static files from /wwwroot.
    var viteLogger = app.Services.GetService<ILogger<ViteProxy>>()!;
    try
    {
        ViteProxy.LaunchVite(viteLogger, proxyConfig);
        app.MapReverseProxy();
        staticFileOptions.FileProvider = new KnockoutTs(app.Environment.WebRootFileProvider);
    }
    catch (Exception e)
    {
        viteLogger.LogError("Hot reloading Typescript is DISABLED because\n{}",
        e.Message);
    }
}

app.UseStaticFiles(staticFileOptions);


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
