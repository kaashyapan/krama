import * as path from "path";
import { defineConfig } from "vite";

const config = defineConfig({
  build: {
    lib: {
      entry: path.resolve(__dirname, "src/main.js"),
      name: "Krama",
      fileName: (format) => `krama.${format}.js`,
    },
    rollupOptions: {},
  },
});

export default config;
