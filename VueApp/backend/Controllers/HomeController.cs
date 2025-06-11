using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace VueBackend.Controllers;

public class HomeController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return Content($"Error occurred. Request ID: {requestId}", "text/plain");
    }
}
