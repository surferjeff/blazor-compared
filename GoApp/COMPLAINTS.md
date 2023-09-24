Errors in templates are discovered at runtime, not compile time.

Type mismatches in the data passed to templates is discovered only when the template is executed; not at compiled time nor when the template is loaded.

Making reusable nested templates required implementing my own Views.
This is pretty basic.  I don't know how other programmers build websites without it.

There's no way to create CSS that applies only in the current template.

Passing multiple parameters to a template requires fifteen lines of magic code that I don't understand:
https://stackoverflow.com/questions/18276173/calling-a-template-with-several-pipeline-parameters
and of course type errors are not caught until execution time. 

There was no built in way to reverse the elements in an array.