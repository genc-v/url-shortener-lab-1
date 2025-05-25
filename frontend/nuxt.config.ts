// nuxt.config.ts
export default defineNuxtConfig({
  compatibilityDate: '2024-11-01',
  nitro: {
    compressPublicAssets: {
      gzip: true,
      brotli: true
    }
  },
  devtools: { enabled: false },
  runtimeConfig: {
    public: {
      API: process.env.API
    }
  },
  modules: ['@nuxt/ui', '@nuxtjs/google-fonts'],
  css: ['~/assets/css/main.css'],
  postcss: {
    plugins: {
      '@tailwindcss/postcss': {},
      autoprefixer: {}
    }
  },
  app: {
    head: {
      title: 'Bytely',
      htmlAttrs: {
        lang: 'en'
      }
    }
  },
  webpack: {
    extractCSS: true,
    optimization: {
      splitChunks: {
        cacheGroups: {
          styles: {
            name: 'styles',
            test: /\.(scss|vue)$/,
            chunks: 'all',
            enforce: true
          }
        }
      }
    }
  }
})
