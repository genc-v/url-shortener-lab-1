<template>
  <div
    class="min-h-screen px-5 max-w-[1440px] m-auto bg-gray-50 dark:bg-gray-900"
  >
    <!-- Header -->
    <div class="w-full mx-auto shadow-sm pb-0 pt-8">
      <div class="mx-auto px-6 bg-white dark:bg-gray-800 py-6 rounded-lg">
        <div class="flex justify-between items-center">
          <div>
            <h1 class="text-3xl font-bold text-gray-900 dark:text-white">
              Dashboard
            </h1>
            <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
              Welcome back! Here's what's happening with your links.
            </p>
          </div>
          <UButton
            icon="i-heroicons-plus"
            label="Create New"
            to="/link/add"
            color="primary"
          />
        </div>
      </div>
    </div>

    <main class="w-full pt-5">
      <Widgets />
      <!-- Main Content Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-5">
        <!-- Left Column -->
        <div class="lg:col-span-2 space-y-6">
          <UCard>
            <template #header>
              <div class="flex items-center justify-between">
                <h2 class="text-lg font-semibold text-gray-900 dark:text-white">
                  Recent Links
                </h2>
                <UButton
                  label="View All"
                  to="/link/all"
                  variant="link"
                  color="primary"
                  size="sm"
                />
              </div>
            </template>
            <UTable
              :data="recentLinks"
              :ui="{
                td: { base: 'whitespace-nowrap' },
                th: { base: 'text-left bg-gray-50 dark:bg-gray-700/50' }
              }"
            />
          </UCard>
        </div>

        <!-- Right Column -->
        <div class="space-y-6">
          <!-- Top Performing Links -->
          <UCard>
            <template #header>
              <div class="flex items-center justify-between">
                <h2 class="text-lg font-semibold text-gray-900 dark:text-white">
                  Top Links
                </h2>
                <UButton
                  label="View All"
                  to="/link/all"
                  variant="link"
                  color="primary"
                  size="sm"
                />
              </div>
            </template>
            <ul class="divide-y divide-gray-200 dark:divide-gray-700">
              <li v-for="(link, index) in topLinks" :key="index" class="py-3">
                <div class="flex items-center justify-between">
                  <div>
                    <p
                      class="text-sm font-medium text-gray-900 dark:text-white"
                    >
                      {{ link.shortUrl }}
                    </p>
                    <p
                      class="text-xs text-gray-500 dark:text-gray-400 truncate"
                    >
                      {{ link.originalUrl }}
                    </p>
                  </div>
                  <UBadge
                    :label="`${link.nrOfClicks} clicks`"
                    color="gray"
                    variant="subtle"
                  />
                </div>
              </li>
            </ul>
          </UCard>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
const token = useCookie('token')
const router = useRouter()
const isLoading = ref(true)
const analyticsData = ref(null)

if (!token.value) {
  await router.push('/login')
} else {
  await loadDashboardData()
}

async function loadDashboardData() {
  try {
    const response = await api('URL/analytics', 'GET')
    analyticsData.value = response
  } catch (error) {
    console.error('Error fetching analytics data:', error)
  } finally {
    isLoading.value = false
  }
}

const recentLinks = computed(() => {
  return (
    analyticsData.value?.recentLinks?.map((link) => ({
      Url: link.originalUrl,
      ShortUrl: link.shortUrl,
      Date: timeAgo(link.dateCreated)
    })) || []
  )
})

const topLinks = computed(() => analyticsData.value?.topLinks || [])

useHead({
  title: 'Dashboard'
})
</script>

<style scoped>
:deep(tbody td) {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  max-width: 200px;
}

:deep(li div p) {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  max-width: 200px;
}
</style>
