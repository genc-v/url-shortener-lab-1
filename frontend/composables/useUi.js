export const useUi = () => {
  const ui = useState('ui', () => ({
    primary: 'primary',
    gray: 'gray',
    button: {
      rounded: 'rounded-lg',
      default: {
        color: 'white'
      }
    },
    icons: {
      dynamic: true
    }
  }))

  const set = (config) => {
    ui.value = {
      ...ui.value,
      ...config
    }
  }

  return {
    ui: readonly(ui),
    set
  }
}
