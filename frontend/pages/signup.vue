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
        <UFormField label="Email" name="email" :rules="validateEmail">
          <UInput
            v-model="state.email"
            size="lg"
            placeholder="Enter your email"
            class="w-full"
          />
          <template #error="{ error }">
            <span class="text-red-500 text-sm">{{ error }}</span>
          </template>
        </UFormField>

        <UFormField label="Full Name" name="fullName" :rules="validateFullName">
          <UInput
            v-model="state.fullName"
            size="lg"
            placeholder="Enter your full name"
            class="w-full"
          />
          <template #error="{ error }">
            <span class="text-red-500 text-sm">{{ error }}</span>
          </template>
        </UFormField>

        <UFormField label="Password" name="password" :rules="validatePassword">
          <UInput
            v-model="state.password"
            type="password"
            size="lg"
            placeholder="Create a password"
            class="w-full"
          />
          <template #error="{ error }">
            <span class="text-red-500 text-sm">{{ error }}</span>
          </template>

          <div class="mt-3 space-y-2">
            <div
              v-for="(req, index) in passwordRequirements"
              :key="index"
              class="flex items-center gap-2 transition-colors"
            >
              <UIcon
                :name="
                  req.isMet
                    ? 'i-heroicons-check-circle-20-solid'
                    : 'i-heroicons-x-circle-20-solid'
                "
                :class="[
                  req.isMet
                    ? 'text-green-500 dark:text-green-400'
                    : 'text-red-500 dark:text-red-400',
                  'w-4 h-4 shrink-0'
                ]"
              />
              <span
                :class="[
                  'text-sm',
                  req.isMet
                    ? 'text-green-600 dark:text-green-300'
                    : 'text-gray-500 dark:text-gray-400'
                ]"
              >
                {{ req.label }}
              </span>
            </div>
          </div>
        </UFormField>

        <UButton
          type="submit"
          size="lg"
          class="w-full cursor-pointer justify-center"
          :disabled="!formIsValid"
        >
          Sign Up
        </UButton>
      </UForm>

      <div class="text-center mt-6 text-sm text-gray-500 dark:text-gray-400">
        Already have an account?
        <nuxt-link
          to="/login"
          class="text-primary-600 dark:text-primary-400 hover:underline"
        >
          Login
        </nuxt-link>
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

const passwordRequirements = computed(() => [
  {
    label: 'At least 8 characters',
    isMet: state.value.password.length >= 8
  },
  {
    label: 'Contains at least one uppercase letter',
    isMet: /[A-Z]/.test(state.value.password)
  },
  {
    label: 'Contains at least one lowercase letter',
    isMet: /[a-z]/.test(state.value.password)
  },
  {
    label: 'Contains at least one number',
    isMet: /\d/.test(state.value.password)
  },
  {
    label: 'Contains at least one special character (!@#$%^&*)',
    isMet: /[!@#$%^&*]/.test(state.value.password)
  }
])

const formIsValid = computed(() => {
  return validatePassword(state.value.password) === true
})

const validatePassword = (value) => {
  const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).{8,}$/
  if (!value) return 'Password is required'
  if (!passwordRegex.test(value)) {
    return 'Password does not meet requirements'
  }
  return true
}

const signUp = async () => {
  if (!formIsValid.value) {
    toast.add({ title: 'Please fix the validation errors', color: 'red' })
    return
  }

  await api('User/signup', 'POST', state.value, false)
    .then(() => {
      toast.add({ title: 'Signup Successful, Redirecting to login' })
      router.push('/login')
    })
    .catch((err) => {
      console.error(err)
      toast.add({
        title: 'Signup failed',
        description: err.message,
        color: 'red'
      })
    })
}
</script>
