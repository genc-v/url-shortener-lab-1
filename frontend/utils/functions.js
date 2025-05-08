export function debounce(func, delay) {
  let timer
  return function (...args) {
    clearTimeout(timer)
    timer = setTimeout(() => func.apply(this, args), delay)
  }
}
export function timeAgo(dateString) {
  const now = new Date()
  const date = new Date(dateString)
  const secondsAgo = Math.floor((now - date) / 1000)

  if (secondsAgo < 60) {
    return `${secondsAgo} seconds ago`
  } else if (secondsAgo < 3600) {
    const minutesAgo = Math.floor(secondsAgo / 60)
    return `${minutesAgo} minutes ago`
  } else if (secondsAgo < 86400) {
    const hoursAgo = Math.floor(secondsAgo / 3600)
    return `${hoursAgo} hours ago`
  } else {
    const daysAgo = Math.floor(secondsAgo / 86400)
    return `${daysAgo} days ago`
  }
}
