module Test.DepRecord

open System

type Var1 = { Name: string }

type Var2 = { Person: Var1 }
