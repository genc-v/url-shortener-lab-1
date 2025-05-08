<template>
  <div class="min-h-screen"></div>
</template>

<script setup>
const route = useRoute()
const router = useRouter()

await api(route.params.shortUrl, 'GET', {}, false)
  .then((res) => {
    // send it to the site of the res
    navigateTo(res.url, { external: true })
  })
  .catch((err) => {
    console.error('Error:', err)
  })
</script>
