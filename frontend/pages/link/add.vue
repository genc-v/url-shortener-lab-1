<template>
  <div class="min-h-screen flex items-center justify-center p-4">
    <div class="w-full max-w-xl">
      <div
        class="bg-gray-900 rounded-2xl shadow-xl p-10 border border-gray-800 transition-all duration-300 hover:shadow-2xl"
      >
        <!-- Header -->
        <div class="mb-8 text-center">
          <h1 class="text-3xl font-bold text-white">Create New Short Link</h1>
          <p class="text-gray-400 mt-1">Quickly shorten and track your URLs</p>
        </div>

        <!-- Form -->
        <form @submit.prevent="handleSubmit" class="space-y-6">
          <!-- Destination URL -->
          <div>
            <label
              for="url"
              class="block text-sm font-medium text-gray-300 mb-1"
            >
              Destination URL <span class="text-red-500">*</span>
            </label>
            <UInput
              id="url"
              v-model="link"
              type="url"
              required
              ref="urlInput"
              placeholder="https://example.com"
              icon="i-heroicons-link-16-solid"
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

          <!-- Optional Description -->
          <div>
            <label
              for="description"
              class="block text-sm font-medium text-gray-300 mb-1"
            >
              Description
            </label>
            <UTextarea
              id="description"
              v-model="description"
              rows="3"
              placeholder="Add a description to help with analytics..."
              color="gray"
              variant="outline"
              class="w-full"
              :disabled="loading"
              :ui="{
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
            :disabled="!link || loading"
            block
            color="primary"
            variant="solid"
            class="transition-all duration-200"
            :ui="{ rounded: 'rounded-lg', padding: { xl: 'px-6 py-3.5' } }"
          >
            <template #leading>
              <UIcon
                v-if="!loading"
                name="i-heroicons-plus-20-solid"
                class="w-5 h-5"
              />
              <UIcon
                v-else
                name="i-heroicons-arrow-path-20-solid"
                class="animate-spin w-5 h-5"
              />
            </template>
            {{ loading ? 'Creating...' : 'Create Short Link' }}
          </UButton>
        </form>

        <!-- Result Box -->
        <transition name="fade" mode="out-in">
          <div
            v-if="shortUrl"
            class="mt-8 p-4 bg-gray-800 rounded-lg border border-gray-700 text-gray-100"
          >
            <p class="mb-2 text-sm text-gray-400">Your short link:</p>
            <div class="flex items-center justify-between gap-2">
              <a
                :href="linkLocation()"
                target="_blank"
                rel="noopener"
                class="text-primary-400 underline truncate"
              >
                {{ shortUrl }}
              </a>
              <UButton
                size="sm"
                color="gray"
                variant="outline"
                @click="copyToClipboard"
              >
                <template #leading>
                  <UIcon
                    name="i-heroicons-clipboard-document-20-solid"
                    class="w-4 h-4"
                  />
                </template>
                Copy
              </UButton>
            </div>
          </div>
        </transition>

        <!-- Footer Info -->
        <div class="mt-6 text-center text-sm text-gray-500">
          <p>
            All links are public. You can manage or generate QR codes on the
            <nuxt-link to="/link/all" class="text-primary-400 underline ml-1"
              >dashboard</nuxt-link
            >.
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const config = useState('config')
const link = ref('')
const description = ref('')
const shortUrl = ref('')
const loading = ref(false)

const toast = useToast()
const urlInput = ref(null)

const handleSubmit = async () => {
  loading.value = true
  shortUrl.value = ''

  await api('URL', 'POST', {
    url: link.value,
    description: description.value
  })
    .then((response) => {
      shortUrl.value = response.shortedUrl

      toast.add({
        title: 'Success!',
        description: 'Your short link has been generated.',
        color: 'green'
      })
      link.value = ''
      description.value = ''
    })
    .catch((error) => {
      console.error('Error:', error)
      toast.add({
        title: 'Error',
        description: 'Failed to create short link. Please try again.',
        color: 'red'
      })
    })
    .finally(() => {
      loading.value = false
    })
}
const linkLocation = () => {
  const baseUrl = window.location.origin
  return `${baseUrl}/${shortUrl.value}`
}
const copyToClipboard = async () => {
  try {
    const baseUrl = window.location.origin
    const fullUrl = `${baseUrl}/${shortUrl.value}`

    await navigator.clipboard.writeText(fullUrl)
    toast.add({
      title: 'URL created!',
      description: 'The shortened URL is ready to share',
      color: 'success',
      icon: 'i-lucide-check-circle-2',
      timeout: 3000
    })
  } catch (err) {
    toast.add({
      title: 'Failed to Copy',
      description: 'Please try manually.',
      color: 'red'
    })
  }
}
</script>

<style>
.fade-enter-active,
.fade-leave-active {
  transition: all 0.3s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateY(10px);
}
</style>
