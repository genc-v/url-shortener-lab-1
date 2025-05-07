<template>
  <UModal v-model="isOpen" title="Modal with title">
    <template #body>
      <Placeholder class="h-48" />
    </template>
  </UModal>
</template>

<script setup>
const isOpen = ref(false)

onMounted(() => {
  isOpen.value = true
})
</script>
