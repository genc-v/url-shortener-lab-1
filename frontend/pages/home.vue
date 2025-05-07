<template>
  <UModal v-model="isOpen" title="Modal with title">
    <template #body>
      <Placeholder class="h-48" />
    </template>
  </UModal>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const isOpen = ref(false)

// Open the modal when component mounts
onMounted(() => {
  isOpen.value = true
})

// You can also expose a function to open the modal from other components
const openModal = () => {
  isOpen.value = true
}

// Or close it
const closeModal = () => {
  isOpen.value = false
}
</script>

<style lang="scss"></style>
