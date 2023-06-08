module SampleProgram

open System
open Krama.Json
open Kitchensink
open KitchensinkJson

let GetStreet (j: JsonNode) : string = j.Get("Address").Get("Street").AsString()
let GetPayloadMaybeNull (j: JsonNode) : Payload option = j.AsPayloadOrNone() 

[<EntryPoint>]
let main argv =

    let jsonStr = Payload.toJson testPayload
    printfn "Encoded - %A" jsonStr

    let p2 = Payload.ofJson jsonStr
    printfn "Decoded - %A" p2

    let street = Json.parse GetStreet jsonStr
    printfn "Street - %A" street

    let payload = Json.parse GetPayloadMaybeNull jsonStr
    printfn "Payload Maybe - %A" payload

    let payload = Json.parse GetPayloadMaybeNull "{}"
    printfn "Payload isNone - %A" payload
    0
