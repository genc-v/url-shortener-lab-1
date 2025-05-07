import globals from 'globals'
import pluginJs from '@eslint/js'
import pluginVue from 'eslint-plugin-vue'
import prettierConfig from 'eslint-config-prettier'

export default [
  {
    files: ['**/*.{js,mjs,cjs,vue}'],
    languageOptions: {
      globals: {
        ...globals.browser,
        ...globals.node,
        onMounted: 'readonly',
        onUnmounted: 'readonly',
        useAsyncQuery: 'readonly',
        definePageMeta: 'readonly',
        useFetch: 'readonly',
        useState: 'readonly',
        useRouter: 'readonly',
        createError: 'readonly',
        useRoute: 'readonly',
        ref: 'readonly',
        computed: 'readonly',
        watch: 'readonly',
        clearError: 'readonly',
        reactive: 'readonly',
        useApolloFetcher: 'readonly',
        nextTick: 'readonly',
        useScript: 'readonly',
        useHead: 'readonly',
        useRuntimeConfig: 'readonly',
        pageviewEvent: 'readonly',
        onBeforeUnmount: 'readonly',
        articleViewEvent: 'readonly',
        defineNuxtRouteMiddleware: 'readonly'
      },
      parserOptions: {
        ecmaVersion: 'latest',
        sourceType: 'module'
      }
    },
    rules: {
      'no-console': ['error', { allow: ['error'] }],
      'no-alert': 'error',
      'no-undef': ['error', { typeof: true }],
      'no-unused-vars': [
        'warn',
        {
          varsIgnorePattern: '^_',
          argsIgnorePattern: '^_'
        }
      ],
      'no-shadow': 'error',
      'prefer-const': 'warn',
      'arrow-body-style': ['warn', 'as-needed'],
      'vue/no-unused-vars': 'error',
      'vue/no-mutating-props': 'error',
      'vue/require-default-prop': 'warn',
      'vue/multi-word-component-names': 'off',
      'vue/component-tags-order': [
        'error',
        { order: ['template', 'script', 'style'] }
      ],
      'vue/html-self-closing': [
        'error',
        { html: { void: 'always', normal: 'always', component: 'always' } }
      ]
    }
  },
  pluginJs.configs.recommended,
  ...pluginVue.configs['flat/essential'],
  prettierConfig,
  {
    files: ['pages/**/*.vue'],
    rules: {
      'vue/multi-word-component-names': 'off'
    }
  },
  {
    files: ['*.vue'],
    rules: {
      'vue/multi-word-component-names': 'off'
    }
  }
]
