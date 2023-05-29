module JsonTests.Serializer

open System
open Krama.Json
open Expecto

// Serialization
let tests =
    [
        test "Can serialize document with nothing" {
            let txt = "{}"
            let decoder (j: JsonNode) = j.AsPropertyArrayOrNone()
            let struc = Json.parse decoder txt
            let encoder j = Map.empty |> JsonNode.encode
            let actual = Json.serialize encoder struc
            Expect.equal actual txt ""
        }

        test "Can serialize document with int" {
            let txt = "{\"firstName\":\"John\",\"age\":25}"

            let decoder j = {| firstName = j?firstName.AsString(); age = j?age.AsInt() |}

            let struc = Json.parse decoder txt

            let encoder (st: {| firstName: string; age: int |}) =
                [| ("firstName", JsonNode.encode st.firstName); ("age", JsonNode.encode st.age) |]
                |> JsonNode.encode

            let actual = Json.serialize encoder struc
            Expect.equal actual txt ""
        }
        test "Can serialize document with bool" {
            let txt = "{\"firstName\":\"John\",\"employed\":false}"

            let decoder j = {| firstName = j?firstName.AsString(); employed = j?employed.AsBool() |}

            let struc = Json.parse decoder txt

            let encoder (st: {| firstName: string; employed: bool |}) =
                [| ("firstName", JsonNode.encode st.firstName); ("employed", JsonNode.encode st.employed) |]
                |> JsonNode.encode

            let actual = Json.serialize encoder struc
            Expect.equal actual txt ""
        }
        test "Can serialize document with booleans" {
            let txt = """{"aa":true,"bb":false}""" 
            let decoder j = {| aa = j?aa.AsBool(); bb = j?bb.AsBool() |}
            let struc = Json.parse decoder txt
            let encoder (st: {| aa: bool; bb: bool |}) =
                [| "aa", JsonNode.encode true; "bb", JsonNode.encode false |]
                |> JsonNode.encode

            let actual = Json.serialize encoder struc  
            Expect.equal actual txt "" 
        }
        test "Can serialize document with float" {
            let txt = "{\"firstName\":\"John\",\"weight\":25.25}"

            let decoder j = {| firstName = j?firstName.AsString(); weight = j?weight.AsFloat() |}

            let struc = Json.parse decoder txt

            let encoder (st: {| firstName: string; weight: float |}) =
                [| ("firstName", JsonNode.encode st.firstName); ("weight", JsonNode.encode st.weight) |]
                |> JsonNode.encode

            let actual = Json.serialize encoder struc
            Expect.equal actual txt ""

        }
        test "Can serialize document with iso 8601 date" {
            let txt = "{\"birthDate\":\"2020-05-19T14:39:22.500Z\"}"

            let decoder j = {| birthDate = (j?birthDate).AsDateTimeOffset() |}

            let struc = Json.parse decoder txt

            let encoder (st: {| birthDate: DateTimeOffset |}) =
                [| ("birthDate", JsonNode.encode st.birthDate) |] |> JsonNode.encode

            let actual = Json.serialize encoder struc
            Expect.equal (actual.Substring(0, 37)) (txt.Substring(0, 37)) ""
        }
        test "Can serialize document with timespan" {
            let txt = "{\"lapTime\":\"00:30:00\"}"

            let decoder j = {| lapTime = (j?lapTime).AsTimeSpan() |}

            let struc = Json.parse decoder txt

            let encoder (st: {| lapTime: TimeSpan |}) = [| ("lapTime", JsonNode.encode st.lapTime) |] |> JsonNode.encode

            let actual = Json.serialize encoder struc
            Expect.equal actual txt ""
        }
        test "Can serialize document with guid" {

            let txt = "{\"id\":\"f842213a-82fb-4eeb-ab75-7ccd18676fd5\"}"
            let decoder j = {| id = j?id.AsGuid() |}
            let struc = Json.parse decoder txt

            let encoder (st: {| id: Guid |}) = [| ("id", JsonNode.encode st.id) |] |> JsonNode.encode

            let actual = Json.serialize encoder struc
            Expect.equal actual txt ""
        }
        test "Can serialize document with null roundtrip" {
            let txt = "{\"firstName\":\"John\",\"age\":null}"

            let decoder j = {| firstName = j?firstName.AsString(); age = j?age.AsInt32OrNone() |}

            let struc = Json.parse decoder txt

            let encoder (st: {| firstName: string; age: int32 option |}) =
                [| ("firstName", JsonNode.encode st.firstName); ("age", JsonNode.encode st.age) |]
                |> JsonNode.encode

            let actual = Json.serialize encoder struc
            Expect.equal actual txt ""
        }
    ]
