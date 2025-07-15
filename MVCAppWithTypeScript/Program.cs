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

if (app.Environment.IsDevelopment())
{
    app.MapReverseProxy(pipeline =>
    {
        // There's no way to call app.UseStaticFiles() and then have vite's proxy
        // take priority over paths that could be served by both.
        // Therefore, we have to implement a light-weight UseStaticFiles() here.
        pipeline.Use(async (context, next) =>
        {
            await next();

            Console.WriteLine($"{context.Response.StatusCode} {context.Request.Path}");
            if (context.Response.StatusCode == 404)
            {
                Console.WriteLine(context.Request.Path);

                var env = app.Services.GetRequiredService<IWebHostEnvironment>();
                var fileProvider = env.WebRootFileProvider;
                var fileInfo = fileProvider.GetFileInfo(context.Request.Path);

                if (fileInfo.Exists)
                {
                    await context.Response.SendFileAsync(fileInfo);
                }
            }
        });
    });
}
else
{
    app.UseStaticFiles();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
