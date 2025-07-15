import { defineConfig } from 'vite';

export default defineConfig({
  base: '/ts/',
  build: {
    outDir: '../wwwroot/ts',
    emptyOutDir: true,
    manifest: true,
  }
});