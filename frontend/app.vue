<template>
  <UApp>
    <div class="flex min-h-screen flex-col">
      <Nav v-if="isLoggedIn" />
      <NuxtPage class="w-full flex-grow" />
      <Footer v-if="isLoggedIn" />
    </div>
  </UApp>
</template>

<script setup>
const runtimeConfig = useRuntimeConfig()
const router = useRouter()
const route = useRoute()
const _config = useState('config', () => runtimeConfig.public)
const isLoggedIn = ref('loggedIn', () => false)

const token = useCookie('byToken')
if (!token.value) {
  isLoggedIn.value = false
  router.push('/login')
}

watch(route, () => {
  const token = useCookie('byToken')

  if (!token.value) {
    isLoggedIn.value = false
  } else {
    isLoggedIn.value = true
  }
})
</script>
