## praise

I prefer explicit path handler assignment in `hello.go` over ASP.NET's
magic attributes.  It's much easier to understand.  There's just way less magic everywhere.  Nothing is happening automatically or mysteriously.

All the Lighthouse numbers for simulated mobile are better than BlazerApp.
* requests: 6 < 8
* kB: 222 < 309
* longest content full paint (seconds): 2.36 < 2.78

This application will be far easier to debug in production, because it uses
human-readable requests and responses containing HTML.

## complaints

Fiber transformed the request header "HX-Boosted" into "Hx-Boosted".
That was very difficult to track down, and I don't see what sense it makes.

# show stoppers

While trying to add protection against CSRF attacks (see the csrf branch),
I learned there were two critical flaws in Fiber's anti-CSRF:
https://www.cve.org/CVERecord?id=CVE-2023-45128
https://www.cve.org/CVERecord?id=CVE-2023-45141

Unbelievable.  CSRF isn't new.  It's very well defined with well-defined
solutions and the Fiber developers just failed.  I don't think I can trust
them.  What other vulnerabilities are lurking?