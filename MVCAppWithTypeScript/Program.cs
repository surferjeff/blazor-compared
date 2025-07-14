using MVCApp.Data;

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

if (app.Environment.IsDevelopment()) {
    // There's no way to call app.UseStaticFiles() and then have the proxy
    // take priority over paths that could be served by both.
    // Therefore, we have to implement a light-weight UseStaticFiles() here.
    #pragma warning disable ASP0014 // Suggest using top level route registrations
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapReverseProxy();

        endpoints.MapFallback(async context =>
        {
            var env = app.Services.GetRequiredService<IWebHostEnvironment>();
            var fileProvider = env.WebRootFileProvider;
            var fileInfo = fileProvider.GetFileInfo(context.Request.Path);

            if (fileInfo.Exists)
            {
                await context.Response.SendFileAsync(fileInfo);
            }
            else
            {
                context.Response.StatusCode = 404;
            }
        });
    });
} else {
    app.UseStaticFiles();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
