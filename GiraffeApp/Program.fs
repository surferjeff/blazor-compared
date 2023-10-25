module GiraffeApp.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Giraffe.Htmx
open Giraffe.ViewEngine
open System.Globalization
open Weather

// ---------------------------------
// Web app
// ---------------------------------

let htmlNodes (htmlNodes : XmlNode list) : HttpHandler =
    let bytes = RenderView.AsBytes.htmlNodes htmlNodes
    fun (_ : HttpFunc) (ctx : HttpContext) ->
        ctx.SetContentType "text/html; charset=utf-8"
        ctx.WriteBytesAsync bytes

let pageHandler (title: string)(view: XmlNode list)(next: HttpFunc)(ctx: HttpContext): HttpFuncResult =
    let boosted = Option.defaultValue false ctx.Request.Headers.HxBoosted
    let menu = Views.navMenu ctx.Request.Path.Value
    let main = Views.mainLayout menu view
    let layout = if boosted then Views.boostedLayout else Views.layout
    let nodes = layout title main
    ctx.SetHttpHeader ("Vary", "HX-Boosted")
    htmlNodes nodes next ctx
    
[<CLIMutable>]
type CountPayload = { Count: int }

let incrementHandler: HttpHandler =
    let culture = CultureInfo.CreateSpecificCulture("en-US")
    bindQuery<CountPayload> (Some culture) (fun payload -> 
        htmlNodes (Views.counter payload.Count)
    )

let webApp =
    choose [
        GET >=>
            choose [
                route "/" >=> pageHandler "Home" Views.index
                route "/counter" >=> pageHandler "Counter" (Views.counter 0)
                route "/about" >=> pageHandler "About" Views.about
                route "/increment" >=> incrementHandler
                route "/fetchdata" >=> pageHandler "Weather forecast"
                    Views.fetchData
                route "/forecasts" >=> htmlNodes (Views.forecasts (makeRandomForecasts 5))
            ]
        setStatusCode 404 >=> text "Not Found" ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder : CorsPolicyBuilder) =
    builder
        .WithOrigins(
            "http://localhost:5000")
       .AllowAnyMethod()
       .AllowAnyHeader()
       |> ignore

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
    (match env.IsDevelopment() with
    | true  ->
        app.UseDeveloperExceptionPage()
    | false ->
        app .UseGiraffeErrorHandler(errorHandler))
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseGiraffe(webApp)

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore
    services.AddAntiforgery() |> ignore

let configureLogging (builder : ILoggingBuilder) =
    builder.AddConsole()
           .AddDebug() |> ignore

[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .UseContentRoot(contentRoot)
                    .UseWebRoot(webRoot)
                    .Configure(Action<IApplicationBuilder> configureApp)
                    .ConfigureServices(configureServices)
                    .ConfigureLogging(configureLogging)
                    |> ignore)
        .Build()
        .Run()
    0