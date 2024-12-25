module Css

open System
open System.Collections.Generic


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

type CssHead() =
    let classes = SortedDictionary<ClassDef, string>()

    member this.Add (classDef: ClassDef) =
        match (classes.TryGetValue classDef), classDef.WithRandom with
        | (true, value), _  -> value
        | _, false -> classes.Add(classDef, classDef.Name); classDef.Name
        | _, true ->
            let sep = if classDef.Name = "" then "" else "-"
            let rando = randomString 6
            let name = $"{classDef.Name}{sep}{rando}"
            classes.Add(classDef, name)
            name

