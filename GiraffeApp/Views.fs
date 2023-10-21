module Views

open Giraffe.ViewEngine

let layout (content: XmlNode list) =
    html [] [
        head [] [
            title []  [ encodedText "GiraffeApp" ]
            link [ _rel  "stylesheet"
                   _type "text/css"
                   _href "/main.css" ]
        ]
        body [] content
    ]

let partial () =
    h1 [] [ encodedText "GiraffeApp" ]

let index (message : string) =
    [
        partial()
        p [] [ encodedText message ]
    ] |> layout
