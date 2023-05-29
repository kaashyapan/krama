import { serializer, deserializer, serde } from "../src/Program.fs.js";

export function json_parse(s) {
  return JSON.parse(s);
}

export function json_serde(s) {
  let jsobj = JSON.parse(s);
  return JSON.stringify(jsobj);
}

export { serializer, deserializer, serde };
