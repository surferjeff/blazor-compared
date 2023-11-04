module GiraffeApp.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Middleware
open MiddleHtmx
open Weather

// ---------------------------------
// Web app
// ---------------------------------


// Post payload for /increment.
[<CLIMutable>]
type IncrementForm = { Count: int }

// Handler for /increment.  Returns new HTML with incremented count and a new
// form.
let incrementHandler: HttpHandler =
    getAndStoreAntiforgeryTokens (fun tokens ->
        bindForm<IncrementForm> None (fun payload -> 
                htmlNodes (Views.counter payload.Count tokens)))

let webApp =
    choose [
        GET >=>
            choose [
                route "/" >=> pageHandler "Home" Views.index
                route "/counter" >=> getAndStoreAntiforgeryTokens (
                    fun tokens -> pageHandler "Counter" (Views.counter 0 tokens))
                route "/about" >=> pageHandler "About" Views.about
                route "/fetchdata" >=> pageHandler "Weather forecast"
                    Views.fetchData
                route "/forecasts" >=> warbler (fun _ ->
                    makeRandomForecasts 5 |> Views.forecasts |> htmlNodes)
            ]
        POST >=> route "/increment" >=> requireAntiforgeryToken >=> incrementHandler
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
    services.AddAntiforgery() |> ignore
    services.AddGiraffe() |> ignore

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