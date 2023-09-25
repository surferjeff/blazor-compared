## praise

I prefer explicit path handler assignment in `hello.go` over ASP.NET's
magic attributes.  It's much easier to understand.

All the Lighthouse numbers for simulated mobile are better than ASP.NET.
* requests: 6 < 8
* kB: 222 < 309
* longest content full paint (seconds): 2.36 < 2.78

This application will be far easier to debug in production, because it uses
human-readable requests and responses containing HTML.

## complaints

Errors in templates are discovered at runtime, not compile time.

Type mismatches in the data passed to templates is discovered only when the template is executed; not at compiled time nor when the template is loaded.

Making reusable nested templates required implementing my own Views.
This is pretty basic.  I don't know how other programmers build websites without it.

There's no way to create CSS that applies only in the current template.

Passing multiple parameters to a template requires fifteen lines of magic code that I don't understand:
https://stackoverflow.com/questions/18276173/calling-a-template-with-several-pipeline-parameters
and of course type errors are not caught until execution time. 

There was no built in way to reverse the elements in an array.

Documentation is frustrating. For example,
https://pkg.go.dev/time#Time.Format says

    See the documentation for the constant called Layout to see how to represent the format.

I still haven't found the documentation for the constant called Layout.
Why on earth would you not make it a hyperlink?

I was never able to get automatic rebuilding (watching) to work with gow.