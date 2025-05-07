<template>
  <div></div>
</template>

<script setup>
const router = useRouter()
onMounted(() => {
  const rtoken = localStorage.getItem('rtoken')
  api('User/Logout', 'DELETE', rtoken)
    .then(() => {
      localStorage.removeItem('token')
      localStorage.removeItem('rtoken')
      localStorage.removeItem('exp')
      router.replace('/login')
    })
    .catch((error) => {
      console.error(error)
    })
})
</script>
