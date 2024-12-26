module Css

open System
open System.Collections.Generic
open Giraffe.ViewEngine

[<Struct>]
type ClassDef = {
    Name: string
    WithRandom: bool
    Decls: (string*string) list
}

let cssClass (namePrefix: string) (decls: (string*string) list) =
    { Name = namePrefix; WithRandom = true; Decls = decls}

let namedClass (decls: (string*string) list) =
    { Name = ""; WithRandom = false; Decls = decls}


let myClass = cssClass "big-yellow " [
    "font-size", "large"
    "background", "yellow"
]

let randomString (length: int) =
    let chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
    let random = Random()
    String.init length (fun _ -> string chars[random.Next(chars.Length)])

let declTextFrom (decls: (string*string) list) =
    let mutable lines = []
    lines <- "}" :: lines
    for decl in decls do
        lines <- $"\t{fst decl}: {snd decl};" :: lines
    lines <- "{" :: lines
    lines |> List.rev |> String.concat "\n"

type Head() =
    let classDefs = SortedDictionary<ClassDef, string>()

    member this.Add (classDef: ClassDef) =
        match (classDefs.TryGetValue classDef), classDef.WithRandom with
        | (true, value), _  -> value
        | _, false -> classDefs.Add(classDef, classDef.Name); classDef.Name
        | _, true ->
            let sep = if classDef.Name = "" then "" else "-"
            let rando = randomString 6
            let name = $"{classDef.Name}{sep}{rando}"
            classDefs.Add(classDef, name)
            name
    
    member this.toXmlNode() =
        let mutable lines = []
        for classDef in classDefs do
            lines <- "}" :: lines
            let classDef, className = classDef.Key, classDef.Value                
            for decl in classDef.Decls do
                lines <- $"\t{fst decl}: {snd decl};" :: lines
            lines <- $"{className} {{" :: lines
        let text = "" :: lines |> List.rev |> String.concat "\n"
        style [] [str text]
