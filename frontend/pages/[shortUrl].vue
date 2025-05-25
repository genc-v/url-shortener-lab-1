<template>
  <div class="min-h-screen"></div>
</template>

<script setup>
const route = useRoute()

onMounted(async () => {
  await api(route.params.shortUrl, 'GET', {}, false).then((res) => {
    navigateTo(res.url, { external: true })
  })
})

useHead({
  title: route.params.shortUrl
})
</script>
