import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import Pages from 'vite-plugin-pages';

// https://vitejs.dev/config/
export default defineConfig({
  define: {
    'process.env': process.env
  },
  plugins: [
    vue(),
    Pages({
      dirs: 'src/pages', // pages 디렉토리 지정
    }),

  ],
  build: {
    commonjsOptions: {
      transformMixedEsModules: true,
    }
  }
})
