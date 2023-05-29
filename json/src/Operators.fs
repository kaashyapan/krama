namespace Krama.Json

open FSharp
open FSharp.Core

[<AutoOpen>]
module Operators =
    let (?) (jsonObj: JsonNode) (name: string) : JsonNode = jsonObj.Get(name)

    //Optional on the both sides
    let (<??>) (jsonObj: JsonNode option) (name: string) =
        match jsonObj with
        | Some jdoc -> jdoc.TryGet(name)
        | None -> None

    //Not Optional
    let (</>) (jsonObj: JsonNode) (name: string) : JsonNode = jsonObj.Get(name)

    //Optional on the right
    let (</?>) (jsonObj: JsonNode) (name: string) = jsonObj.TryGet(name)

    [<RequireQualifiedAccess>]
    module JsonNode =
        //Pipeline functions for convenience
        let get (property: string) (jsonObj: JsonNode) : JsonNode = jsonObj.Get(property)

        let tryGet (property: string) (jsonObj: JsonNode) : JsonNode option = jsonObj.TryGet(property)

        module Optional =
            let get (property: string) (jsonObj: JsonNode option) : JsonNode option =
                match jsonObj with
                | Some j -> j.Get(property) |> Some
                | None -> None

            let tryGet (property: string) (jsonObj: JsonNode option) : JsonNode option =
                match jsonObj with
                | Some j -> j.TryGet(property)
                | None -> None
