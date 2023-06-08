module Model

open System
open Types

type Person =
    {
        Name: string
        Age: Age
        Address: Address option
        Sex: Sex
        Pet: Pet option
        Cht: Cht
    }

type Employee =
    {
        Name: string
        Age: Age
        Address: Address option
    }

type Department =
    {
        Name: string
        Manager: string
        Employee: int 
    }


