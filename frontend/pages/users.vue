<template>
  <div class="flex flex-col h-full max-w-[1440px] p-5 mx-auto">
    <h1 class="text-center">Users</h1>

    <!-- Delete Modal -->
    <transition name="fade" mode="out-in">
      <div
        v-if="isDeleteModalOpen"
        class="fixed inset-0 flex items-center justify-center bg-[#00000073] z-50 backdrop-blur-sm"
      >
        <div
          class="bg-white dark:bg-gray-800 rounded-lg shadow-lg p-6 w-[90%] max-w-md relative"
        >
          <button
            @click="closeDeleteModal"
            class="absolute top-3 right-3 text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200"
          >
            <UIcon name="i-lucide-x" class="w-5 h-5" />
          </button>
          <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
            Confirm User Deletion
          </h2>
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-6">
            Are you sure you want to delete user
            <span class="font-semibold">{{ itemToDelete?.name }}</span
            >? This action cannot be undone.
          </p>
          <div class="flex justify-end space-x-3">
            <button
              @click="closeDeleteModal"
              class="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-200 rounded hover:bg-gray-300 dark:bg-gray-700 dark:text-gray-300 dark:hover:bg-gray-600"
            >
              Cancel
            </button>
            <button
              @click="confirmDelete"
              :disabled="isDeleting"
              class="px-4 py-2 text-sm font-medium text-white bg-red-600 rounded hover:bg-red-700"
            >
              <span v-if="isDeleting" class="animate-spin mr-2">⏳</span>
              Delete
            </button>
          </div>
        </div>
      </div>
    </transition>

    <!-- Edit Modal -->
    <transition name="fade" mode="out-in">
      <div
        v-if="isEditModalOpen"
        class="fixed inset-0 flex items-center justify-center bg-[#00000073] z-50 backdrop-blur-sm"
      >
        <div
          class="bg-white dark:bg-gray-800 rounded-lg shadow-lg p-6 w-[90%] max-w-md relative"
        >
          <button
            @click="closeEditModal"
            class="absolute top-3 right-3 text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200"
          >
            <UIcon name="i-lucide-x" class="w-5 h-5" />
          </button>
          <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
            Edit User
          </h2>
          <form @submit.prevent="submitEdit">
            <div class="space-y-4">
              <UFormGroup label="Name" name="name">
                <UInput
                  v-model="editForm.name"
                  placeholder="Enter user name"
                  required
                />
              </UFormGroup>

              <UFormGroup label="Email" name="email">
                <UInput
                  v-model="editForm.email"
                  type="email"
                  placeholder="Enter email"
                  required
                />
              </UFormGroup>

              <UFormGroup label="Password" name="password">
                <UInput
                  v-model="editForm.password"
                  type="password"
                  placeholder="Leave blank to keep current"
                />
              </UFormGroup>
            </div>

            <div class="flex justify-end space-x-3 mt-6">
              <UButton
                type="button"
                @click="closeEditModal"
                color="gray"
                variant="ghost"
              >
                Cancel
              </UButton>
              <UButton type="submit" :disabled="isEditing" :loading="isEditing">
                Save Changes
              </UButton>
            </div>
          </form>
        </div>
      </div>
    </transition>

    <!-- Main Table -->
    <div
      class="border rounded-lg overflow-hidden dark:border-gray-700 bg-white dark:bg-gray-800 shadow-sm flex-1"
    >
      <UTable
        :data="data"
        :columns="columns"
        :loading="isLoading"
        class="w-full"
        :ui="{
          wrapper: 'h-full flex flex-col',
          thead: 'sticky top-0 z-10',
          tbody: 'flex-1',
          td: { base: 'whitespace-nowrap', padding: 'py-3 px-4' },
          th: {
            base: 'text-left bg-gray-50 dark:bg-gray-700/50',
            padding: 'py-3 px-4'
          },
          loadingState: {
            icon: 'i-lucide-loader-2',
            label: 'Loading...',
            iconClass: 'animate-spin text-primary-500'
          }
        }"
      >
        <template #empty-state>
          <div
            class="flex flex-col items-center justify-center py-12 text-center"
          >
            <UIcon
              name="i-lucide-users"
              class="w-12 h-12 mx-auto text-gray-400 mb-4"
            />
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-1">
              No users found
            </h3>
            <p
              class="text-sm text-gray-500 dark:text-gray-400 max-w-md mx-auto"
            >
              Get started by creating your first user
            </p>
            <UButton
              icon="i-lucide-plus"
              label="Create User"
              @click="navigateTo('/users/new')"
              class="mt-4"
              color="primary"
            />
          </div>
        </template>
      </UTable>
    </div>
  </div>
</template>

<script setup>
const UButton = resolveComponent('UButton')
const UDropdownMenu = resolveComponent('UDropdownMenu')
const UTable = resolveComponent('UTable')
const UFormGroup = resolveComponent('UFormGroup')
const UInput = resolveComponent('UInput')

// Composables
const toast = useToast()

// State
const data = ref([])
const isLoading = ref(true)
const isDeleteModalOpen = ref(false)
const itemToDelete = ref(null)
const isDeleting = ref(false)

// Edit Modal State
const isEditModalOpen = ref(false)
const isEditing = ref(false)
const editForm = ref({
  id: null,
  name: '',
  email: '',
  password: ''
})

// Fetch users
const fetchUsers = async () => {
  try {
    isLoading.value = true
    const response = await api('User', 'GET')
    data.value = response.map((user) => ({
      id: user.id,
      email: user.email,
      name: user.fullName,
      createdAt: user.createdAt // Assuming this field exists
    }))
  } catch (error) {
    console.error('User fetch failed:', error)
    toast.add({
      title: 'Failed to load users',
      description: error.message || 'Please try again later',
      color: 'error',
      icon: 'i-lucide-alert-circle',
      timeout: 5000
    })
  } finally {
    isLoading.value = false
  }
}

await fetchUsers()

const formatDateTime = (dateString) => {
  if (!dateString) return 'N/A'
  return new Date(dateString).toLocaleString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    hour12: false
  })
}

// Delete functionality
const confirmDelete = async () => {
  if (!itemToDelete.value) return

  isDeleting.value = true
  try {
    await api(`User?id=${itemToDelete.value.id}`, 'DELETE')
    toast.add({
      title: 'User deleted',
      description: `${itemToDelete.value.name} was removed`,
      color: 'success',
      icon: 'i-lucide-trash-2'
    })
    await fetchUsers()
  } catch (error) {
    await fetchUsers()
  } finally {
    isDeleting.value = false
    isDeleteModalOpen.value = false
    itemToDelete.value = null
  }
}

const closeDeleteModal = () => {
  isDeleteModalOpen.value = false
  itemToDelete.value = null
}

// Edit functionality
const openEditModal = (user) => {
  editForm.value = {
    id: user.id,
    name: user.name,
    email: user.email,
    password: ''
  }
  isEditModalOpen.value = true
}

const closeEditModal = () => {
  isEditModalOpen.value = false
  editForm.value = {
    id: user.id,
    name: '',
    email: '',
    password: ''
  }
}

const submitEdit = async () => {
  if (!editForm.value.id) return

  isEditing.value = true
  try {
    const payload = {
      email: editForm.value.email,
      fullName: editForm.value.name,
      oldPassword: null,
      newPassword: null
    }

    if (editForm.value.password) {
      payload.password = editForm.value.password
    }

    await api('User/' + userId, 'PUT', payload)
    toast.add({
      title: 'User updated',
      description: 'User details were successfully updated',
      color: 'success',
      icon: 'i-lucide-check-circle'
    })
    await fetchUsers()
    closeEditModal()
  } catch (error) {
    toast.add({
      title: 'Update failed',
      description: error.message || 'Failed to update user',
      color: 'error',
      icon: 'i-lucide-alert-circle'
    })
  } finally {
    isEditing.value = false
  }
}

// Table configuration
const columns = [
  {
    accessorKey: 'id',
    header: 'ID',
    cell: ({ row }) => `#${row.getValue('id')}`,
    meta: { width: '80px', align: 'center' }
  },
  {
    accessorKey: 'name',
    header: 'Name',
    meta: { minWidth: '150px' }
  },
  {
    accessorKey: 'email',
    header: 'Email',
    meta: { minWidth: '200px', truncate: true }
  },
  {
    accessorKey: 'createdAt',
    header: 'Created At',
    cell: ({ row }) => formatDateTime(row.getValue('createdAt')),
    meta: { width: '180px' }
  },
  {
    id: 'actions',
    header: 'Actions',
    meta: { width: '60px', align: 'center' },
    cell: ({ row }) =>
      h(
        'div',
        { class: 'flex justify-end' },
        h(
          UDropdownMenu,
          {
            content: { align: 'end', class: 'min-w-[180px] shadow-lg' },
            items: [
              {
                type: 'label',
                label: 'User Management',
                class:
                  'font-semibold text-xs uppercase tracking-wider text-gray-500 dark:text-gray-400'
              },
              {
                label: 'Edit User',
                icon: 'i-lucide-pencil',
                onSelect: () => openEditModal(row.original)
              },
              { type: 'separator' },
              {
                label: 'Delete',
                icon: 'i-lucide-trash-2',
                class:
                  'text-red-500 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20',
                onSelect: () => {
                  itemToDelete.value = row.original
                  isDeleteModalOpen.value = true
                }
              }
            ]
          },
          () =>
            h(UButton, {
              icon: 'i-lucide-ellipsis-vertical',
              color: 'neutral',
              variant: 'ghost',
              class: 'h-8 w-8 p-0 hover:bg-gray-100 dark:hover:bg-gray-700'
            })
        )
      )
  }
]

// Watch for escape key to close modals
watch([isDeleteModalOpen, isEditModalOpen], ([deleteOpen, editOpen]) => {
  const handler = (event) => {
    if (event.key === 'Escape') {
      if (deleteOpen) closeDeleteModal()
      if (editOpen) closeEditModal()
    }
  }

  if (deleteOpen || editOpen) {
    document.addEventListener('keydown', handler)
  } else {
    document.removeEventListener('keydown', handler)
  }
})
</script>
