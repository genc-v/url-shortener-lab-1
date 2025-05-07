<script setup lang="ts">
import type { NavigationMenuItem } from '@nuxt/ui'

const items = ref<NavigationMenuItem[]>([
  {
    label: 'Home',
    icon: 'i-heroicons-home',
    to: '/',
    description: 'Return to the dashboard overview'
  },
  {
    label: 'Links',
    icon: 'i-heroicons-link',
    to: '/link/all',
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
    label: 'Analytics',
    icon: 'i-heroicons-chart-bar',
    to: '/analytics',
    description: 'View performance statistics',
    children: [
      {
        label: 'Overview',
        icon: 'i-heroicons-chart-pie',
        to: '/analytics',
        description: 'Summary of all analytics'
      },
      {
        label: 'Link Analytics',
        icon: 'i-heroicons-arrow-trending-up',
        to: '/analytics',
        description: 'Detailed link performance'
      },
      {
        label: 'QR Code Analytics',
        icon: 'i-heroicons-qr-code',
        to: '/analytics',
        description: 'QR code scan statistics'
      },
      {
        label: 'Reports',
        icon: 'i-heroicons-document-chart-bar',
        to: '/analytics',
        description: 'Generate custom reports'
      }
    ]
  },
  {
    label: 'Settings',
    icon: 'i-heroicons-cog-6-tooth',
    to: '/settings',
    description: 'Configure your account and preferences'
  },
  {
    label: 'Logout',
    icon: 'i-heroicons-arrow-left-on-rectangle',
    to: '/logout',
    description: 'Sign out of your account'
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
