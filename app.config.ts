import { defineConfig } from "@solidjs/start/config";

export default defineConfig({
    ssr: false,
    server: {
        preset: "static",
        output: {
            dir: ".",
            publicDir: "www",
            serverDir: "",
        },
    },
    vite: {
        css: {
            preprocessorOptions: {
              scss: {
                api: 'modern-compiler' // or "modern"
              }
            }
        }
    }
});