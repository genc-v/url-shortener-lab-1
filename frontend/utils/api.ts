import { jwtDecode } from 'jwt-decode'

export const api = async (
  path: string,
  method: string = 'GET',
  body?: any,
  needsToken: boolean = true
) => {
  const toast: any = useToast()
  const config: any = useState('config')
  const headers: Record<string, string> = {
    'Content-Type': 'application/json'
  }

  if (needsToken) {
    const token = localStorage.getItem('token')
    const exp: any = localStorage.getItem('exp')

    if (!token || !exp) {
      toast.add({
        title: 'Error fetching data',
        description: 'Token not found',
        color: 'red'
      })
      return
    }
    const tokenExp = exp * 1000 < Date.now()
    if (tokenExp) {
      await fetchToken()
    }

    const validToken = localStorage.getItem('token')
    headers['Authorization'] = `Bearer ${validToken}`
  }

  const url = `${config.value.API}api/${path}`
  const options: RequestInit = {
    method,
    headers,
    body: method !== 'GET' && body ? JSON.stringify(body) : undefined
  }

  try {
    const response = await fetch(url, options)

    if (!response.ok) {
      let errorMessage = `Request failed with status ${response.status}`
      try {
        const errorData = await response.json()
        if (errorData?.message) {
          errorMessage = errorData.message
        }
      } catch (e: any) {
        console.error(e.message)
      }
      throw new Error(errorMessage)
    }

    if (response.status === 204) {
      return null
    }

    return await response.json()
  } catch (error: any) {
    toast.add({
      title: 'Error fetching data',
      description: error.message,
      color: 'red'
    })
    console.error(
      'Fetch error:',
      error instanceof Error ? error.message : 'Unknown error'
    )
    throw error
  }
}
const fetchToken = async () => {
  const config: any = useState('config')
  const toast: any = useToast()
  const url = `${config.value.API}api/User/silent-login`
  const token = localStorage.getItem('token')
  const rt = localStorage.getItem('rtoken')

  if (!token || !rt) {
    toast.add({
      title: 'Error fetching data',
      description: 'Token not found',
      color: 'red'
    })
    console.error('Token not found')
    return
  }

  try {
    const response = await fetch(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(rt)
    })

    const newToken = await response.json()

    localStorage.removeItem('token')
    localStorage.removeItem('exp')
    localStorage.setItem('token', newToken.token)
    const decoded: any = jwtDecode(newToken.token)
    localStorage.setItem('exp', decoded.exp)
    return
  } catch (error) {
    console.error('Fetch error:', error)
    toast.add({
      title: 'Silent login failed',
      color: 'red'
    })
  }
}
