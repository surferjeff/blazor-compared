
using Microsoft.AspNetCore.Mvc;

namespace ViewComponents;

public class NavMenu : ViewComponent {
    public string Path { get; set; } = "";

    public IViewComponentResult Invoke() {
        Path = ViewContext.HttpContext.Request.Path;
        return View(this);
    }
}