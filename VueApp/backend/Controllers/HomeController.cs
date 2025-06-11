using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VueBackend.Data;

namespace VueBackend.Controllers;

public class HomeController : Controller
{
    private readonly WeatherForecastService _forecaster;

    public HomeController(WeatherForecastService forecaster)
    {
        _forecaster = forecaster;
    }

    public async Task<IActionResult> Index()
    {
        var forecast = await _forecaster.GetForecastAsync(DateTime.Now);
        return Json(forecast);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return Content($"Error occurred. Request ID: {requestId}", "text/plain");
    }
}
