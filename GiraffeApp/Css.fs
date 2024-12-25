module Css

open System.Collections.Generic

type CssClassDef = string*string list

type CssHead() =
    let classes = SortedDictionary<CssClassDef, string>()

    member this.Add (name: string) (def: CssClassDef) =
        classes[def] <- name
        
