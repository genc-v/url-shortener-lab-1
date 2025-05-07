<template>
  <div></div>
</template>

<script setup>
const token = useCookie('token')

console.log(token.value)

// onMounted(() => {
// const exp = localStorage.getItem('exp')

// const tokenExp = exp * 1000 < Date.now()
// console.log(tokenExp)
// const rt = localStorage.getItem('rtoken')
// api('User/slient-login', 'POST', rt).then((data) => {
//   console.log(data)
// })
// })
</script>

<style lang="scss"></style>
