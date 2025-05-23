<script setup lang="ts">
import type { NavigationMenuItem } from '@nuxt/ui'

const isAdmin = useCookie('isAdmin')

const items = ref<NavigationMenuItem[]>([
  {
    label: 'Home',
    icon: 'i-heroicons-home',
    to: '/dashboard',
    description: 'Return to the dashboard overview'
  },
  {
    label: 'Links',
    icon: 'i-heroicons-link',
    description: 'Manage your shortened links',
    children: [
      {
        label: 'All Links',
        icon: 'i-heroicons-list-bullet',
        to: '/link/all',
        description: 'View all your shortened links'
      },
      {
        label: 'Create New',
        icon: 'i-heroicons-plus-circle',
        to: '/link/add',
        description: 'Create a new shortened link'
      }
    ]
  },
  {
    label: 'Settings',
    icon: 'i-heroicons-cog-6-tooth',
    description: 'Configure your account and preferences',
    children: [
      {
        label: 'Account Settings',
        icon: 'i-heroicons-user-circle',
        to: '/settings',
        description: 'Manage your account preferences'
      },
      ...(isAdmin.value
        ? [
            {
              label: 'User Management',
              icon: 'i-heroicons-users',
              to: '/users',
              description: 'Manage system users'
            },
            {
              label: 'System Logs',
              icon: 'i-heroicons-clipboard-document-list',
              to: '/logs',
              description: 'View system activity logs'
            }
          ]
        : []),
      {
        label: 'Logout',
        icon: 'i-heroicons-arrow-left-on-rectangle',
        to: '/logout',
        description: 'Sign out of your account'
      }
    ]
  }
])
</script>

<template>
  <UNavigationMenu
    variant="pill"
    content-orientation="vertical"
    :items="items"
    class="z-50 justify-center"
  />
</template>
