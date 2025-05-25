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
      title: 'Bytely - UBT Computer Science URL Shortener',
      htmlAttrs: {
        lang: 'en'
      },
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' },
        {
          name: 'description',
          content:
            'Academic URL shortener project developed by UBT Computer Science students in Kosovo. Create and analyze short links with our platform.'
        },
        {
          name: 'keywords',
          content:
            'UBT Kosovo, URL shortener, academic project, Genc Vlahiu, Computer Science, link shortener, Bytely, University for Business and Technology, UBT URL shortener'
        },
        { name: 'author', content: 'Genc Vllahiu' },
        { name: 'robots', content: 'index, follow' }
      ],
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/logo.png' },
        { rel: 'canonical', href: 'https://bytely.xyz' }
      ],
      script: [
        {
          type: 'application/ld+json',
          innerHTML: JSON.stringify({
            '@context': 'https://schema.org',
            '@type': 'CreativeWork',
            name: 'Bytely URL Shortener',
            description: 'Academic project',
            creator: {
              '@type': 'Person',
              name: 'Genc Vllahiu',
              affiliation: {
                '@type': 'EducationalOrganization',
                name: 'UBT - University for Business and Technology',
                location: {
                  '@type': 'Place',
                  address: {
                    '@type': 'PostalAddress',
                    addressCountry: 'XK'
                  }
                }
              }
            }
          })
        }
      ]
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
