module Test

open System

type Person =
  | Male
  | Female

type Being =
  | Human of Person
  | Animal of Person
