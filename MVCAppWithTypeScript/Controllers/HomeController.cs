using Microsoft.AspNetCore.Mvc;
using MVCApp.Data;

namespace MVCApp.Controllers;

[Route("/")]
public class HomeController : Controller {
    WeatherForecastService _weatherService;

    public HomeController(WeatherForecastService weatherService) {
        _weatherService = weatherService;
    }
    
    [HttpGet("")]
    public IActionResult Index() => View();

    [HttpGet("About")]
    public IActionResult About() => View();

    [HttpGet("Counter")]
    public IActionResult Counter() => View();

    [HttpGet("FetchData")]
    public IActionResult FetchData() => View();

    [HttpGet("Forecasts")]
    public async Task<IActionResult> Forecasts() {
        var forecast = await _weatherService.GetForecastAsync(DateTime.Now);
        return View(forecast);
    }

}