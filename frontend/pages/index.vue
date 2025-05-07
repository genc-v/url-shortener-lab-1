<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Header -->
    <div class="max-w-7xl mx-auto shadow-sm px-4 sm:px-6 pb-0 lg:px-8 py-8">
      <div
        class="mx-auto sm:px-6 lg:px-8 bg-white dark:bg-gray-800 py-6 rounded-lg"
      >
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
            to="/links/add"
            color="primary"
          />
        </div>
      </div>
    </div>

    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Stats Overview -->
      <div class="grid grid-cols-1 gap-6 sm:grid-cols-1 lg:grid-cols-3 mb-8">
        <template v-for="stat in stats" :key="stat.name">
          <UCard>
            <div class="flex items-center justify-between">
              <div>
                <p
                  class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate"
                >
                  {{ stat.name }}
                </p>
                <p
                  class="mt-1 text-2xl font-semibold text-gray-900 dark:text-white"
                >
                  {{ stat.value }}
                </p>
                <p
                  v-if="stat.change"
                  class="mt-1 flex items-baseline text-sm font-medium"
                  :class="
                    stat.changeType === 'positive'
                      ? 'text-green-600 dark:text-green-400'
                      : 'text-red-600 dark:text-red-400'
                  "
                >
                  {{ stat.change }}
                  <UIcon
                    :name="
                      stat.changeType === 'positive'
                        ? 'i-heroicons-arrow-trending-up'
                        : 'i-heroicons-arrow-trending-down'
                    "
                    class="ml-1 h-4 w-4 flex-shrink-0"
                  />
                </p>
                <p
                  v-if="stat.clicks"
                  class="mt-1 text-sm text-gray-500 dark:text-gray-400"
                >
                  {{ stat.clicks }} clicks
                </p>
              </div>
              <div
                class="bg-primary-500/10 dark:bg-primary-400/10 p-3 rounded-lg"
              >
                <UIcon
                  v-if="stat.name.includes('Link')"
                  name="i-heroicons-link"
                  class="h-6 w-6 text-primary-500 dark:text-primary-400"
                />
                <UIcon
                  v-else-if="stat.name.includes('Click')"
                  name="i-heroicons-cursor-arrow-rays"
                  class="h-6 w-6 text-primary-500 dark:text-primary-400"
                />
                <UIcon
                  v-else-if="stat.name.includes('QR')"
                  name="i-heroicons-qr-code"
                  class="h-6 w-6 text-primary-500 dark:text-primary-400"
                />
              </div>
            </div>
          </UCard>
        </template>
      </div>

      <!-- Main Content Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Left Column -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Quick Actions -->
          <UCard>
            <template #header>
              <div class="flex items-center justify-between">
                <h2 class="text-lg font-semibold text-gray-900 dark:text-white">
                  Quick Actions
                </h2>
              </div>
            </template>
            <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
              <UButton
                v-for="action in quickActions"
                :key="action.name"
                :icon="action.icon"
                :label="action.name"
                :to="action.to"
                variant="outline"
                color="gray"
                class="flex-col h-full py-4"
                :ui="{ base: 'flex items-center justify-center' }"
              >
                <template #leading>
                  <UIcon :name="action.icon" class="w-6 h-6 mb-2" />
                </template>
              </UButton>
            </div>
          </UCard>

          <UCard>
            <template #header>
              <div class="flex items-center justify-between">
                <h2 class="text-lg font-semibold text-gray-900 dark:text-white">
                  Recent Links
                </h2>
                <UButton
                  label="View All"
                  to="/linkF/all"
                  variant="link"
                  color="primary"
                  size="sm"
                />
              </div>
            </template>
            <UTable
              :rows="recentLinks"
              :columns="[
                {
                  key: 'url',
                  label: 'Short URL',
                  id: 'col1'
                },
                {
                  key: 'destination',
                  label: 'Destination',
                  id: 'col2'
                },
                {
                  key: 'clicks',
                  label: 'Clicks',
                  id: 'col3'
                },
                {
                  key: 'created',
                  label: 'Created',
                  id: 'col4'
                }
              ]"
              :ui="{
                td: { base: 'whitespace-nowrap' },
                th: { base: 'text-left bg-gray-50 dark:bg-gray-700/50' }
              }"
            >
              <template #url-data="{ row }">
                <div class="flex items-center">
                  <UIcon
                    name="i-heroicons-link"
                    class="w-4 h-4 mr-2 text-gray-400"
                  />
                  <span class="font-medium">{{ row.url }}</span>
                </div>
              </template>
              <template #destination-data="{ row }">
                <span
                  class="text-gray-500 dark:text-gray-400 truncate max-w-[160px]"
                  >{{ row.destination }}</span
                >
              </template>
              <template #clicks-data="{ row }">
                <UBadge :label="row.clicks" color="gray" variant="subtle" />
              </template>
            </UTable>
          </UCard>
        </div>

        <!-- Right Column -->
        <div class="space-y-6">
          <!-- Clicks Chart -->

          <!-- Top Performing Links -->
          <UCard>
            <template #header>
              <div class="flex items-center justify-between">
                <h2 class="text-lg font-semibold text-gray-900 dark:text-white">
                  Top Links
                </h2>
                <UButton
                  label="View All"
                  to="/analytics"
                  variant="link"
                  color="primary"
                  size="sm"
                />
              </div>
            </template>
            <ul class="divide-y divide-gray-200 dark:divide-gray-700">
              <li
                v-for="(link, index) in recentLinks"
                :key="index"
                class="py-3"
              >
                <div class="flex items-center justify-between">
                  <div>
                    <p
                      class="text-sm font-medium text-gray-900 dark:text-white"
                    >
                      {{ link.url }}
                    </p>
                    <p
                      class="text-xs text-gray-500 dark:text-gray-400 truncate"
                    >
                      {{ link.destination }}
                    </p>
                  </div>
                  <UBadge
                    :label="`${link.clicks} clicks`"
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
const stats = ref([
  {
    name: 'Total Links',
    value: '1,248',
    change: '+12.5%',
    changeType: 'positive'
  },
  {
    name: 'Total Clicks',
    value: '24,901',
    change: '+8.2%',
    changeType: 'positive'
  },
  { name: 'Top Link', value: 'localhost:5001/sale', clicks: '3,421' }
])

const recentLinks = ref([
  {
    id: 1,
    url: 'localhost:5001/marketing',
    destination: 'example.com/campaign',
    clicks: 421,
    created: '2 hours ago'
  },
  {
    id: 2,
    url: 'localhost:5001/product',
    destination: 'example.com/new-product',
    clicks: 198,
    created: '5 hours ago'
  },
  {
    id: 3,
    url: 'localhost:5001/support',
    destination: 'example.com/help-center',
    clicks: 156,
    created: '1 day ago'
  }
])
const quickActions = [
  { icon: 'i-heroicons-plus-circle', name: 'Create Link', to: '/links/add' },
  {
    icon: 'i-heroicons-qr-code',
    name: 'Generate QR',
    to: '/qr-codes/generator'
  },

  {
    icon: 'i-heroicons-document-text',
    name: 'Generate Report',
    to: '/analytics/reports'
  }
]
</script>
