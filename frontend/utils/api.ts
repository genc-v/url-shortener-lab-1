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
    const token = useCookie('token')
    const exp: any = useCookie('exp')

    if (!token.value == null || !exp.value == null) {
      toast.add({
        title: 'Error fetching data',
        description: 'Token not found',
        color: 'red'
      })
      return
    }

    const tokenExp = exp.value * 1000 < Date.now()
    if (tokenExp) {
      await fetchToken()
    }

    const validToken = useCookie('token')
    headers['Authorization'] = `Bearer ${validToken.value}`
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
  const token = useCookie('token')
  const rt = useCookie('refreshToken')

  if (!token.value || !rt.value) {
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
        Authorization: `Bearer ${token.value}`
      },
      body: JSON.stringify(rt.value)
    })

    const newToken = await response.json()

    const setToken = useCookie('token', {
      maxAge: 60 * 60 * 24 * 30,
      sameSite: 'lax',
      secure: true,
      path: '/'
    })
    setToken.value = newToken.token

    const decoded: any = jwtDecode(newToken.token)
    const exp = useCookie('exp')
    exp.value = decoded.exp
    return
  } catch (error) {
    console.error('Fetch error:', error)
    toast.add({
      title: 'Silent login failed',
      color: 'red'
    })
  }
}
