
using Microsoft.AspNetCore.Mvc;

namespace ViewComponents;

public class MainLayout : ViewComponent {
    public IViewComponentResult Invoke() {
        return View(this);
    }
}