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

app.UseStaticFiles();

app.UseRouting();

// Map YARP reverse proxy BEFORE your MVC endpoints
// app.MapReverseProxy();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
