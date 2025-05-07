<template>
  <div class="flex flex-col h-full max-w-[1440px] p-5 mx-auto">
    <!-- Header -->
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          Link Details
        </h1>
        <p class="text-sm text-gray-500 dark:text-gray-400 mt-1">
          Detailed analytics and information for your shortened URL
        </p>
      </div>
      <div class="flex space-x-3">
        <UButton
          icon="i-lucide-refresh-ccw"
          variant="outline"
          size="sm"
          label="Refresh"
          :loading="isRefreshing"
          @click="fetchLinkDetails"
        />
      </div>
    </div>

    <!-- Main Content -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Left Column - Link Information -->
      <div class="lg:col-span-2 space-y-6">
        <!-- Link Card -->
        <UCard>
          <template #header>
            <div class="flex items-center justify-between">
              <h2 class="text-lg font-semibold">Link Information</h2>
              <div class="flex space-x-2">
                <UButton
                  v-if="!isEditing"
                  icon="i-lucide-edit"
                  size="sm"
                  label="Edit"
                  class="cursor-pointer"
                  @click="startEditing"
                />
                <UButton
                  icon="i-lucide-trash-2"
                  class="cursor-pointer"
                  size="sm"
                  label="Delete"
                  color="red"
                  @click="isDeleteModalOpen = true"
                />
              </div>
            </div>
          </template>

          <div class="space-y-4">
            <div class="flex items-start">
              <div
                class="w-32 text-sm font-medium text-gray-500 dark:text-gray-400"
              >
                Short URL
              </div>
              <div class="flex-1">
                <div class="flex items-center gap-2">
                  <span
                    class="font-mono text-primary-600 dark:text-primary-400"
                  >
                    {{ config.API }}{{ link.shortUrl }}
                  </span>
                  <UButton
                    icon="i-lucide-copy"
                    size="xs"
                    color="gray"
                    variant="ghost"
                    @click="copyToClipboard(`${config.API}/${link.shortUrl}`)"
                  />
                </div>
              </div>
            </div>

            <div class="flex items-start">
              <div
                class="w-32 text-sm font-medium text-gray-500 dark:text-gray-400"
              >
                Description
              </div>
              <div class="flex-1">
                <template v-if="isEditing">
                  <div class="flex flex-col gap-2">
                    <UInput
                      v-model="editedData.description"
                      type="text"
                      placeholder="Enter description"
                    />
                    <div class="flex gap-2 mt-2">
                      <UButton
                        icon="i-lucide-check"
                        size="sm"
                        color="green"
                        @click="updateLink"
                        label="Save"
                      />
                      <UButton
                        icon="i-lucide-x"
                        size="sm"
                        color="gray"
                        @click="cancelEditing"
                        label="Cancel"
                      />
                    </div>
                  </div>
                </template>
                <template v-else>
                  <p v-if="link.description">{{ link.description }}</p>
                  <p v-else class="text-gray-400">No description provided</p>
                </template>
              </div>
            </div>

            <div class="flex items-start">
              <div
                class="w-32 text-sm font-medium text-gray-500 dark:text-gray-400"
              >
                Destination
              </div>
              <div class="flex-1">
                <a
                  :href="link.originalUrl"
                  target="_blank"
                  rel="noopener noreferrer"
                  class="text-primary-600 dark:text-primary-400 hover:underline break-all"
                >
                  {{ link.originalUrl }}
                </a>
              </div>
            </div>

            <div class="flex items-start">
              <div
                class="w-32 text-sm font-medium text-gray-500 dark:text-gray-400"
              >
                Created
              </div>
              <div class="flex-1">
                <p>{{ formatDateTime(link.dateCreated) }}</p>
              </div>
            </div>
          </div>
        </UCard>

        <!-- Analytics Card -->
        <UCard>
          <template #header>
            <h2 class="text-lg font-semibold">Analytics Overview</h2>
          </template>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div class="border border-gray-500 rounded-lg p-4">
              <div class="flex items-center justify-between">
                <div>
                  <p
                    class="text-sm font-medium text-gray-500 dark:text-gray-400"
                  >
                    Total Clicks
                  </p>
                  <p class="text-2xl font-semibold">{{ link.nrOfClicks }}</p>
                </div>
                <UIcon
                  name="i-lucide-mouse-pointer-click"
                  class="w-8 h-8 text-blue-500"
                />
              </div>
            </div>

            <div class="border border-gray-500 rounded-lg p-4">
              <div class="flex items-center justify-between">
                <div>
                  <p
                    class="text-sm font-medium text-gray-500 dark:text-gray-400"
                  >
                    Last Click
                  </p>
                  <p class="text-lg font-semibold">
                    {{
                      link.lastClickDate
                        ? formatDateTime(link.lastClickDate)
                        : 'Never'
                    }}
                  </p>
                </div>
                <UIcon name="i-lucide-clock" class="w-8 h-8 text-purple-500" />
              </div>
            </div>
          </div>
        </UCard>
      </div>

      <!-- Right Column - Quick Actions -->
      <div class="space-y-6">
        <!-- QR Code Card -->
        <UCard>
          <template #header>
            <h2 class="text-lg font-semibold">QR Code</h2>
          </template>

          <div class="flex flex-col items-center space-y-4">
            <div class="border rounded-lg p-4 bg-white">
              <QrcodeVue
                :value="qrCodeValue"
                :size="128"
                level="H"
                class="mx-auto"
                render-as="canvas"
              />
            </div>
            <div class="flex flex-col w-full gap-2">
              <UButton
                icon="i-lucide-download"
                label="Download QR Code"
                size="sm"
                class="w-full"
                @click="downloadQRCode"
              />
              <UButton
                icon="i-lucide-palette"
                label="Customize QR Code"
                size="sm"
                variant="outline"
                class="w-full"
                @click="navigateTo(`/qr-code/customize/${link.id}`)"
              />
            </div>
          </div>
        </UCard>

        <!-- Quick Actions Card -->
        <UCard>
          <template #header>
            <h2 class="text-lg font-semibold">Quick Actions</h2>
          </template>

          <div class="space-y-3">
            <UButton
              icon="i-lucide-copy"
              label="Copy Short URL"
              block
              @click="copyToClipboard(`${config}/${link.shortUrl}`)"
            />
            <UButton
              icon="i-lucide-share-2"
              label="Share Link"
              block
              variant="outline"
            />
          </div>
        </UCard>
      </div>
    </div>

    <!-- Delete Modal -->
    <transition name="fade" mode="out-in">
      <div
        v-if="isDeleteModalOpen"
        class="fixed inset-0 flex items-center justify-center bg-[#00000073] z-50 backdrop-blur-sm"
      >
        <div
          class="bg-white dark:bg-gray-800 rounded-lg shadow-lg p-6 w-[90%] max-w-md relative"
        >
          <button
            @click="closeDeleteModal"
            class="absolute cursor-pointer top-3 right-3 text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200"
          >
            <UIcon name="i-lucide-x" class="w-5 h-5" />
          </button>
          <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
            Confirm Deletion
          </h2>
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-6">
            Are you sure you want to delete the URL
            <span class="font-semibold">{{ link.shortUrl }}</span
            >?
          </p>
          <div class="flex justify-end space-x-3">
            <button
              @click="closeDeleteModal"
              class="px-4 py-2 text-sm font-medium cursor-pointer text-gray-700 bg-gray-200 rounded hover:bg-gray-300 dark:bg-gray-700 dark:text-gray-300 dark:hover:bg-gray-600"
            >
              Cancel
            </button>
            <button
              @click="confirmDelete"
              :disabled="isDeleting"
              class="px-4 py-2 text-sm cursor-pointer font-medium text-white bg-red-600 rounded hover:bg-red-700"
            >
              <span v-if="isDeleting" class="animate-spin mr-2">⏳</span>
              Delete
            </button>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup>
import QrcodeVue from 'qrcode.vue'

const route = useRoute()
const router = useRouter()
const toast = useToast()
const config = useState('config')

// State
const link = ref({
  id: 0,
  originalUrl: '',
  shortUrl: '',
  nrOfClicks: 0,
  userId: 0,
  description: '',
  dateCreated: null,
  lastClickDate: null
})
const isLoading = ref(true)
const isRefreshing = ref(false)
const isDeleteModalOpen = ref(false)
const isDeleting = ref(false)
const isEditing = ref(false)
const editedData = ref({
  description: link.value.description || ''
})

// Computed
const qrCodeValue = computed(() => `${config.value}/${link.value.shortUrl}`)

// Methods
const startEditing = () => {
  editedData.value = {
    description: link.value.description || ''
  }
  isEditing.value = true
}

const cancelEditing = () => {
  isEditing.value = false
  editedData.value = {
    description: link.value.description || ''
  }
}

const updateLink = async () => {
  try {
    await api(`URL/${route.params.slug}`, 'PUT', editedData.value.description)
    await fetchLinkDetails()
    isEditing.value = false

    toast.add({
      title: 'Link updated',
      description: 'The link has been successfully updated',
      color: 'success',
      icon: 'i-lucide-check-circle-2'
    })
  } catch (error) {
    console.error('Update failed:', error)
    toast.add({
      title: 'Update failed',
      description: 'Could not update the link',
      color: 'error',
      icon: 'i-lucide-alert-circle'
    })
  }
}

const fetchLinkDetails = async () => {
  try {
    const data = await api(`URL/${route.params.slug}`)
    link.value = data
  } finally {
    isLoading.value = false
    isRefreshing.value = false
  }
}

const formatDateTime = (dateString) => {
  if (!dateString) return 'N/A'
  return new Date(dateString).toLocaleString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    hour12: false
  })
}

const copyToClipboard = (text) => {
  navigator.clipboard
    .writeText(text)
    .then(() => {
      toast.add({
        title: 'Copied!',
        description: 'The URL has been copied to your clipboard',
        color: 'success',
        icon: 'i-lucide-check-circle-2',
        timeout: 3000
      })
    })
    .catch((err) => {
      console.error('Failed to copy:', err)
      toast.add({
        title: 'Copy failed',
        description: 'Please try again or copy manually',
        color: 'error',
        icon: 'i-lucide-alert-circle'
      })
    })
}

const downloadQRCode = () => {
  const canvas = document.querySelector('canvas')
  if (!canvas) {
    toast.add({
      title: 'Error',
      description: 'Could not generate QR code',
      color: 'red',
      icon: 'i-lucide-alert-circle'
    })
    return
  }

  const downloadLink = document.createElement('a')
  downloadLink.download = `${link.value.shortUrl}-qrcode.png`
  downloadLink.href = canvas.toDataURL('image/png')
  document.body.appendChild(downloadLink)
  downloadLink.click()
  document.body.removeChild(downloadLink)

  toast.add({
    title: 'Download started',
    description: 'QR code is being downloaded',
    color: 'green',
    icon: 'i-lucide-check-circle-2'
  })
}

const confirmDelete = async () => {
  isDeleting.value = true
  api('api/Url/' + route.params.slug, 'DELETE')
    .then(() => {
      toast.add({
        title: 'URL deleted',
        description: `${link.value.shortUrl} was removed`,
        color: 'success',
        icon: 'i-lucide-trash-2'
      })
      router.push('/links')
    })
    .catch((error) => {
      console.error(error)
      toast.add({
        title: 'Deletion failed',
        description: 'Could not delete the URL',
        color: 'red',
        icon: 'i-lucide-alert-circle'
      })
    })
    .finally(() => {
      isDeleting.value = false
      isDeleteModalOpen.value = false
    })
}

const closeDeleteModal = () => {
  isDeleteModalOpen.value = false
}

fetchLinkDetails()

watch(isDeleteModalOpen, (isOpen) => {
  const handler = (event) => {
    if (event.key === 'Escape') closeDeleteModal()
  }
  if (isOpen) document.addEventListener('keydown', handler)
  else document.removeEventListener('keydown', handler)
})

watch(isEditing, (isEditing) => {
  const handler = (event) => {
    if (event.key === 'Escape') cancelEditing()
  }
  if (isEditing) document.addEventListener('keydown', handler)
  else document.removeEventListener('keydown', handler)
})
</script>
