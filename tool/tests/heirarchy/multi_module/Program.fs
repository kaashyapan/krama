namespace Test

open System

module Mod1 =

  type Name = string
  type Persons = Name list

type Model = { allpersons: Mod1.Persons option }
