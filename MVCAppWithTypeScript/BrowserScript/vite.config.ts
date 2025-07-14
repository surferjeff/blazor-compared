import { defineConfig } from 'vite';

export default defineConfig({
  base: '/dist/',
  build: {
    outDir: '../wwwroot/dist',
    emptyOutDir: true,
    manifest: true,
  }
});