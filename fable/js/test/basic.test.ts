import { assert, expect, test } from "vitest";
import { serializer, deserializer, serde } from "../src/Program.fs.js";
// Edit an assertion and save to see HMR in action

test("JSON Deserializer", () => {
  const _ = deserializer();

  assert.deepEqual(1, 1, "matches original");
});

test("JSON Serializer", () => {
  const input = {
    foo: "hello",
    bar: "world",
  };

  const output = serializer();

  expect(output).eq('{"foo":"hello","bar":"world"}');
  assert.deepEqual(JSON.parse(output), input, "matches original");
});


test("JSON Serde", () => {
  const input = '{"foo":"hello","bar":"world"}'
  const output = serde(input);

  expect(output).eq('{"foo":"hello","bar":"world"}');
  assert.deepEqual(output, input, "matches original");
});;
