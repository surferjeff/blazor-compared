module CssClasses
open Css

let mediaWide = media "(min-width: 641px)"

let pageClass =
    scopedClass "page"
    |> mediaAll [
            position "relative"
            display "flex"
            flexDirection "column"]
    |> mediaWide [
        flexDirection "row"
    ]

let sidebarClass =
    scopedClass "sidebar"
    |> mediaAll [
        backgroundImage "linear-gradient(180deg, rgb(5, 39, 103) 0%, #3a0647 70%)" ]
    |> mediaWide [
        width  "250px"
        height "100vh"
        position "sticky"
        top "0"
    ]
