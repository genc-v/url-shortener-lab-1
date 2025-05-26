<template>
  <div class="min-h-screen"></div>
</template>

<script setup>
const router = useRouter()
const rtoken = useCookie('byRefreshToken')

await api('User/Logout', 'DELETE', rtoken.value)
  .then(() => {
    const token = useCookie('byToken')
    token.value = null
    const rToken = useCookie('byRefreshToken')
    rToken.value = null
    const exp = useCookie('exp')
    exp.value = null
    const userId = useCookie('userId')
    userId.value = null
    const isAdmin = useCookie('isAdmin')
    isAdmin.value = null
    router.replace('/login')
  })
  .catch((error) => {
    console.error(error)
    const token = useCookie('byToken')
    token.value = null
    const rToken = useCookie('byRefreshToken')
    rToken.value = null
    const exp = useCookie('exp')
    exp.value = null
    const userId = useCookie('userId')
    userId.value = null
    const isAdmin = useCookie('isAdmin')
    isAdmin.value = null
    router.replace('/login')
  })
</script>
