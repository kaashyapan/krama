<img src="./ganapati.png" alt="Ganapati" width="100"/>

# Krama - JSON library for F#
#### With code generation (TODO). Fable friendly.
[nuget](https://www.nuget.org/packages/Krama.Json)

This library was inspired by [Jay](https://github.com/pimbrouwers/Jay) which is in turn inspired by [FSharp.Data](https://github.com/fsprojects/FSharp.Data/blob/main/src/Json/JsonDocument.fs).
Also inspired by [Thoth](https://github.com/thoth-org/Thoth.Json) which is probably inspired by [Elm.Json](https://package.elm-lang.org/packages/elm/json/latest/)

### Aims
- Fable friendly
- Codec based encoding & decoding
- (TODO) Have code generation similar to [Dart Json Serialization](https://docs.flutter.dev/development/data-and-backend/json)
- Make it easy to write custom encoders & decoders

### Usage 
See `sample/Kitchensink.fs` & `sample/Kitchensink.json.fs`
```fsharp
open Krama

type Payload =
    { Name: string
      Age: int
      Address: Address option
      Sex: Sex
      Pet: Pet option
      Cht: Choice<bool, int> }

// Generated code
type Payload with
    static member make(p: Payload) : JsonNode =
        seq {
            ("Name", JsonNode.make p.Name)
            ("Age", JsonNode.make p.Age)
            ("Address", Encoders.make p.Address)
            ("Sex", Encoders.make p.Sex)
            ("Pet", Encoders.make p.Pet)
            ("Cht", Encoders.make p.Cht)
        }
        |> JsonNode.make

    static member AsPayload(j: JsonNode) : Payload =
            { Name = (j </> "Name").AsString()
              Age = (j </> "Age").AsInt()
              Address = (j?Address).AsAddressOrNone()
              Sex = (j </> "Sex").AsSex()
              Pet = (j </> "Pet").AsPetOrNone()
              Cht = (j </> "Cht").AsCht() }

// How to use 
// Json.serialize takes the encoder function and object
// Json.parse takes the decoder function and json string

let jsonStr = Json.serialize Payload.make payloadRecord
let record = Json.parse Payload.AsPayload jsonStr
```
