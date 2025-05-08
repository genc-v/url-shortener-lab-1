<script setup>
const UButton = resolveComponent('UButton')
const UDropdownMenu = resolveComponent('UDropdownMenu')
const UCard = resolveComponent('UCard')
const UProgress = resolveComponent('UProgress')
const UPagination = resolveComponent('UPagination')
const UTable = resolveComponent('UTable')

// Composables
const toast = useToast()
const route = useRoute()
const router = useRouter()
const config = useState('config')

// State
const data = ref([])
const currentpage = ref(route.query.page || 1)
const isLoading = ref(true)
const isRefreshing = ref(false)
const page = ref(parseInt(route.query.page) || 1)
const pageSize = ref(10)
const totalPages = ref(1)
const isDeleteModalOpen = ref(false)
const itemToDelete = ref(null)
const isDeleting = ref(false)
const items = ref([10, 20, 50, 100])
const value = ref(items.value[0])
const searchQuery = ref(route.query.searchQuery || '')
const isSearching = ref(false)

// Computed properties
const filteredData = computed(() => data.value)
const totalClicks = computed(() =>
  data.value.reduce((sum, url) => sum + url.nrOfClicks, 0)
)

const topLinks = computed(() =>
  [...data.value].sort((a, b) => b.nrOfClicks - a.nrOfClicks).slice(0, 3)
)
const clicksGrowth = computed(() => {
  if (data.value.length < 3) return 0
  const currentPeriod = totalClicks.value
  const previousPeriod = currentPeriod * 1.8
  return ((currentPeriod - previousPeriod) / previousPeriod) * 100
})

const fetchUrls = async () => {
  try {
    api(`User/Urls/${page.value}/${pageSize.value}`, 'GET').then((result) => {
      data.value = result.urls
      totalPages.value = result.totalPages
    })
  } catch (error) {
    console.error('URL fetch failed:', error)
    toast.add({
      title: 'Failed to load URLs',
      description: error.message || 'Please try again later',
      color: 'error',
      icon: 'i-lucide-alert-circle',
      timeout: 5000
    })
  } finally {
    isLoading.value = false
    isRefreshing.value = false
  }
}

await fetchUrls()

if (totalPages.value < (route.query.page || 1)) {
  toast.add({
    title: 'Page not found',
    description: `The page ${route.query.page} does not exist, redireecting to pagg 1`,
    color: 'error',
    icon: 'i-lucide-alert-circle'
  })
  page.value = 1
  router.push({
    query: {
      page: 1
    }
  })
  await fetchUrls()
}

watch(
  () => page.value,
  (a) => {
    window.scrollTo(0, 0)
    router.push({
      query: {
        page: a
      }
    })
    fetchUrls()
  }
)
watch(
  () => value.value,
  (a) => {
    page.value = 1
    pageSize.value = a
    fetchUrls()
  }
)

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

const copyShortUrl = (shortCode) => {
  const fullUrl = `${config.value}/${shortCode}`
  copyToClipboard(fullUrl)
}

const copyToClipboard = (text) => {
  navigator.clipboard
    .writeText(text)
    .then(() => {
      toast.add({
        title: 'URL copied!',
        description: 'The shortened URL is ready to share',
        color: 'success',
        icon: 'i-lucide-check-circle-2',
        timeout: 3000
      })
    })
    .catch((err) => {
      console.error('Copy failed:', err)
      toast.add({
        title: 'Copy failed',
        description: 'Please try again or use browser controls',
        color: 'error',
        icon: 'i-lucide-alert-circle'
      })
    })
}

const confirmDelete = async () => {
  if (!itemToDelete.value) return

  isDeleting.value = true
  api(`URL/${itemToDelete.value.id}`, 'DELETE')
    .then(() => {
      toast.add({
        title: 'URL deleted',
        description: `${itemToDelete.value.shortUrl} was removed`,
        color: 'success',
        icon: 'i-lucide-trash-2'
      })
      fetchUrls()
    })
    .finally(() => {
      isDeleting.value = false
      isDeleteModalOpen.value = false
      itemToDelete.value = null
    })
}

const closeDeleteModal = () => {
  isDeleteModalOpen.value = false
  itemToDelete.value = null
}

const handlePageChange = (newPage) => {
  router.push({
    query: {
      ...route.query,
      page: newPage
    }
  })
}

// Table configuration
const columns = [
  {
    accessorKey: 'id',
    header: 'ID',
    cell: ({ row }) => `#${row.getValue('id')}`,
    meta: { width: '80px', align: 'center' }
  },
  {
    accessorKey: 'originalUrl',
    header: 'Destination URL',
    meta: { minWidth: '200px', truncate: true }
  },
  {
    accessorKey: 'shortUrl',
    header: 'Short Code',
    meta: { width: '120px' }
  },
  {
    accessorKey: 'nrOfClicks',
    header: 'Clicks',
    meta: { width: '100px', align: 'center' }
  },
  {
    accessorKey: 'description',
    header: 'Description',
    meta: { minWidth: '150px', truncate: true }
  },
  {
    accessorKey: 'dateCreated',
    header: 'Created At',
    cell: ({ row }) => formatDateTime(row.getValue('dateCreated')),
    meta: { width: '180px' }
  },
  {
    id: 'actions',
    header: 'Actions',
    meta: { width: '60px', align: 'center' },
    cell: ({ row }) =>
      h(
        'div',
        { class: 'flex justify-end' },
        h(
          UDropdownMenu,
          {
            content: { align: 'end', class: 'min-w-[180px] shadow-lg' },
            items: [
              {
                type: 'label',
                label: 'URL Management',
                class:
                  'font-semibold text-xs uppercase tracking-wider text-gray-500 dark:text-gray-400'
              },
              {
                label: 'Copy Short URL',
                icon: 'i-lucide-copy',
                onSelect: () => copyShortUrl(row.original.shortUrl)
              },

              { type: 'separator' },
              {
                label: 'Edit Details',
                icon: 'i-lucide-pencil',
                onSelect: () => navigateTo(`/link/${row.original.id}`)
              },
              { type: 'separator' },
              {
                label: 'Delete',
                icon: 'i-lucide-trash-2',
                class:
                  'text-red-500 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20',
                onSelect: () => {
                  itemToDelete.value = row.original
                  isDeleteModalOpen.value = true
                }
              }
            ]
          },
          () =>
            h(UButton, {
              icon: 'i-lucide-ellipsis-vertical',
              color: 'neutral',
              variant: 'ghost',
              class: 'h-8 w-8 p-0 hover:bg-gray-100 dark:hover:bg-gray-700'
            })
        )
      )
  }
]
const SearchUrls = async () => {
  if (searchQuery.value) {
    api('search?UrlName=' + searchQuery.value, 'GET')
      .then((result) => {
        console.log(result)
        data.value = result
      })
      .catch(() => {
        data.value = []
        toast.add({
          title: 'Search failed',
          description: 'No results found for "' + searchQuery.value + '"',
          color: 'error',
          icon: 'i-lucide-alert-circle',
          timeout: 5000
        })
      })
      .finally(() => {
        isLoading.value = false
      })
  } else {
    isSearching.value = false
  }
}

watch(isDeleteModalOpen, (isOpen) => {
  const handler = (event) => {
    if (event.key === 'Escape') closeDeleteModal()
  }
  if (isOpen) document.addEventListener('keydown', handler)
  else document.removeEventListener('keydown', handler)
})

const debouncedSearch = debounce(() => {
  isSearching.value = true
  router.push({
    query: {
      searchQuery: searchQuery.value
    }
  })
  if (searchQuery.value == '') {
    router.push({
      query: {
        page: 1
      }
    })
    isSearching.value = false
    fetchUrls()
    return
  }
  page.value = 1
  SearchUrls()
}, 500)
watch(searchQuery, debouncedSearch)
</script>

<template>
  <div class="flex flex-col h-full max-w-[1440px] p-5 mx-auto">
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
            class="absolute top-3 right-3 text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200"
          >
            <UIcon name="i-lucide-x" class="w-5 h-5" />
          </button>
          <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
            Confirm Deletion
          </h2>
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-6">
            Are you sure you want to delete the URL
            <span class="font-semibold">{{ itemToDelete?.shortUrl }}</span
            >?
          </p>
          <div class="flex justify-end space-x-3">
            <button
              @click="closeDeleteModal"
              class="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-200 rounded hover:bg-gray-300 dark:bg-gray-700 dark:text-gray-300 dark:hover:bg-gray-600"
            >
              Cancel
            </button>
            <button
              @click="confirmDelete"
              :disabled="isDeleting"
              class="px-4 py-2 text-sm font-medium text-white bg-red-600 rounded hover:bg-red-700"
            >
              <span v-if="isDeleting" class="animate-spin mr-2">⏳</span>
              Delete
            </button>
          </div>
        </div>
      </div>
    </transition>

    <!-- Header -->
    <div class="flex flex-wrap items-center justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          URL Dashboard
        </h1>
        <p class="text-sm text-gray-500 dark:text-gray-400 mt-1">
          Manage and analyze your shortened URLs
        </p>
      </div>
      <div class="">
        <UInput
          v-model="searchQuery"
          placeholder="Search URLs..."
          icon="i-lucide-search"
          size="lg"
          :ui="{
            icon: {
              trailing: {
                pointer: 'cursor-pointer'
              }
            }
          }"
        >
          <template #trailing>
            <UButton
              v-if="searchQuery"
              color="gray"
              variant="link"
              icon="i-lucide-x"
              :padded="false"
              @click="searchQuery = ''"
            />
          </template>
        </UInput>
      </div>
      <div class="flex space-x-3">
        <UButton
          icon="i-lucide-refresh-ccw"
          variant="outline"
          size="sm"
          label="Refresh"
          :loading="isRefreshing"
          @click="fetchUrls"
          class="cursor-pointer"
        />
        <UButton
          icon="i-lucide-plus"
          size="sm"
          label="Add New"
          @click="navigateTo('/link/add')"
          color="primary"
        />
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-1 lg:grid-cols-3 gap-4 mb-6">
      <UCard>
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">
              Total Links
            </p>
            <p class="text-2xl font-semibold">{{ data.length }}</p>
          </div>
          <UIcon name="i-lucide-link" class="w-8 h-8 text-primary-500" />
        </div>
      </UCard>

      <UCard>
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">
              Total Clicks
            </p>
            <p class="text-2xl font-semibold">{{ totalClicks }}</p>
            <p
              class="text-xs mt-1"
              :class="clicksGrowth >= 0 ? 'text-green-500' : 'text-red-500'"
            >
              {{ clicksGrowth >= 0 ? '↑' : '↓' }}
              {{ Math.abs(clicksGrowth).toFixed(1) }}%
            </p>
          </div>
          <UIcon
            name="i-lucide-mouse-pointer-click"
            class="w-8 h-8 text-blue-500"
          />
        </div>
      </UCard>

      <UCard v-if="topLinks.length > 0">
        <div>
          <p class="text-sm font-medium text-gray-500 dark:text-gray-400">
            Top Performing Link
          </p>
          <p class="text-sm font-medium truncate mt-1">
            {{ topLinks[0].shortUrl }}
          </p>
          <div class="flex items-center mt-2">
            <UIcon
              name="i-lucide-bar-chart-2"
              class="w-4 h-4 mr-2 text-yellow-500"
            />
            <span class="text-sm">{{ topLinks[0].nrOfClicks }} clicks</span>
          </div>
        </div>
      </UCard>
    </div>

    <!-- Main Table -->
    <div
      class="border rounded-lg overflow-hidden dark:border-gray-700 bg-white dark:bg-gray-800 shadow-sm flex-1"
    >
      <UTable
        :data="data"
        :columns="columns"
        :loading="isLoading"
        class="w-full"
        :ui="{
          wrapper: 'h-full flex flex-col',
          thead: 'sticky top-0 z-10',
          tbody: 'flex-1',
          td: { base: 'whitespace-nowrap', padding: 'py-3 px-4' },
          th: {
            base: 'text-left bg-gray-50 dark:bg-gray-700/50',
            padding: 'py-3 px-4'
          },
          loadingState: {
            icon: 'i-lucide-loader-2',
            label: 'Loading...',
            iconClass: 'animate-spin text-primary-500'
          }
        }"
      >
        <template #empty-state>
          <div
            class="flex flex-col items-center justify-center py-12 text-center"
          >
            <UIcon
              name="i-lucide-link"
              class="w-12 h-12 mx-auto text-gray-400 mb-4"
            />
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-1">
              No shortened URLs yet
            </h3>
            <p
              class="text-sm text-gray-500 dark:text-gray-400 max-w-md mx-auto"
            >
              Get started by creating your first short URL
            </p>
            <UButton
              icon="i-lucide-plus"
              label="Create First URL"
              @click="navigateTo('/links/new')"
              class="mt-4"
              color="primary"
            />
          </div>
        </template>
      </UTable>
    </div>

    <div v-if="!isSearching" class="flex items-center justify-between mt-4">
      <USelect v-model="value" :items="items" class="w-20" />
      <UPagination
        v-model:page="page"
        :total="totalPages * 10"
        @update:model-value="handlePageChange"
      />
    </div>
  </div>
</template>
