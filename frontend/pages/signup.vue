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
        Create an Account
      </h1>
      <p class="text-center text-gray-500 dark:text-gray-400 mb-8">
        Sign up to start using our service
      </p>

      <UForm :state="state" class="space-y-6" @submit.prevent="signUp">
        <UFormField label="Email" name="email">
          <UInput
            v-model="state.email"
            size="lg"
            placeholder="Enter your email"
            class="w-full"
          />
        </UFormField>

        <UFormField label="Full Name" name="fullName">
          <UInput
            v-model="state.fullName"
            size="lg"
            placeholder="Enter your full name"
            class="w-full"
          />
        </UFormField>

        <UFormField label="Password" name="password">
          <UInput
            v-model="state.password"
            type="password"
            size="lg"
            placeholder="Create a password"
            class="w-full"
          />
        </UFormField>

        <UButton type="submit" size="lg" class="w-full justify-center"
          >Sign Up</UButton
        >
      </UForm>

      <div class="text-center mt-6 text-sm text-gray-500 dark:text-gray-400">
        Already have an account?
        <nuxt-link
          to="/login"
          class="text-primary-600 dark:text-primary-400 hover:underline"
          >Login</nuxt-link
        >
      </div>
    </div>
  </div>
</template>

<script setup>
const state = ref({
  email: '',
  fullName: '',
  password: ''
})

const toast = useToast()
const router = useRouter()

const signUp = async () => {
  await api('User/signup', 'POST', state.value, false)
    .then(() => {
      toast.add({ title: 'Signup Successful, Redirecting to login' })
      router.push('/login')
    })
    .catch((err) => {
      console.log(err)
    })
}

const token = useCookie('token')
if (token.value) {
  router.push('/')
}
</script>
