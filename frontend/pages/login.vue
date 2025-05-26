<template>
  <div
    class="w-full min-h-screen flex items-center justify-center bg-gray-100 dark:bg-gray-900 p-4"
  >
    <div
      class="w-full max-w-md bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-700 p-8 rounded-2xl shadow-lg"
    >
      <h1
        class="text-3xl font-bold text-center text-gray-800 dark:text-white mb-4"
      >
        Welcome Back
      </h1>
      <p class="text-center text-gray-500 dark:text-gray-400 mb-8">
        Login to your account to continue
      </p>

      <UForm
        :validate="validate"
        :state="state"
        class="space-y-6"
        @submit="login"
      >
        <UFormField label="Email" name="email">
          <UInput
            v-model="state.email"
            size="lg"
            placeholder="Enter your email"
            class="w-full"
          />
        </UFormField>

        <UFormField label="Password" name="password">
          <div class="relative">
            <UInput
              v-model="state.password"
              :type="showPassword ? 'text' : 'password'"
              size="lg"
              placeholder="Enter your password"
              class="w-full pr-10"
            />
            <button
              type="button"
              class="absolute inset-y-0 right-0 flex items-center pr-3 text-gray-400 hover:text-gray-600 dark:text-gray-500 dark:hover:text-gray-300"
              @click="showPassword = !showPassword"
            >
              <UIcon
                :name="
                  showPassword ? 'i-heroicons-eye-slash' : 'i-heroicons-eye'
                "
                class="h-5 w-5"
              />
            </button>
          </div> </UFormField
        ><UButton type="submit" size="lg" class="w-full justify-center"
          >Login</UButton
        >
      </UForm>

      <div class="text-center mt-6 text-sm text-gray-500 dark:text-gray-400">
        Don't have an account?
        <nuxt-link
          to="/signup"
          class="text-primary-600 dark:text-primary-400 hover:underline"
          >Sign up</nuxt-link
        >
      </div>
    </div>
  </div>
</template>
<script setup>
import { jwtDecode } from 'jwt-decode'

const state = ref({
  email: '',
  password: ''
})

const showPassword = ref(false)
const router = useRouter()

const validate = (state) => {
  const errors = []
  if (!state.email) errors.push({ name: 'email', message: 'Required' })
  if (!state.password) errors.push({ name: 'password', message: 'Required' })
  return errors
}

const login = async () => {
  const token = useCookie('byToken', {
    maxAge: 60 * 60 * 24 * 30,
    sameSite: 'lax',
    secure: true,
    path: '/'
  })

  const refreshToken = useCookie('byRefreshToken', {
    maxAge: 60 * 60 * 24 * 30,
    sameSite: 'lax',
    secure: true,
    path: '/'
  })

  const exp = useCookie('exp', {
    maxAge: 60 * 60 * 24 * 30,
    sameSite: 'lax',
    secure: true,
    path: '/'
  })

  const userId = useCookie('userId', {
    maxAge: 60 * 60 * 24 * 30,
    sameSite: 'lax',
    secure: true,
    path: '/'
  })

  const isAdmin = useCookie('isAdmin', {
    maxAge: 60 * 60 * 24 * 30,
    sameSite: 'lax',
    secure: true,
    path: '/'
  })
  await api('User/login', 'POST', state.value, false).then((data) => {
    token.value = data.token
    refreshToken.value = data.refreshToken

    const decoded = jwtDecode(data.token)
    exp.value = decoded.exp
    userId.value = decoded.nameid
    isAdmin.value = decoded.IsAdmin
    router.push('/dashboard')
  })
}

const token = useCookie('byToken')

if (token.value) {
  router.push('/dashboard')
}
useHead({
  title: 'Login'
})
</script>
