
using Microsoft.AspNetCore.Mvc;

namespace ViewComponents;

public class CounterComponent : ViewComponent {
    public IViewComponentResult Invoke(CounterModel model) {
        return View(model);
    }
}