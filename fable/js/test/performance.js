// Import the ../ module
import BenchTable from "benchtable";
//import { deserializer, serde } from "../src/main.js";
import {
  deserializer,
  serde,
  json_parse,
  json_serde,
} from "../dist/krama.es.js";

// Create benchtable suite
var suite = new BenchTable();

suite
  // Add functions for benchmarking
  .addFunction("Json parse", (s) => json_parse(s))
  .addFunction("Krama Json deserializer", (s) => deserializer())
  .addFunction("Json parse & stringify", (s) => json_serde(s))
  .addFunction("Krama Json ser & deser", (s) => serde(s))
  // Add inputs
  .addInput("Short string", ['{"foo":"hello","bar":"world"}'])
  // Add listeners
  .on("cycle", (event) => {
    console.log(event.target.toString());
  })
  .on("complete", () => {
    console.log("Fastest is " + suite.filter("fastest").map("name"));
    console.log(suite.table.toString());
  });
// Run async

suite.run({ async: false });
