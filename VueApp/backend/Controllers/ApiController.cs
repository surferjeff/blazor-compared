using Microsoft.AspNetCore.Mvc;
using VueBackend.Data;

namespace VueBackend.Controllers;

public class ApiController : Controller
{
    private readonly WeatherForecastService _forecaster;

    public ApiController(WeatherForecastService forecaster)
    {
        _forecaster = forecaster;
    }

    public async Task<IActionResult> Forecasts()
    {
        var forecast = await _forecaster.GetForecastAsync(DateTime.Now);
        return Json(forecast);
    }
}
