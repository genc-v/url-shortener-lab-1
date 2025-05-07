<template>
  <UApp>
    <Nav v-if="isLoggedIn" />
    <NuxtPage />
    <Footer v-if="isLoggedIn" />
  </UApp>
</template>

<script setup>
const runtimeConfig = useRuntimeConfig()
const router = useRouter()
const route = useRoute()
const _config = useState('config', () => runtimeConfig.public)
const { ui } = useUi()
const isLoggedIn = ref('loggedIn', () => true)

onMounted(() => {
  const token = localStorage.getItem('token')
  if (!token) {
    isLoggedIn.value = false
    router.push('/login')
  }
})

watch(route, () => {
  const token = localStorage.getItem('token')

  if (!token) {
    isLoggedIn.value = false
  } else {
    isLoggedIn.value = true
  }
})

useHead({
  htmlAttrs: {
    class: computed(() => {
      return [`primary-${ui.value.primary}`, `gray-${ui.value.gray}`].join(' ')
    })
  }
})
</script>
