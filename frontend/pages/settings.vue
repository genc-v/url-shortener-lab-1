<template>
  <div class="min-h-screen flex items-center justify-center p-4">
    <div class="w-full max-w-xl">
      <div
        class="bg-gray-900 rounded-2xl shadow-xl p-10 border border-gray-800 transition-all duration-300 hover:shadow-2xl"
      >
        <!-- Header -->
        <div class="mb-8 text-center">
          <h1 class="text-3xl font-bold text-white">User Settings</h1>
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
            class="transition-all duration-200"
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
        </form>

        <!-- Success Message -->
        <transition name="fade">
          <div
            v-if="successMessage"
            class="mt-6 p-3 bg-green-900/50 rounded-lg border border-green-800 text-green-100 text-center"
          >
            {{ successMessage }}
          </div>
        </transition>
      </div>
    </div>
  </div>
</template>

<script setup>
const config = useState('config')
const toast = useToast()
const loading = ref(false)
const successMessage = ref('')
const initialFormData = ref(null)

// Form data
const form = reactive({
  fullName: '',
  email: ''
})

// Get user ID from JWT
const getUserIdFromToken = () => {
  const token = localStorage.getItem('token')
  if (!token) return null

  try {
    const payload = JSON.parse(atob(token.split('.')[1]))
    return payload.nameId // Adjust this based on your JWT structure
  } catch (error) {
    console.error('Error decoding token:', error)
    return null
  }
}

// Check if form has changed
const formChanged = computed(() => {
  return (
    initialFormData.value &&
    (form.fullName !== initialFormData.value.fullName ||
      form.email !== initialFormData.value.email)
  )
})

// Fetch user data
const fetchUserData = async () => {
  const userId = getUserIdFromToken()
  if (!userId) {
    toast.add({
      title: 'Authentication Error',
      description: 'Please log in to view settings',
      color: 'red'
    })
    return
  }

  loading.value = true

  try {
    const response = await $fetch(`${config.value}/api/User/${userId}`, {
      method: 'GET',
      headers: {
        Authorization: `Bearer ${localStorage.getItem('token')}`
      }
    })

    form.fullName = response.fullName
    form.email = response.email
    initialFormData.value = { ...response }
  } catch (error) {
    toast.add({
      title: 'Error',
      description: 'Failed to fetch user data',
      color: 'red'
    })
    console.error('Error fetching user data:', error)
  } finally {
    loading.value = false
  }
}

// Handle form submission
const handleSubmit = async () => {
  const userId = getUserIdFromToken()
  if (!userId) return

  loading.value = true
  successMessage.value = ''

  try {
    const response = await $fetch(`${config.value}/api/User/${userId}`, {
      method: 'PUT',
      headers: {
        Authorization: `Bearer ${localStorage.getItem('token')}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        fullName: form.fullName,
        email: form.email
      })
    })

    successMessage.value = 'Your changes have been saved successfully'
    initialFormData.value = { ...form }

    setTimeout(() => {
      successMessage.value = ''
    }, 3000)
  } catch (error) {
    toast.add({
      title: 'Error',
      description: 'Failed to update user data',
      color: 'red'
    })
    console.error('Error updating user data:', error)
  } finally {
    loading.value = false
  }
}

// Fetch user data on component mount
onMounted(() => {
  fetchUserData()
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
