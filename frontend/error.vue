<script setup>
const error = useError()
const route = useRoute()

const errorType = computed(() => {
  if (error.value?.statusCode === 404) return 'not_found'
  if (error.value?.statusCode === 500) return 'server_error'
  if (!navigator.onLine) return 'offline'
  return 'generic'
})

const errorMessages = {
  not_found: {
    title: 'Page Not Found',
    description:
      'The page you are looking for might have been removed, had its name changed, or is temporarily unavailable.',
    icon: 'i-heroicons-magnifying-glass'
  },
  server_error: {
    title: 'Server Error',
    description:
      'Something went wrong on our end. Our team has been notified and we are working to fix it.',
    icon: 'i-heroicons-server-stack'
  },
  offline: {
    title: 'You Are Offline',
    description: 'Please check your internet connection and try again.',
    icon: 'i-heroicons-signal-slash'
  },
  generic: {
    title: 'Something Went Wrong',
    description: 'An unexpected error occurred. Please try again later.',
    icon: 'i-heroicons-exclamation-triangle'
  }
}

const primaryAction = computed(() => {
  return {
    not_found: { label: 'Go to Homepage', to: '/' },
    server_error: { label: 'Try Again', action: () => refresh() },
    offline: { label: 'Retry Connection', action: () => location.reload() },
    generic: { label: 'Go to Homepage', to: '/' }
  }[errorType.value]
})
</script>

<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900 flex flex-col">
    <header class="bg-white dark:bg-gray-800 shadow-sm">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
        <div class="flex justify-between items-center">
          <NuxtLink to="/" class="flex items-center space-x-2">
            <UIcon name="i-heroicons-link" class="w-8 h-8 text-primary-500" />
            <span class="text-xl font-bold text-gray-900 dark:text-white"
              >Bytely</span
            >
          </NuxtLink>
          <div class="flex space-x-4">
            <UButton
              to="/links/add"
              icon="i-heroicons-plus"
              label="Create New"
              color="primary"
              size="sm"
            />
          </div>
        </div>
      </div>
    </header>

    <!-- Error Content -->
    <main class="flex-grow flex items-center justify-center p-4 sm:p-6">
      <div class="w-full max-w-md">
        <UCard
          class="overflow-hidden text-center"
          :ui="{
            base: 'relative',
            background: 'dark:bg-gray-900/50 bg-white',
            ring: 'ring-1 ring-gray-200 dark:ring-gray-800',
            shadow: 'shadow-xl hover:shadow-2xl transition-shadow duration-300'
          }"
        >
          <!-- Decorative element -->
          <div
            class="absolute top-0 left-0 right-0 h-1 bg-red-500 dark:bg-red-400"
          ></div>

          <div class="space-y-6">
            <!-- Error icon -->
            <div
              class="mx-auto h-16 w-16 flex items-center justify-center rounded-full bg-red-100 dark:bg-red-900/50"
            >
              <UIcon
                :name="errorMessages[errorType].icon"
                class="h-8 w-8 text-red-600 dark:text-red-400"
              />
            </div>

            <!-- Error title and message -->
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
                {{ errorMessages[errorType].title }}
              </h1>
              <p class="mt-2 text-gray-600 dark:text-gray-300">
                {{ errorMessages[errorType].description }}
              </p>
              <p
                v-if="errorType === 'not_found'"
                class="mt-2 text-sm text-gray-500 dark:text-gray-400"
              >
                Requested URL: <span class="font-mono">{{ route.path }}</span>
              </p>
            </div>

            <!-- Error code (if available) -->
            <div
              v-if="error?.statusCode"
              class="inline-flex items-center px-3 py-1 rounded-full text-xs font-medium bg-gray-100 dark:bg-gray-800 text-gray-800 dark:text-gray-200"
            >
              Error {{ error.statusCode }}
            </div>

            <!-- Action buttons -->
            <div class="flex flex-col sm:flex-row gap-3 justify-center">
              <UButton
                v-if="primaryAction.to"
                :to="primaryAction.to"
                color="primary"
                variant="solid"
                size="lg"
                :label="primaryAction.label"
                class="flex-1"
              />
              <UButton
                v-else-if="primaryAction.action"
                @click="primaryAction.action"
                color="primary"
                variant="solid"
                size="lg"
                :label="primaryAction.label"
                class="flex-1"
              />
              <UButton
                to="/support"
                color="gray"
                variant="outline"
                size="lg"
                label="Contact Support"
                icon="i-heroicons-envelope"
                class="flex-1"
              />
            </div>
          </div>
        </UCard>

        <!-- Additional help -->
        <div class="mt-6 text-center text-sm text-gray-500 dark:text-gray-400">
          <p>
            Need help? Check our
            <NuxtLink
              to="/help"
              class="text-primary-500 dark:text-primary-400 hover:underline"
              >help center</NuxtLink
            >
          </p>
        </div>
      </div>
    </main>
  </div>
</template>

<style scoped>
/* Smooth transitions for error card */
.fade-enter-active,
.fade-leave-active {
  transition:
    opacity 0.3s ease,
    transform 0.3s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateY(10px);
}
</style>
