module GiraffeApp.Middleware

open Giraffe
open Giraffe.ViewEngine
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Antiforgery

// Writes the nodes to the response as HTML.  Sets the content type to text/html.
// Also sets the Content-Length header.
let htmlNodes (htmlNodes : XmlNode list) : HttpHandler =
    let bytes = RenderView.AsBytes.htmlNodes htmlNodes
    fun (_ : HttpFunc) (ctx : HttpContext) ->
        ctx.SetContentType "text/html; charset=utf-8"
        ctx.WriteBytesAsync bytes
    
// Middleware to generate an anti-forgery token and store it in the response
// cookie.  Invokes argument with the token.
let getAndStoreAntiforgeryTokens (f: AntiforgeryTokenSet -> HttpHandler): HttpHandler =
    fun (next: HttpFunc)(ctx: HttpContext) ->
        let af = ctx.GetService<IAntiforgery>()
        f (af.GetAndStoreTokens ctx) next ctx

// Middleware to require an antiforgery token for a request
let requireAntiforgeryToken: HttpHandler =
    authorizeRequest (fun ctx -> 
        let af = ctx.GetService<IAntiforgery>()
        af.IsRequestValidAsync(ctx) |> Async.AwaitTask
            |> Async.RunSynchronously)
        (RequestErrors.forbidden (text "Forbidden"))

