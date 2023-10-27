module Views

open Giraffe.ViewEngine

// HTMX Attributes
let _hxSwapOob  = attr "hx-swap-oob"
let _hxTarget  = attr "hx-target"
let _hxBoost  = attr "hx-boost"
let _hxGet = attr "hx-get"
let _hxPost = attr "hx-post"
let _hxSwap = attr "hx-swap"
let _hxTrigger = attr "hx-trigger"

// Layouts
let layout (atitle: string) (content: XmlNode list) = [
    html [ _lang "en" ] [
        head [] [
            meta [ _charset "utf-8" ]
            meta [ _name "viewport"
                   _content "width=device-width, initial-scale=1.0" ]
            title []  [ encodedText atitle ]
            link [ _rel  "stylesheet"; _type "text/css";
                   _href "/css/bootstrap/bootstrap.min.css" ]
            link [ _rel  "stylesheet"; _type "text/css";
                   _href "/css/open-iconic/font/css/open-iconic-bootstrap.min.css" ]                   
            link [ _rel  "stylesheet"; _type "text/css";
                   _href "/css/BlazorApp.styles.css" ] ]
        body [] [
            div [ _id "main-layout"] content
            script [ _src "/htmx1.9.6.min.js" ] [] ] ] ]

let boostedLayout (atitle: string) (content: XmlNode list) =
    title [ _hxSwapOob "title" ] [ encodedText atitle ] :: content

let mainLayout (navMenu: XmlNode list) (mainArticle: XmlNode list) = [
    div [ _class "page" ] [
        div [ _class "sidebar" ] navMenu
        main [] [
            div [ _class "top-row px-4" ] [
                a [ _href "/about"
                    _hxBoost "true"
                    _hxTarget "#main-layout" ] [ encodedText "About" ] ]
            article [ _class "content px-4 article"
                      _id "main-article" ] mainArticle ] ] ]

// Navigation menu.
let navItem (requestPath: string) (text: string) (href: string)(oiIcon: string) = 
    let aclass = if href = requestPath then "nav-link active" else "nav-link"
    div [ _class "nav-item px-3" ] [
        a [ _class aclass; _href href ]
          [ span [ _class ("oi oi-" + oiIcon); attr "aria-hidden" "true" ] []
            encodedText text ] ]
                 

let navMenu (path: string) = [
    div [ _class "navbar-top-row ps-3 navbar navbar-dark" ] [
        div [ _class "container-fluid" ] [
            a [ _class "navbar-brand"; _href ""] [ encodedText "BlazorApp" ]
            label [ _for "toggle-menu" ] [
                div [ _title "Navigation menu"; _class "navbar-toggler" ] [
                    span [ _class "navbar-toggler-icon" ] [] ] ] ] ]

    input [ _type "checkbox"; _id "toggle-menu"; _class "visually-hidden" ]

    div [ _id "nav-menu" ] [
        nav [ _class "flex-column"; _hxBoost "true"; _hxTarget "#main-layout" ] [
            navItem path "Home" "/" "home"
            navItem path "Counter" "/counter" "plus"
            navItem path "Fetch data" "/fetchdata" "list-rich" ] ] ]

let survey (title: string) =
    div [ _class "alert alert-secondary mt-4" ] [
        span [ _class "oi oi-pencil me-2"; (attr "aria-hidden" "true")] []
        strong [] [ encodedText title ]
        span [ _class "text-nowrap" ] [
            encodedText " Please take our "
            a [ _target "_blank"; _class "font-weight-bold link-dark"
                _href "https://go.microsoft.com/fwlink/?linkid=2149017" ]
              [encodedText " brief survey"] ]
        encodedText " and tell us what you think." ]

let forecasts (forecasts: Weather.Forecast list) = [
    table [ _class "table"; _hxTrigger "every 2s"; _hxGet "/forecasts";
            _hxSwap "outerHTML" ] [
        thead [] [
        tr [] [
            th [] [rawText "Date"]
            th [] [rawText "Temp. (C)"]
            th [] [rawText "Temp. (F)"]
            th [] [rawText "Summary"] ] ]
        tbody [] (List.map (fun (forecast: Weather.Forecast) -> 
            tr [] [
                td [] [encodedText (forecast.Date.ToShortDateString())]
                td [] [encodedText (string forecast.TemperatureC)]
                td [] [encodedText (string forecast.TemperatureF)]
                td [] [encodedText forecast.Summary] ] ) forecasts ) ] ]


/////////////////////////////////////////////////////////////////////////
// Top level pages.
let index  = [
    h1 [] [ encodedText "Hello, world!"]
    p [] [ encodedText "Welcome to your new app."]
    survey "How is Blazor working for you?" ]

let about = [
    p [] [encodedText "I'm built with"]
    a [ _class "big-link"
        _href "https://giraffe.wiki/"] [ encodedText "F# Giraffe"]
    a [ _class "big-link"
        _href "https://htmx.org/"] [encodedText "HTMX"] ]

let counter (count: int) = [
    form [ _id "increment-form"; _method "post"; _hxPost "/increment";
           _hxSwap "outerHTML"] [
        h1 [] [rawText "Counter" ]
        p [ attr "role" "status" ] [
            rawText "Current count: "
            encodedText (string count) ]
        input [ _type "hidden"; _name "Count"; _value (string (count + 1))]
        input [ _type "submit"; _class "btn btn-primary"; _id "ClickMeButton";
            _value "Click me"] ] ]

let fetchData = [
    h1 [] [rawText "Weather forecast"]
    p [] [rawText "This component demonstrates fetching data from a service."]
    p [ _hxTrigger "every 2s"; _hxGet "/forecasts"; _hxSwap "outerHTML"] [
        em [] [rawText "Loading..."] ] ]