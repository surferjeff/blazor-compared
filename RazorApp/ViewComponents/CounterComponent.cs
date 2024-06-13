
using Microsoft.AspNetCore.Mvc;

namespace ViewComponents;

public class CounterComponent : ViewComponent {
    public IViewComponentResult Invoke(CounterModel? counterModel) {
        return View(counterModel ?? new CounterModel());
    }
}