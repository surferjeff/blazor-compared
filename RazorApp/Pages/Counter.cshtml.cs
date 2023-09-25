using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class CounterModel : PageModel {
    [BindProperty(SupportsGet = true)]
    public int Count { get; set; } = 0;
    
    public IActionResult OnGet() => Page();
}