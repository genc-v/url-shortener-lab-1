export default defineNuxtRouteMiddleware((to, from) => {
  const publicRoutes = ['/login', '/signup']

  if (publicRoutes.includes(to.path)) {
    return
  }

  if (process.client) {
    const token = localStorage.getItem('token')
    if (!token) {
      return navigateTo('/login')
    }
  }
})
