
using Microsoft.AspNetCore.Mvc;

namespace ViewComponents;

public class SurveyPrompt : ViewComponent {
    public string Title { get; set; } = "";

    public IViewComponentResult Invoke(string title) {
        Title = title;
        return View(this);
    }
}