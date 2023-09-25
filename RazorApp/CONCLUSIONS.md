# praise

Templates are type checked at compile time. This is huge.

Razor templates let me *flexibly* render the model.

All the Lighthouse numbers for simulated mobile are better than BlazerApp.

This application will be far easier to debug in production, because it uses
human-readable requests and responses containing HTML.

Aside from FetchData, everything still works without javascript.

CSS is scoped to its component.

## complaints

Creating reusable components is clunky. The file is always named Default.cshtml,
and the ViewComponent.cs file lives in a totally different directory.

I frequently saw errors when `dotnet watch` hot reloaded some code
and the page was broken.  Restarting dotnet fixed the problem.