using MVCApp.Data;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add services to the container.
builder.Services.AddSingleton<WeatherForecastService>();

if ((Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "")
    .ToLower() != "development")
{
    var port = Environment.GetEnvironmentVariable("PORT");
    if (port != null) {
        builder.WebHost.UseUrls($"http://*:{port}");
    }
}

builder.Services.AddSpaStaticFiles(options => {
    options.RootPath = "js";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
} else {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

if (app.Environment.IsDevelopment()) {
    app.UseSpa(spa =>
    {
        spa.Options.SourcePath = "scripts";
        spa.Options.DevServerPort = 5173;
        spa.UseReactDevelopmentServer(npmScript: "start");
    });
}    

app.Run();
