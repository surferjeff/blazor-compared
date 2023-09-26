using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class CounterModel : PageModel {
    [BindProperty(SupportsGet = true)]
    public int Count { get; set; } = 0;
    public string Path { get; set; } = "";    

    public IActionResult OnGet() {
        Path = HttpContext.Request.Path;
        return Page();
    }
}