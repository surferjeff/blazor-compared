using Microsoft.Extensions.Options;
using MVCApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add services to the container.
builder.Services.AddSingleton<WeatherForecastService>();

if ((Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "")
.ToLower() == "development")
{
    builder.Services.AddHostedService<HotTypeScript>();
}
else
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

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
