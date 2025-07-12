using Microsoft.AspNetCore.Mvc;

namespace MVCApp.Controllers;

[Route("/")]
public class HomeController : Controller {
    [HttpGet("")]
    public IActionResult Index() => View();

    [HttpGet("About")]
    public IActionResult About() => View();
}