module GiraffeApp.MiddleHtmx

open Giraffe
open Giraffe.ViewEngine
open Microsoft.AspNetCore.Http
open Giraffe.Htmx
open GiraffeApp.Middleware

// Given a title and a view, renders the view inside the main layout.
// Chooses a layout based on whether the request is boosted.
let pageHandler (title: string)(view: XmlNode list)(next: HttpFunc)(ctx: HttpContext): HttpFuncResult =
    let boosted = Option.defaultValue false ctx.Request.Headers.HxBoosted
    let menu = Views.navMenu ctx.Request.Path.Value
    let main = Views.mainLayout menu view
    let layout = if boosted then Views.boostedLayout else Views.layout
    let nodes = layout title main
    ctx.SetHttpHeader ("Vary", "HX-Boosted")
    htmlNodes nodes next ctx
