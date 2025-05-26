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
    const token = useCookie('byToken')
    const exp = useCookie('exp')

    if (!token.value || !exp.value) {
      toast.add({
        title: 'Error fetching data',
        description: 'Token not found',
        color: 'red'
      })
      return
    }

    const tokenExp = Number(exp.value) * 1000 < Date.now()
    if (tokenExp) {
      await fetchToken()
    }

    const validToken = useCookie('byToken')
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
  const token = useCookie('byToken')
  const rt = useCookie('byRefreshToken', {
    maxAge: 60 * 60 * 24 * 30,
    sameSite: 'lax',
    secure: true,
    path: '/'
  })

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

    const newTokens = await response.json()

    if (!newTokens.token) {
      throw new Error('Invalid token response')
    }

    const setToken = useCookie('byToken', {
      maxAge: 60 * 60 * 24 * 30,
      sameSite: 'lax',
      secure: true,
      path: '/'
    })
    setToken.value = newTokens.token

    const decoded: any = await jwtDecode(newTokens.token)
    const setExp = useCookie('exp')
    setExp.value = decoded.exp.toString()

    return
  } catch (error) {
    console.error(error)
  }
}
