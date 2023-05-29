namespace Krama.Json

open System.Collections
open System.Collections.Generic

[<AutoOpen>]
[<RequireQualifiedAccess>]
module Exception =

    let context (msg: string * JsonNode option) =
        let mutable x = Dictionary<string, int>()

        match (snd msg) with
        | Some n ->
            match n.Position with
            | Some pos -> x.Add("Position", pos)
            | None -> x.Clear()
        | None -> x.Clear()

        x

    exception JsonParserException of msg: string with
        override this.Message = this.msg

    exception PropertyNotFoundException of msg: (string * JsonNode option) with
        override this.Message = this.msg |> fst
        override this.Data = context this.msg

    exception InvalidPropertyTypeException of msg: (string * JsonNode option) with
        override this.Message = this.msg |> fst
        override this.Data = context this.msg

    exception InvalidCharacterException of msg: string with
        override this.Message = this.msg
