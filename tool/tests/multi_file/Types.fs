module Types

open System

type Address = { Street: string; State': string }

type Age = {| name: string |} list

type Pet =
    | Cat
    | Dog

type Sex =
    | Male of string
    | Female

type Cht = Choice<bool, int>

type ChtResult = Result<bool, int>

