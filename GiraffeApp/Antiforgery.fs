module Antiforgery

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Antiforgery
open Giraffe
open Giraffe.ViewEngine

// Generates a CSRF token using the Microsoft.AspNetCore.Antiforgery package,
// which is fed into the provided handler
let csrfTokenizer (handler : AntiforgeryTokenSet -> HttpHandler) : HttpHandler =
    fun (next: HttpFunc) (ctx : HttpContext) ->                
        let antiFrg = ctx.GetService<IAntiforgery>()
        handler (antiFrg.GetAndStoreTokens ctx) next ctx

// Injects a newly generated CSRF token into a Giraffe.GiraffeViewEngine.XmlNode
let csrfHtmlView (view : AntiforgeryTokenSet -> XmlNode) : HttpHandler =            
    let handler token : HttpHandler =
        fun (next: HttpFunc) (ctx : HttpContext) ->              
            htmlView (view token) next ctx

    csrfTokenizer handler  

// Checks the presence and validity of CSRF token and calls invalidTokenHandler on failure
// GET, HEAD, OPTIONS & TRACE always validate as true
let requiresCsrfToken (invalidTokenHandler : HttpHandler) : HttpHandler =
    fun (next: HttpFunc) (ctx : HttpContext) ->                                
        let antiFrg = ctx.GetService<IAntiforgery>()        
        (match antiFrg.IsRequestValidAsync(ctx) |> Async.AwaitTask |> Async.RunSynchronously with
        | true  -> next        
        | false -> invalidTokenHandler earlyReturn) ctx