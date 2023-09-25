using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class CounterModel : PageModel {
    [BindProperty(SupportsGet = true)]
    public int Count { get; set; } = 0;
    
    public IActionResult OnGet() => Page();

    // Because this handler never receives a post request, I thought the
    // next line of code would be unnecessary.  But if I remove it,
    // I see the error 
    //   InvalidOperationException: Unsupported handler method return type 'ViewComponents.NavMenu'.
    // with an inscrutable stack trace.    
    public IActionResult OnPost() => Page();
}