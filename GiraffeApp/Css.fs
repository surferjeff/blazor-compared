module Css

open System
open System.Collections.Generic

[<Struct>]
type ClassDef = {
    Name: string
    WithRandom: bool
    DeclText: string
}

let declTextFrom (decls: (string*string) list) =
    decls
    |> Seq.fold (fun lines decl -> $"\t{fst decl}: {snd decl};" :: lines) []
    |> String.concat "\n"

let cssClass (namePrefix: string) (decls: (string*string) list) =
    { Name = namePrefix; WithRandom = true; DeclText = declTextFrom decls }

let cssClassText (namePrefix: string) (declText: string) =
    { Name = namePrefix; WithRandom = true; DeclText = declText }

let namedClass (decls: (string*string) list) =
    { Name = ""; WithRandom = false; DeclText = declTextFrom decls }

let namedClassText (declText: string) =
    { Name = ""; WithRandom = false; DeclText = declText }

let randomString (random: Random) (length: int) =
    let chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
    String.init length (fun _ -> string chars[random.Next(chars.Length)])

type Head() =
    let classDefs = SortedDictionary<ClassDef, string>()
    let randomStr() = randomString (Random()) 6

    member this.Add (classDef: ClassDef) =
        match (classDefs.TryGetValue classDef), classDef.WithRandom with
        | (true, value), _  -> value
        | _, false -> classDefs.Add(classDef, classDef.Name); classDef.Name
        | _, true ->
            let sep = if classDef.Name = "" then "" else "-"
            let name = $"{classDef.Name}{sep}{randomStr()}"
            classDefs.Add(classDef, name)
            name
    
    member this.toStyleText() =
        let mutable lines = []
        for classDef in classDefs do
            lines <- "}" :: lines
            let classDef, className = classDef.Key, classDef.Value                
            lines <- $".{className} {{" :: classDef.DeclText :: lines
        "" :: lines |> String.concat "\n"
