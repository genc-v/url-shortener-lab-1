<template>
  <!-- Stats Cards -->
  <div class="grid grid-cols-1 sm:grid-cols-1 lg:grid-cols-3 gap-4 mb-6">
    <UCard>
      <div class="flex items-center justify-between">
        <div>
          <p class="text-sm font-medium text-gray-500 dark:text-gray-400">
            Total Links
          </p>
          <p class="text-2xl font-semibold">{{ analyticsData.totalUrls }}</p>
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
          ></p>
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
</template>
<script setup>
const analyticsData = useState('analyticsData')

const totalClicks = analyticsData.value.totalClicks

const topLinks = analyticsData.value.topLinks
</script>
