import { defineConfig } from 'vite';

export default defineConfig({
  base: '/js/',
  build: {
    outDir: '../wwwroot/js',
    emptyOutDir: true,
    manifest: true,
  }
});