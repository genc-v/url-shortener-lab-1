<template>
  <div class="min-h-screen flex items-center justify-center p-4">
    <div class="w-full max-w-xl">
      <div
        class="rounded-2xl shadow-xl brightness-90 p-10 border border-gray-800 transition-all duration-300 hover:shadow-2xl"
      >
        <!-- Header -->
        <div class="mb-8 text-center">
          <h1 class="text-3xl font-bold">User Settings</h1>
          <p class="text-gray-400 mt-1">Update your account information</p>
        </div>

        <!-- Form -->
        <form @submit.prevent="handleSubmit" class="space-y-6">
          <!-- Full Name -->
          <div>
            <label
              for="fullName"
              class="block text-sm font-medium text-gray-300 mb-1"
            >
              Full Name <span class="text-red-500">*</span>
            </label>
            <UInput
              id="fullName"
              v-model="form.fullName"
              type="text"
              required
              placeholder="John Doe"
              icon="i-heroicons-user-16-solid"
              color="gray"
              variant="outline"
              class="w-full"
              :disabled="loading"
              :ui="{
                base: 'pl-10',
                color: {
                  gray: {
                    outline:
                      'dark:bg-gray-950 ring-gray-700 focus:ring-2 focus:ring-primary-500 dark:focus:ring-primary-500'
                  }
                }
              }"
            />
          </div>

          <!-- Email -->
          <div>
            <label
              for="email"
              class="block text-sm font-medium text-gray-300 mb-1"
            >
              Email <span class="text-red-500">*</span>
            </label>
            <UInput
              id="email"
              v-model="form.email"
              type="email"
              required
              placeholder="john@example.com"
              icon="i-heroicons-envelope-16-solid"
              color="gray"
              variant="outline"
              class="w-full"
              :disabled="loading"
              :ui="{
                base: 'pl-10',
                color: {
                  gray: {
                    outline:
                      'dark:bg-gray-950 ring-gray-700 focus:ring-2 focus:ring-primary-500 dark:focus:ring-primary-500'
                  }
                }
              }"
            />
          </div>

          <!-- Submit Button -->
          <UButton
            type="submit"
            :disabled="loading || !formChanged"
            block
            color="primary"
            variant="solid"
            class="transition-all duration-200 cursor-pointer"
            :ui="{ rounded: 'rounded-lg', padding: { xl: 'px-6 py-3.5' } }"
          >
            <template #leading>
              <UIcon
                v-if="!loading"
                name="i-heroicons-check-20-solid"
                class="w-5 h-5"
              />
              <UIcon
                v-else
                name="i-heroicons-arrow-path-20-solid"
                class="animate-spin w-5 h-5"
              />
            </template>
            {{ loading ? 'Saving...' : 'Save Changes' }}
          </UButton>

          <!-- Password Change Section -->
          <div class="pt-4 border-t border-gray-800">
            <h3 class="text-lg font-medium text-gray-300 mb-4">
              Change Password
            </h3>

            <!-- Current Password -->
            <div class="mb-4">
              <label
                for="currentPassword"
                class="block text-sm font-medium text-gray-300 mb-1"
              >
                Current Password
              </label>
              <UInput
                id="currentPassword"
                v-model="passwordForm.currentPassword"
                type="password"
                placeholder="••••••••"
                icon="i-heroicons-lock-closed-16-solid"
                color="gray"
                variant="outline"
                class="w-full"
                :disabled="loading"
                :ui="{
                  base: 'pl-10',
                  color: {
                    gray: {
                      outline:
                        'dark:bg-gray-950 ring-gray-700 focus:ring-2 focus:ring-primary-500 dark:focus:ring-primary-500'
                    }
                  }
                }"
              />
            </div>

            <!-- New Password -->
            <div class="mb-4">
              <UFormField
                label="New Password"
                name="newPassword"
                :rules="validatePassword"
              >
                <UInput
                  id="newPassword"
                  v-model="passwordForm.newPassword"
                  type="password"
                  placeholder="••••••••"
                  icon="i-heroicons-lock-closed-16-solid"
                  color="gray"
                  variant="outline"
                  class="w-full"
                  :disabled="loading"
                  :ui="{
                    base: 'pl-10',
                    color: {
                      gray: {
                        outline:
                          'dark:bg-gray-950 ring-gray-700 focus:ring-2 focus:ring-primary-500 dark:focus:ring-primary-500'
                      }
                    }
                  }"
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
                        'text-xs',
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
            </div>

            <!-- Confirm New Password -->
            <div>
              <label
                for="confirmPassword"
                class="block text-sm font-medium text-gray-300 mb-1"
              >
                Confirm New Password
              </label>
              <UInput
                id="confirmPassword"
                v-model="passwordForm.confirmPassword"
                type="password"
                placeholder="••••••••"
                icon="i-heroicons-lock-closed-16-solid"
                color="gray"
                variant="outline"
                class="w-full"
                :disabled="loading"
                :ui="{
                  base: 'pl-10',
                  color: {
                    gray: {
                      outline:
                        'dark:bg-gray-950 ring-gray-700 focus:ring-2 focus:ring-primary-500 dark:focus:ring-primary-500'
                    }
                  }
                }"
              />
              <p
                v-if="
                  passwordForm.newPassword &&
                  passwordForm.confirmPassword &&
                  passwordForm.newPassword !== passwordForm.confirmPassword
                "
                class="mt-1 text-xs text-red-500"
              >
                Passwords do not match
              </p>
            </div>

            <!-- Change Password Button -->
            <UButton
              type="button"
              @click="handlePasswordChange"
              :disabled="loading || !passwordIsValid"
              block
              color="primary"
              variant="outline"
              class="mt-4 transition-all duration-200"
              :ui="{ rounded: 'rounded-lg', padding: { xl: 'px-6 py-3.5' } }"
            >
              <template #leading>
                <UIcon
                  v-if="!loading"
                  name="i-heroicons-key-20-solid"
                  class="w-5 h-5"
                />
                <UIcon
                  v-else
                  name="i-heroicons-arrow-path-20-solid"
                  class="animate-spin w-5 h-5"
                />
              </template>
              {{ loading ? 'Updating...' : 'Update Password' }}
            </UButton>
          </div>
        </form>

        <transition name="fade">
          <div
            v-if="successMessage"
            class="mt-6 p-3 bg-green-900/50 rounded-lg border border-green-800 text-green-100 text-center"
          >
            {{ successMessage }}
          </div>
        </transition>

        <transition name="fade">
          <div
            v-if="errorMessage"
            class="mt-6 p-3 bg-red-900/50 rounded-lg border border-red-800 text-red-100 text-center"
          >
            {{ errorMessage }}
          </div>
        </transition>
      </div>
    </div>
  </div>
</template>

<script setup>
const toast = useToast()
const loading = ref(false)
const successMessage = ref('')
const errorMessage = ref('')
const initialFormData = ref(null)

const form = reactive({
  fullName: '',
  email: ''
})

const passwordForm = reactive({
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
})

const formChanged = computed(() => {
  return (
    initialFormData.value &&
    (form.fullName !== initialFormData.value.fullName ||
      form.email !== initialFormData.value.email)
  )
})

const passwordChanged = computed(() => {
  return (
    passwordForm.currentPassword &&
    passwordForm.newPassword &&
    passwordForm.confirmPassword &&
    passwordForm.newPassword === passwordForm.confirmPassword &&
    passwordForm.newPassword.length >= 8
  )
})

const fetchUserData = async () => {
  loading.value = true
  const userId = useCookie('userId').value

  await api(`User/${userId}`, 'GET')
    .then((response) => {
      form.fullName = response.fullName
      form.email = response.email
      initialFormData.value = { ...response }
    })
    .catch((error) => {
      errorMessage.value = 'Failed to fetch user data'
      console.error('Error fetching user data:', error)
    })
    .finally(() => {
      loading.value = false
    })
}

const handleSubmit = async () => {
  loading.value = true
  successMessage.value = ''
  errorMessage.value = ''
  const userId = useCookie('userId').value

  await api(`User/${userId}`, 'PUT', {
    fullName: form.fullName,
    email: form.email
  })
    .then(() => {
      successMessage.value = 'Profile updated successfully'
      initialFormData.value = { ...form }
      setTimeout(() => {
        successMessage.value = ''
      }, 3000)
    })
    .catch((error) => {
      errorMessage.value = error.message || 'Failed to update profile'
      setTimeout(() => {
        errorMessage.value = ''
      }, 3000)
    })
    .finally(() => {
      loading.value = false
    })
}

const handlePasswordChange = async () => {
  loading.value = true
  successMessage.value = ''
  errorMessage.value = ''
  const userId = useCookie('userId').value

  await api('User/' + userId, 'PUT', {
    email: null,
    fullName: null,
    oldPassword: passwordForm.currentPassword,
    newPassword: passwordForm.confirmPassword
  })
    .then(() => {
      successMessage.value = 'Password changed successfully'
      passwordForm.currentPassword = ''
      passwordForm.newPassword = ''
      passwordForm.confirmPassword = ''
      setTimeout(() => {
        successMessage.value = ''
      }, 3000)
    })
    .catch((error) => {
      errorMessage.value = error.message || 'Failed to change password'
      setTimeout(() => {
        errorMessage.value = ''
      }, 3000)
    })
    .finally(() => {
      loading.value = false
    })
}

const passwordRequirements = computed(() => [
  {
    label: 'At least 8 characters',
    isMet: passwordForm.newPassword.length >= 8
  },
  {
    label: 'Contains at least one uppercase letter',
    isMet: /[A-Z]/.test(passwordForm.newPassword)
  },
  {
    label: 'Contains at least one lowercase letter',
    isMet: /[a-z]/.test(passwordForm.newPassword)
  },
  {
    label: 'Contains at least one number',
    isMet: /\d/.test(passwordForm.newPassword)
  },
  {
    label: 'Contains at least one special character (!@#$%^&*)',
    isMet: /[!@#$%^&*]/.test(passwordForm.newPassword)
  }
])

const validatePassword = (value) => {
  const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).{8,}$/
  if (!value) return 'Password is required'
  if (!passwordRegex.test(value)) {
    return 'Password does not meet requirements'
  }
  return true
}

const passwordIsValid = computed(() => {
  return (
    passwordForm.currentPassword &&
    passwordForm.newPassword &&
    passwordForm.confirmPassword &&
    passwordForm.newPassword === passwordForm.confirmPassword &&
    validatePassword(passwordForm.newPassword) === true
  )
})

fetchUserData()
useHead({
  title: 'Settings'
})
</script>

<style>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
