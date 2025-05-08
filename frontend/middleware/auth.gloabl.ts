export default defineNuxtRouteMiddleware((to) => {
  const publicPaths = ['/login', '/signup']

  if (
    publicPaths.some(
      (path) => to.path === path || to.path.startsWith(`${path}/`)
    )
  ) {
    return
  }

  const token = useCookie('token')
  if (!token.value) return navigateTo('/login')
})
