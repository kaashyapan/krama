module Test.DepUnion

open System

type Sex =
  | Male
  | Female

type Person = { Gender: Sex }
