module Views

open Giraffe.ViewEngine

// HTMX Attributes
let _hx_swap_oob  = attr "hx-swap-oob"
let _hx_target  = attr "hx-target"
let _hx_boost  = attr "hx-swap-oob"

// Layouts
let layout (atitle: string) (content: XmlNode list) =
    html [] [
        head [ _lang "en" ] [
            meta [ _charset "utf-8" ]
            meta [ _name "viewport"
                   _content "width=device-width, initial-scale=1.0" ]
            title []  [ encodedText atitle ]
            link [ _rel  "stylesheet"
                   _type "text/css"
                   _href "/css/BlazorApp.styles.css" ]
            link [ _rel  "stylesheet"
                   _type "text/css"
                   _href "/css/bootstrap/bootstrap.min.css" ]
            link [ _rel  "stylesheet"
                   _type "text/css"
                   _href "/css/open-iconic/font/css/open-iconic-bootstrap.min.css" ]                   
        ]
        body [] [
            div [ _id "main-layout"] content
            script [ _src "/htmx1.9.6.min.js" ] []
        ]
    ]

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
let navItem (requestPath: string) (text: string) (href: string) = 
    let aclass = if href = requestPath then "nav-link active" else "nav-link"
    [ div [ _class "nav-item px-3" ] [
        a [ _class aclass; _href href ]
          [ span [ _class "oi oi-home"; attr "aria-hidden" "true" ]
                 [ encodedText text ] ] ] ]


/////////////////////////////////////////////////////////////////////////
let partial () =
    h1 [] [ encodedText "GiraffeApp" ]


let index (message : string) =
    [
        partial()
        p [] [ encodedText message ]
    ] |> layout "Yes!"
