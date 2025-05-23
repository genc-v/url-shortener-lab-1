<template>
  <div class="h-full max-w-[1440px] m-auto px-5 flex flex-col space-y-4">
    <div class="flex justify-between items-center">
      <h2 class="text-xl font-semibold text-gray-900 dark:text-white">Logs</h2>
    </div>

    <div
      class="border rounded-lg overflow-hidden shadow-sm dark:border-gray-700 bg-white dark:bg-gray-800 flex-1"
    >
      <UTable
        :data="data"
        :columns="columns"
        :loading="isLoading"
        :ui="{
          wrapper: 'h-full flex flex-col',
          thead: 'sticky top-0 z-10',
          tbody: 'flex-1 min-h-[200px]',
          td: {
            base: 'whitespace-nowrap',
            padding: 'py-3 px-4',
            color: 'text-gray-700 dark:text-gray-300'
          },
          th: {
            base: 'text-left bg-gray-50 dark:bg-gray-700/50 font-medium text-gray-700 dark:text-gray-300',
            padding: 'py-3 px-4'
          },
          tr: {
            base: 'hover:bg-gray-50 dark:hover:bg-gray-700/50',
            active: 'bg-gray-100 dark:bg-gray-700/75'
          },
          loadingState: {
            icon: 'i-lucide-loader-2',
            label: 'Loading...',
            iconClass: 'animate-spin text-primary-500'
          }
        }"
        class="h-full"
      />
    </div>
  </div>
</template>
<script setup>
const isLoading = ref(true)
const data = ref([])

await api('logs', 'GET').then((result) => {
  data.value = result.slice().reverse()
  isLoading.value = false
})

const columns = [
  {
    accessorKey: 'id',
    header: 'ID',
    cell: ({ row }) => `#${row.getValue('id')}`
  },
  {
    accessorKey: 'note',
    header: 'Note'
  },
  {
    accessorKey: 'createdAt',
    header: 'Created At',
    cell: ({ row }) => timeAgo(row.getValue('createdAt'))
  }
]
useHead({
  title: 'App logs'
})
</script>
