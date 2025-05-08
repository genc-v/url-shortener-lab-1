<template>
  <div class="min-h-screen"></div>
</template>

<script setup>
const router = useRouter()
const rtoken = useCookie('refreshToken')

api('User/Logout', 'DELETE', rtoken.value)
  .then(() => {
    const token = useCookie('token')
    token.value = null
    const rToken = useCookie('refreshToken')
    rToken.value = null
    const exp = useCookie('exp')
    exp.value = null
    const userId = useCookie('userId')
    userId.value = null
    router.replace('/login')
  })
  .catch((error) => {
    console.error(error)
  })
</script>
