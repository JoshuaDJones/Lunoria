import { fileURLToPath, URL } from "node:url";
import { defineConfig } from "vite";
import react from "@vitejs/plugin-react-swc";

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  base: "/",
  server: {
    host: "0.0.0.0",
    port: 5173,
    strictPort: true,
    // Keep browser requests on the frontend origin when switching networks.
    proxy: {
      "/api": {
        target: "http://127.0.0.1:5243",
        changeOrigin: true,
      },
      "/hubs": {
        target: "http://127.0.0.1:5243",
        changeOrigin: true,
        ws: true,
      },
    },
  },
  build: {
    outDir: "dist",
    emptyOutDir: true,
  },
});
