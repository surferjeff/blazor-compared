using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class CounterModel : PageModel {
    [BindProperty(SupportsGet = true)]
    public int Count { get; set; } = 0;
    public string Layout { get; set; } = "_Layout";
    public string Path { get; set; } = "";    

    public IActionResult OnGet() {
        Path = HttpContext.Request.Path;
        var headers = HttpContext.Request.Headers;
        if (headers["HX-Request"].Contains("true")
            && !headers["HX-Boosted"].Contains("true"))
        {
            Layout = "_LayoutBodyOnly";
        }
        return Page();
    }
}