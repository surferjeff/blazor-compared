module Views

open Giraffe.ViewEngine

// HTMX Attributes
let _hx_swap_oob  = attr "hx-swap-oob"
let _hx_target  = attr "hx-target"
let _hx_boost  = attr "hx-boost"

// Layouts
let layout (atitle: string) (content: XmlNode list) = [
    html [ _lang "en" ] [
        head [] [
            meta [ _charset "utf-8" ]
            meta [ _name "viewport"
                   _content "width=device-width, initial-scale=1.0" ]
            title []  [ encodedText atitle ]
            link [ _rel  "stylesheet"
                   _type "text/css"
                   _href "/css/bootstrap/bootstrap.min.css" ]
            link [ _rel  "stylesheet"
                   _type "text/css"
                   _href "/css/open-iconic/font/css/open-iconic-bootstrap.min.css" ]                   
            link [ _rel  "stylesheet"
                   _type "text/css"
                   _href "/css/BlazorApp.styles.css" ]
        ]
        body [] [
            div [ _id "main-layout"] content
            script [ _src "/htmx1.9.6.min.js" ] []
        ]
    ] ]

let boostedLayout (atitle: string) (content: XmlNode list) =
    title [ _hx_swap_oob "title" ] [ encodedText atitle ] :: content

let mainLayout (navMenu: XmlNode list) (mainArticle: XmlNode list) = [
    div [ _class "page" ] [
        div [ _class "sidebar" ] navMenu
        main [] [
            div [ _class "top-row px-4" ] [
                a [ _href "/about"
                    _hx_boost "true"
                    _hx_target "#main-layout" ] [ encodedText "About" ] ]
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
        nav [ _class "flex-column"; _hx_boost "true"; _hx_target "#main-layout" ] [
            navItem path "Home" "/" "home"
            navItem path "Counter" "/counter" "plus"
            navItem path "Fetch data" "/fetchdata" "list-rich" ] ] ]

/////////////////////////////////////////////////////////////////////////

let index (requestPath: string)(boosted: bool) =
    let menu = navMenu requestPath
    let main = mainLayout menu [
        h1 [] [ encodedText "Hello, world!"]
        p [] [ encodedText "Welcome to your new app."]
    ]
    let lout = if boosted then boostedLayout else layout
    lout "Home" main

let about =
    let menu = navMenu "/about"
    let main = mainLayout menu [
        p [] [encodedText "I'm built with"]
        a [ _class "big-link"
            _href "https://giraffe.wiki/"] [ encodedText "F# Giraffe"]
        a [ _class "big-link"
            _href "https://htmx.org/"] [encodedText "HTMX"]
    ]
    layout "About" main