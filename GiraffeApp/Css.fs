module Css

open System
open System.Collections.Generic

[<Struct>]
type Property = {
    key: string
    value: string
}

let property (key: string) (value: string) = 
    { key = key; value = value }

let position = property "position"
let display = property "display"
let flexDirection = property "flex-direction"

[<Struct>]
type MediaQuery = {
    Media: string
    PropText: string
}

[<Struct>]
type ClassDef = {
    MediaQueries: MediaQuery list
    Name: string
    WithRandom: bool
}

let private defaultClassDef = {
    MediaQueries = []
    Name = ""
    WithRandom = true
}

let private propTextFrom (props: Property list) =
    props
    |> Seq.fold (fun lines prop -> $"\t{prop.key}: {prop.value};" :: lines) []
    |> String.concat "\n"

let scopedClass (namePrefix: string) =
    { defaultClassDef with Name = namePrefix }

let mediaAll (props: Property list) (classDef: ClassDef) =
    let mediaProps =  { Media = ""; PropText = propTextFrom props } 
    { classDef with MediaQueries = mediaProps :: classDef.MediaQueries }

let media (media: string) (props: Property list) (classDef: ClassDef) =
    let mediaProps =  { Media = media; PropText = propTextFrom props } 
    { classDef with MediaQueries = mediaProps :: classDef.MediaQueries }

let private randomString (random: Random) (length: int) =
    let chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
    String.init length (fun _ -> string chars[random.Next(chars.Length)])

[<Struct>]
type private ClassDecl = {
    Media: string
    ClassName: string
    PropText: string
}

type Head() =
    let classNames = SortedDictionary<string, string>()
    let classProps = SortedSet<ClassDecl>()
    let randomStr() = randomString (Random()) 6

    member this.Add (classDef: ClassDef) =
        let className =
            match classNames.TryGetValue classDef.Name with
            | true, name -> name
            | _ -> 
                let name =
                    if classDef.WithRandom then $"{classDef.Name}-{randomStr()}"
                    else classDef.Name
                classNames.Add(classDef.Name, name)
                name

        for query in classDef.MediaQueries do
            let headKey = {
                Media = query.Media
                ClassName = className
                PropText = query.PropText
            }
            classProps.Add headKey |> ignore
            
        className

    member this.toStyleText() =
        let mutable lines = []
        let mutable prevQuery = None
        for classDecl in classProps do
            match prevQuery, classDecl.Media with
            | None, "" -> () // The first group has no enclosing media query.
            | None, query ->
                lines <- $"@media {query} {{" :: lines
                prevQuery <- Some query
            | Some prev, query when prev = query -> () // Continue in same media.
            | Some _, query ->
                lines <- $"@media {query} {{" :: "}" :: lines
                prevQuery <- Some query
            lines <- "}" :: classDecl.PropText :: $".{classDecl.ClassName} {{" :: lines
        if prevQuery |> Option.isSome then
            lines <- "}" :: lines
        lines |> List.rev |> String.concat "\n"
