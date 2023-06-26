module Kitchensink

open System

type Address = { Street: string; State': string }

type Age = int

type Pet =
    | Cat
    | Dog

type Sex =
    | Male of {| name: string ; age: int |}
    | Female

type Cht = Choice<bool, int>

type ChtResult = Result<bool, int>

type Payload =
    {
        Name: string
        Age: Age
        Address: Address option
        Sex: Sex
        Pet: Pet option
        Cht: Cht
        Books: string list
        Bills: string * string * int
    }

let testPayload =
    {
        Name = "Elon Musk"
        Age = 50
        Address = Some { Street = "San Fransisco"; State' = "California" }
        Sex = Female
        Pet = Some Dog
        Cht = Choice2Of2 32
        Books = [ "Issa"; "Basho" ]
        Bills = ("Water", "Feb", 05)
    }
