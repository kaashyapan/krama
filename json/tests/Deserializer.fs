module JsonTests.Deserializer

open System
open Expecto
open Krama.Json
open Krama.Json.Json

// Parsing
let tests =
    [
        test "Can parse empty document" {
            let decoder j = Decode.AsPropertyArray(j)
            let actual = Json.parse decoder "{}"
            let k: seq<string * JsonNode> = Seq.empty
            let node = JsonNode.encode (k)
            Expect.equal actual (k |> Seq.toArray) ""
        }
        test "Can parse document with single property" {
            let decoder (jobj: JsonNode) = {| firstName = Decode.Get(jobj, "firstName") |> Decode.AsString |}

            let actual = Json.parse decoder "{\"firstName\": \"John\"}"
            Expect.equal actual.firstName "John" ""
        }
        test "Can parse document with nested property" {
            let decoder j = (j </?> "name" <??> "first").AsStringOrNone()

            let actual = Json.parse decoder "{\"name\": {\"first\":\"John\"}}"
            Expect.equal actual (Some "John") ""

        }
        test "Can parse document with nested optional property" {
            let decoder j = (j </?> "person" <??> "name" <??> "first").AsStringOrNone()

            let actual = Json.parse decoder "{\"person\":{\"name\": {\"first\":\"John\"}}}"
            Expect.equal actual (Some "John") ""
        }
        test "Can parse document with misspelt nested optional property" {
            let decoder j = (j </?> "person" <??> "name" <??> "first").AsStringOrNone()

            let actual = Json.parse decoder "{\"person\":{\"name1\": {\"first\":\"John\"}}}"
            Expect.equal actual (None) ""
        }
        test "Can parse document with missing nested optional property" {
            let decoder j = (j </?> "person" <??> "name" <??> "last").AsStringOrNone()

            let actual = Json.parse decoder "{\"person\":{\"name\": {\"first\":\"John\"}}}"
            Expect.equal actual (None) ""
        }
        test "Can parse document with int16" {
            let decoder j = j?age.AsInt16()
            let actual = Json.parse decoder "{\"firstName\": \"John\", \"age\": 25}"
            Expect.equal actual (25 |> int16) ""
        }
        test "Can parse document with int" {
            let decoder j = j?age.AsInt()
            let actual = Json.parse decoder "{\"firstName\": \"John\", \"age\": 25}"
            Expect.equal actual 25 ""
        }
        test "Can parse document with int64" {
            let decoder j = j?age.AsInt64()
            let actual = Json.parse decoder "{\"firstName\": \"John\", \"age\": 25}"
            Expect.equal actual (25 |> int64) ""
        }
        test "Can parse document with float" {
            let decoder j = j?age.AsDecimal()
            let actual = Json.parse decoder "{\"firstName\": \"John\", \"age\": 25.25}"
            Expect.equal actual 25.25m ""
        }
        test "Can parse document with decimal" {
            let decoder j = j?age.AsDecimal()
            let actual = Json.parse decoder "{\"firstName\": \"John\", \"age\": 25.25}"
            Expect.equal actual 25.25M ""
        }
        test "Can parse document with iso 8601 date" {
            let decoder j = j?birthDate.AsDateTime()
            let actual = Json.parse decoder "{\"birthDate\": \"2020-05-19T14:39:22.500Z\"}"
            Expect.equal actual (new DateTime(2020, 5, 19, 14, 39, 22, 500)) ""
        }
        test "Can parse document with unix epoch timestamp" {
            let decoder j = j?birthDate.AsDateTime()
            let actual = Json.parse decoder "{\"birthDate\": 1587147118004}"
            Expect.equal actual (new DateTime(2020, 4, 17, 18, 11, 58, 4)) ""
        }
        test "Can parse document with datetimeoffset" {
            let dtOffset = new DateTimeOffset(2020, 4, 17, 18, 11, 58, TimeSpan.FromHours(float -4))

            let decoder j = j?birthDate.AsDateTimeOffset()
            let actual = Json.parse decoder (sprintf "{\"birthDate\": \"%O\"}" dtOffset)
            Expect.equal actual dtOffset ""
        }
        test "Can parse document with timespan" {
            let decoder j = j?lapTime.AsTimeSpan()
            let actual = Json.parse decoder "{\"lapTime\": \"00:30:00\"}"
            Expect.equal actual (new TimeSpan(0, 30, 0)) ""
        }
        test "Can parse document with guid" {
            let decoder j = j?id.AsGuid()

            let actual = Json.parse decoder "{ \"id\": \"{F842213A-82FB-4EEB-AB75-7CCD18676FD5}\" }"

            Expect.equal actual (Guid.Parse "F842213A-82FB-4EEB-AB75-7CCD18676FD5") ""
        }
        // test "Can parse a string from twitter api without throwing an error" {
        //     let txt =
        //         "[{\"in_reply_to_status_id_str\":\"115445959386861568\",\"truncated\":false,\"in_reply_to_user_id_str\":\"40453522\",\"geo\":null,\"retweet_count\":0,\"contributors\":null,\"coordinates\":null,\"user\":{\"default_profile\":false,\"statuses_count\":3638,\"favourites_count\":28,\"protected\":false,\"profile_text_color\":\"634047\",\"profile_image_url\":\"http:\\/\\/a3.twimg.com\\/profile_images\\/1280550984\\/buddy_lueneburg_normal.jpg\",\"name\":\"Steffen Forkmann\",\"profile_sidebar_fill_color\":\"E3E2DE\",\"listed_count\":46,\"following\":true,\"profile_background_tile\":false,\"utc_offset\":3600,\"description\":\"C#, F# and Dynamics NAV developer, blogger and sometimes speaker. Creator of FAKE - F# encode and NaturalSpec.\",\"location\":\"Hamburg \\/ Germany\",\"contributors_enabled\":false,\"verified\":false,\"profile_link_color\":\"088253\",\"followers_count\":471,\"url\":\"http:\\/\\/www.navision-blog.de\\/blog-mitglieder\\/steffen-forkmann-ueber-mich\\/\",\"profile_sidebar_border_color\":\"D3D2CF\",\"screen_name\":\"sforkmann\",\"default_profile_image\":false,\"notifications\":false,\"show_all_inline_media\":false,\"geo_enabled\":true,\"profile_use_background_image\":true,\"friends_count\":373,\"id_str\":\"22477880\",\"is_translator\":false,\"lang\":\"en\",\"time_zone\":\"Berlin\",\"created_at\":\"Mon Mar 02 12:04:39 +0000 2009\",\"profile_background_color\":\"EDECE9\",\"id\":22477880,\"follow_request_sent\":false,\"profile_background_image_url_https\":\"https:\\/\\/si0.twimg.com\\/images\\/themes\\/theme3\\/bg.gif\",\"profile_background_image_url\":\"http:\\/\\/a1.twimg.com\\/images\\/themes\\/theme3\\/bg.gif\",\"profile_image_url_https\":\"https:\\/\\/si0.twimg.com\\/profile_images\\/1280550984\\/buddy_lueneburg_normal.jpg\"},\"favorited\":false,\"in_reply_to_screen_name\":\"ovatsus\",\"source\":\"\\u003Ca href=\\\"http:\\/\\/www.tweetdeck.com\\\" rel=\\\"nofollow\\\"\\u003ETweetDeck\\u003C\\/a\\u003E\",\"id_str\":\"115447331628916736\",\"in_reply_to_status_id\":115445959386861568,\"id\":115447331628916736,\"created_at\":\"Sun Sep 18 15:29:23 +0000 2011\",\"place\":null,\"retweeted\":false,\"in_reply_to_user_id\":40453522,\"text\":\"@ovatsus I know it's not complete. But I don't want to add a dependency on FParsec in #FSharp.Data. Can you send me samples where it fails?\"},{\"in_reply_to_status_id_str\":null,\"truncated\":false,\"in_reply_to_user_id_str\":null,\"geo\":null,\"retweet_count\":0,\"contributors\":null,\"coordinates\":null,\"user\":{\"statuses_count\":3637,\"favourites_count\":28,\"protected\":false,\"profile_text_color\":\"634047\",\"profile_image_url\":\"http:\\/\\/a3.twimg.com\\/profile_images\\/1280550984\\/buddy_lueneburg_normal.jpg\",\"name\":\"Steffen Forkmann\",\"profile_sidebar_fill_color\":\"E3E2DE\",\"listed_count\":46,\"following\":true,\"profile_background_tile\":false,\"utc_offset\":3600,\"description\":\"C#, F# and Dynamics NAV developer, blogger and sometimes speaker. Creator of FAKE - F# encode and NaturalSpec.\",\"location\":\"Hamburg \\/ Germany\",\"contributors_enabled\":false,\"verified\":false,\"profile_link_color\":\"088253\",\"followers_count\":471,\"url\":\"http:\\/\\/www.navision-blog.de\\/blog-mitglieder\\/steffen-forkmann-ueber-mich\\/\",\"profile_sidebar_border_color\":\"D3D2CF\",\"screen_name\":\"sforkmann\",\"default_profile_image\":false,\"notifications\":false,\"show_all_inline_media\":false,\"geo_enabled\":true,\"profile_use_background_image\":true,\"friends_count\":372,\"id_str\":\"22477880\",\"is_translator\":false,\"lang\":\"en\",\"time_zone\":\"Berlin\",\"created_at\":\"Mon Mar 02 12:04:39 +0000 2009\",\"profile_background_color\":\"EDECE9\",\"id\":22477880,\"default_profile\":false,\"follow_request_sent\":false,\"profile_background_image_url_https\":\"https:\\/\\/si0.twimg.com\\/images\\/themes\\/theme3\\/bg.gif\",\"profile_background_image_url\":\"http:\\/\\/a1.twimg.com\\/images\\/themes\\/theme3\\/bg.gif\",\"profile_image_url_https\":\"https:\\/\\/si0.twimg.com\\/profile_images\\/1280550984\\/buddy_lueneburg_normal.jpg\"},\"favorited\":false,\"in_reply_to_screen_name\":null,\"source\":\"\\u003Ca href=\\\"http:\\/\\/www.tweetdeck.com\\\" rel=\\\"nofollow\\\"\\u003ETweetDeck\\u003C\\/a\\u003E\",\"id_str\":\"115444490331889664\",\"in_reply_to_status_id\":null,\"id\":115444490331889664,\"created_at\":\"Sun Sep 18 15:18:06 +0000 2011\",\"possibly_sensitive\":false,\"place\":null,\"retweeted\":false,\"in_reply_to_user_id\":null,\"text\":\"Added a simple Json parser to #FSharp.Data http:\\/\\/t.co\\/3JGI56SM - #fsharp\"}]"
        //
        //     Json.parse txt |> ignore
        //     Expect.equal 1 1 ""
        // }
        test "Can convert missing optional string field to None" {
            let decoder j = (j </?> "name").AsStringOrNone()
            let actual = Json.parse decoder "{ \"firstName\": \"Don\", \"lastName\": \"Syme\" }"
            Expect.equal actual (None) ""

        }
        test "Can convert optional string field to Some" {
            let decoder j = j?firstName.AsStringOrNone()
            let actual = Json.parse decoder "{ \"firstName\": \"Don\", \"lastName\": \"Syme\" }"
            Expect.equal actual (Some "Don") ""
        }
        test "Can convert missing optional int16 field to None" {
            let decoder j = (j </?> "age").AsInt16OrNone()
            let actual = Json.parse decoder "{ \"firstName\": \"Don\", \"lastName\": \"Syme\" }"
            Expect.equal actual (None) ""

        }
        test "Can convert optional int16 field to Some" {
            let decoder j = j?age.AsInt16OrNone()

            let actual = Json.parse decoder "{ \"firstName\": \"Don\", \"lastName\": \"Syme\", \"age\": 45 }"

            Expect.equal actual (Some(int16 45)) ""

        }
        test "Can convert missing optional int32 field to None" {
            let decoder j = (j </?> "age").AsInt32OrNone()
            let actual = Json.parse decoder "{ \"firstName\": \"Don\", \"lastName\": \"Syme\" }"
            Expect.equal actual (None) ""

        }
        test "Can convert optional int32 field to Some" {
            let decoder j = (j </?> "age").AsInt32OrNone()

            let actual = Json.parse decoder "{ \"firstName\": \"Don\", \"lastName\": \"Syme\", \"age\": 45 }"

            Expect.equal actual (Some(int32 45)) ""

        }
        test "Can convert missing optional int64 field to None" {
            let decoder j = (j </?> "age").AsInt64OrNone()
            let actual = Json.parse decoder "{ \"firstName\": \"Don\", \"lastName\": \"Syme\" }"
            Expect.equal actual (None) ""

        }
        test "Can convert optional int64 field to Some" {
            let decoder j = (j </?> "age").AsInt64OrNone()

            let actual = Json.parse decoder "{ \"firstName\": \"Don\", \"lastName\": \"Syme\", \"age\": 45 }"

            Expect.equal actual (Some(int64 45)) ""

        }
        test "Can convert missing optional int field to None" {
            let decoder j = (j </?> "age").AsIntOrNone()
            let actual = Json.parse decoder "{ \"firstName\": \"Don\", \"lastName\": \"Syme\" }"
            Expect.equal actual (None) ""

        }
        test "Can convert optional int field to Some" {
            let decoder j = j?age.AsIntOrNone()

            let actual = Json.parse decoder "{ \"firstName\": \"Don\", \"lastName\": \"Syme\", \"age\": 45 }"

            Expect.equal actual (Some(int 45)) ""

        }
        test "Can convert missing optional float field to None" {
            let decoder j = (j </?> "height").AsFloatOrNone()
            let actual = Json.parse decoder "{ \"firstName\": \"Don\", \"lastName\": \"Syme\" }"
            Expect.equal actual (None) ""

        }
        test "Can convert optional float field to Some" {
            let decoder j = (j </> "height").AsFloatOrNone()

            let actual = Json.parse decoder "{ \"firstName\": \"Don\", \"lastName\": \"Syme\", \"height\": 5.8 }"

            Expect.equal actual (Some(float 5.8m)) ""

        }
        test "Can convert missing optional decimal field to None" {
            let decoder j = (j </?> "height").AsDecimalOrNone()
            let actual = Json.parse decoder "{ \"firstName\": \"Don\", \"lastName\": \"Syme\" }"
            Expect.equal actual (None) ""
        }
        test "Can convert optional decimal field to Some" {
            let decoder j = j?height.AsDecimalOrNone()

            let actual = Json.parse decoder "{ \"firstName\": \"Don\", \"lastName\": \"Syme\", \"height\": 5.8 }"

            Expect.equal actual (Some(decimal 5.8M)) ""

        }
        test "Can convert missing optional boolean field to None" {
            let decoder j = (j </?> "isMale").AsBoolOrNone()
            let actual = Json.parse decoder "{ \"firstName\": \"Don\", \"lastName\": \"Syme\" }"

            Expect.equal actual (None) ""

        }
        test "Can convert optional boolean field to Some" {
            let decoder j = (j </?> "isMale").AsBoolOrNone()

            let actual = Json.parse decoder "{ \"firstName\": \"Don\", \"lastName\": \"Syme\", \"isMale\": \"true\" }"

            Expect.equal actual (Some true) ""

        }
        test "Can convert optional iso 8601 date to None" {
            let decoder j = (j </?> "birthDate1").AsDateTimeOrNone()

            let actual = Json.parse decoder "{\"birthDate\": \"2020-05-19T14:39:22.500Z\"}"

            Expect.equal actual (None) ""

        }
        test "Can convert optional iso 8601 date to Some" {
            let decoder j = (j </?> "birthDate").AsDateTimeOrNone()
            let actual = Json.parse decoder "{\"birthDate\": \"2020-05-19T14:39:22.500Z\"}"

            Expect.equal actual (new DateTime(2020, 5, 19, 14, 39, 22, 500) |> Some) ""

        }
        test "Can convert optional unix epoch timestamp to None" {
            let decoder j = (j </?> "birthDate1").AsDateTimeOrNone()

            let actual = Json.parse decoder "{\"birthDate\": 1587147118004}"

            Expect.equal actual (None) ""

        }
        test "Can convert optional unix epoch timestamp to Some" {
            let decoder j = (j </> "birthDate").AsDateTimeOrNone()
            let actual = Json.parse decoder "{\"birthDate\": 1587147118004}"

            Expect.equal actual (new DateTime(2020, 4, 17, 18, 11, 58, 4) |> Some) ""

        }
        test "Can convert optional datetimeoffset to None" {
            let dtOffset = new DateTimeOffset(2020, 4, 17, 18, 11, 58, TimeSpan.FromHours(float -4))

            let decoder j = (j </?> "birthDate1").AsDateTimeOffsetOrNone()

            let actual = Json.parse decoder (sprintf "{\"birthDate\": \"%O\"}" dtOffset)

            Expect.equal actual (None) ""

        }
        test "Can convert optional datetimeoffset to Some" {
            let dtOffset = new DateTimeOffset(2020, 4, 17, 18, 11, 58, TimeSpan.FromHours(float -4))

            let decoder j = (j </?> "birthDate").AsDateTimeOffsetOrNone()

            let actual = Json.parse decoder (sprintf "{\"birthDate\": \"%O\"}" dtOffset)

            Expect.equal actual (dtOffset |> Some) ""

        }
        test "Can convert optional timespan to None" {
            let decoder j = (j </?> "lapTime1").AsTimeSpanOrNone()
            let actual = Json.parse decoder "{\"lapTime\": \"00:30:00\"}"

            Expect.equal actual (None) ""

        }
        test "Can convert optional timespan to Some" {
            let decoder j = (j </?> "lapTime").AsTimeSpanOrNone()
            let actual = Json.parse decoder "{\"lapTime\": \"00:30:00\"}"

            Expect.equal actual (new TimeSpan(0, 30, 0) |> Some) ""

        }
        test "Can convert optional guid to None" {
            let decoder j = (j </?> "id1").AsGuidOrNone()

            let actual = Json.parse decoder "{ \"id\": \"{F842213A-82FB-4EEB-AB75-7CCD18676FD5}\" }"

            Expect.equal actual (None) ""

        }
        test "Can convert optional guid to Some" {
            let decoder j = (j </?> "id").AsGuidOrNone()

            let actual = Json.parse decoder "{ \"id\": \"{F842213A-82FB-4EEB-AB75-7CCD18676FD5}\" }"

            Expect.equal actual (Guid.Parse "F842213A-82FB-4EEB-AB75-7CCD18676FD5" |> Some) ""

        }
        test "Can parse array of numbers" {
            let decoder j = Decode.AsArray(j) |> Array.map (fun x -> Decode.AsInt(x))

            let actual = Json.parse decoder "[1, 2, 3]"
            Expect.equal actual [| 1; 2; 3 |] ""
        }
        test "Quotes in strings are properly escaped" {
            let txt = "{\"short_description\":\"This a string with \\\"quotes\\\"\"}"
            let decoder j = (j </> "short_description").AsString()
            let actual = Json.parse decoder txt
            let expected = "This a string with \"quotes\""
            Expect.equal actual expected ""
        }
        test "Can convert optional array to None" {
            let decoder j = (j </?> "nos1").AsArrayOrNone()
            let actual = Json.parse decoder "{ \"nos\": [1, 2, 3] }"

            Expect.equal actual (None) ""
        }
        test "Can convert optional array to Some" {
            let decoder j = Option.get((j </?> "nos").AsArrayOrNone()).Length

            let actual = Json.parse decoder "{ \"nos\": [1, 2, 3] }"
            Expect.equal actual (3) ""
        }
        test "Can convert optional obj to None" {
            let decoder j = (j </?> "name1").AsArrayOrNone()
            let actual = Json.parse decoder "{ \"name\": { \"firstName\": \"Don Syme\" } }"

            Expect.equal actual (None) ""

        }
        test "Can convert optional obj to Some" {
            let decoder j = Option.get((j </?> "name").AsPropertyArrayOrNone()).Length

            let actual = Json.parse decoder "{ \"name\": { \"firstName\": \"Don Syme\" } }"

            Expect.equal actual (1) ""

        }
        test "Can deserialize document with null to object" {
            let txt = "{\"firstName\":\"John\"}"
            let expectedResult = {| firstName = "John"; age = None |}

            let decoder j = {| firstName = (j </> "firstName").AsString(); age = (j </?> "age").AsInt16OrNone() |}

            let actual = Json.parse decoder txt

            Expect.equal actual expectedResult ""

        }
        test "Can deserialize document with explicit nullable string to None" {
            let txt = "{\"firstName\":null}"

            let decoder j = {| firstName = (j </?> "firstName").AsStringOrNone() |}

            let actual = Json.parse decoder txt
            let expectedResult = {| firstName = None |}

            Expect.equal actual expectedResult ""

        }
        test "Can deserialize document with explicit nullable int to None" {
            let txt = "{\"age\":null}"
            let decoder j = {| age = j?age.AsInt32OrNone() |}
            let actual = Json.parse decoder txt
            let expectedResult = {| age = None |}
            Expect.equal actual expectedResult ""

        }
        test "Can deserialize document with explicit nullable int64 to None" {
            let txt = "{\"age\":null}"
            let decoder j = {| age = j?age.AsInt64OrNone() |}
            let actual = Json.parse decoder txt
            let expectedResult = {| age = None |}
            Expect.equal actual expectedResult ""

        }
        test "Can deserialize document with explicit nullable int16 to None" {
            let txt = "{\"age\":null}"
            let decoder j = {| age = j?age.AsInt16OrNone() |}
            let actual = Json.parse decoder txt
            let expectedResult = {| age = None |}
            Expect.equal actual expectedResult ""

        }
        test "Can deserialize document with explicit nullable float to None" {
            let txt = "{\"age\":null}"
            let decoder j = {| age = j?age.AsFloatOrNone() |}
            let actual = Json.parse decoder txt
            let expectedResult = {| age = None |}
            Expect.equal actual expectedResult ""

        }
        test "Can deserialize document with explicit nullable decimal to None" {
            let txt = "{\"age\":null}"
            let decoder j = {| age = j?age.AsDecimalOrNone() |}
            let actual = Json.parse decoder txt
            let expectedResult = {| age = None |}
            Expect.equal actual expectedResult ""

        }
        test "Can deserialize document with explicit nullable array to None" {
            let txt = "{\"age\":null}"
            let decoder j = {| age = j?age.AsArrayOrNone() |}
            let actual = Json.parse decoder txt
            let expectedResult = {| age = None |}
            Expect.equal actual expectedResult ""

        }
        test "Can deserialize document with explicit nullable object to None" {
            let txt = "{\"age\":null}"

            let decoder j = {| age = j?age.AsPropertyArrayOrNone() |}

            let actual = Json.parse decoder txt
            let expectedResult = {| age = None |}
            Expect.equal actual expectedResult ""
        }
    ]
