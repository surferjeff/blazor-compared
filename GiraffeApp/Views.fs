module Views

open Giraffe.ViewEngine

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

let _hx_swap_oob (targetCssSelector: string) = 
    KeyValue("hx-swap-oob", targetCssSelector)

let boostedLayout (atitle: string) (content: XmlNode list) =
    title [ _hx_swap_oob "title" ] [ encodedText atitle ] :: content

let partial () =
    h1 [] [ encodedText "GiraffeApp" ]

let index (message : string) =
    [
        partial()
        p [] [ encodedText message ]
    ] |> layout "Yes!"
