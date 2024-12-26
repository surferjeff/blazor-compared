module Css

open System
open System.Collections.Generic

[<Struct>]
type Declaration = {
    key: string
    value: string
}

let decl (key: string) (value: string) = 
    { key = key; value = value }

let position = decl "position"
let display = decl "display"
let flexDirection = decl "flex-direction"

[<Struct>]
type MediaQuery = {
    Media: string
    DeclText: string
}

[<Struct>]
type ClassDef = {
    MediaQueries: MediaQuery list
    Name: string
    WithRandom: bool
}

let defaultClassDef = {
    MediaQueries = []
    Name = ""
    WithRandom = true
}

let declTextFrom (decls: Declaration list) =
    decls
    |> Seq.fold (fun lines decl -> $"\t{decl.key}: {decl.value};" :: lines) []
    |> String.concat "\n"

let scopedClass (namePrefix: string) =
    { defaultClassDef with Name = namePrefix }

let mediaAll (decls: Declaration list) (classDef: ClassDef) =
    let mediaDecls =  { Media = ""; DeclText = declTextFrom decls } 
    { classDef with MediaQueries = mediaDecls :: classDef.MediaQueries }

let media (media: string) (decls: Declaration list) (classDef: ClassDef) =
    let mediaDecls =  { Media = media; DeclText = declTextFrom decls } 
    { classDef with MediaQueries = mediaDecls :: classDef.MediaQueries }

let randomString (random: Random) (length: int) =
    let chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
    String.init length (fun _ -> string chars[random.Next(chars.Length)])

[<Struct>]
type private ClassDecl = {
    Media: string
    ClassName: string
    DeclText: string
}

type Head() =
    let classNames = SortedDictionary<string, string>()
    let classDecls = SortedSet<ClassDecl>()
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
                DeclText = query.DeclText
            }
            classDecls.Add headKey |> ignore
            
        className

    member this.toStyleText() =
        let mutable lines = []
        let mutable prevQuery = None
        for classDecl in classDecls do
            match prevQuery, classDecl.Media with
            | None, "" -> () // The first group has no enclosing media query.
            | None, query ->
                lines <- $"@media {query} {{" :: lines
                prevQuery <- Some query
            | Some prev, query when prev = query -> () // Continue in same media.
            | Some _, query ->
                lines <- $"@media {query} {{" :: "}" :: lines
                prevQuery <- Some query
            lines <- "}" :: classDecl.DeclText :: $".{classDecl.ClassName} {{" :: lines
        if prevQuery |> Option.isSome then
            lines <- "}" :: lines
        lines |> List.rev |> String.concat "\n"
