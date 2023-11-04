module Giraffe.GiraffeViewEngine.Antiforgery

open Microsoft.AspNetCore.Antiforgery
open Giraffe.ViewEngine

let antiforgeryInput (token : AntiforgeryTokenSet) =
    input [ 
            _type "hidden"
            _name token.FormFieldName
            _value token.RequestToken 
        ]