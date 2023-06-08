module Kitchensink

open System

type Address = { Street: string; State': string }

type Age = int

type Pet =
    | Cat
    | Dog

type Sex =
    | Male
    | Female

type Cht = Choice<bool, int>

type ChtResult = Result<bool, int>

type Payload = 
    { Name: string; Age: Age; Address: Address option; Sex: Sex; Pet: Pet option; Cht: Cht }

let testPayload =
    {
        Name = "Elon Musk"
        Age = 50
        Address = Some { Street = "San Fransisco"; State' = "California" }
        Sex = Male
        Pet = Some Dog
        Cht = Choice2Of2 32
    }
