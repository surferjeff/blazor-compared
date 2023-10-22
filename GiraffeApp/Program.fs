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

// ---------------------------------
// Web app
// ---------------------------------

let rec writeBytesAsync (ctx: HttpContext option)(bytesList: byte[] list) =
    task {
        match (ctx, bytesList) with
        | (Some(x), head :: tail) ->
            let! nextCtx = x.WriteBytesAsync head
            return! writeBytesAsync nextCtx tail
        | (finalCtx, _) -> return finalCtx
    }

/// <summary>
/// <para>Compiles a `Giraffe.GiraffeViewEngine.XmlNode` object to a HTML view and writes the output to the body of the HTTP response.</para>
/// <para>It also sets the HTTP header `Content-Type` to `text/html` and sets the `Content-Length` header accordingly.</para>
/// </summary>
/// <param name="htmlView">An `XmlNode` object to be send back to the client and which represents a valid HTML view.</param>
/// <returns>A Giraffe `HttpHandler` function which can be composed into a bigger web application.</returns>
let htmlViews (htmlNodes : XmlNode list)(_ : HttpFunc) (ctx: HttpContext) : HttpFuncResult =
    let bytesList = List.map RenderView.AsBytes.htmlDocument htmlNodes
    ctx.SetContentType "text/html; charset=utf-8"
    writeBytesAsync (Some ctx) bytesList

let indexHandler (next: HttpFunc)(ctx: HttpContext): HttpFuncResult =
    let boosted = match ctx.Request.Headers.HxBoosted with
                    | Some(b) -> b
                    | None -> false
    let view =  Views.index ctx.Request.Path.Value boosted
    htmlViews view next ctx

let webApp =
    choose [
        GET >=>
            choose [
                route "/" >=> indexHandler
                route "/about" >=> htmlViews Views.about
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