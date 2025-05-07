<script setup>
const currentYear = new Date().getFullYear()
</script>

<template>
  <footer
    class="border-t border-gray-200 dark:border-gray-800 bg-white/50 dark:bg-gray-900/50 backdrop-blur-lg"
  >
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="py-12 grid grid-cols-1 md:grid-cols-4 gap-8">
        <!-- Logo and description -->
        <div class="space-y-4">
          <div class="flex items-center space-x-2">
            <UIcon name="i-heroicons-link" class="w-8 h-8 text-primary-500" />
            <span class="text-xl font-bold text-gray-900 dark:text-white"
              >Bytely</span
            >
          </div>
          <p class="text-sm text-gray-500 dark:text-gray-400">
            Powerful URL shortening and link management for modern businesses.
          </p>
          <div class="flex space-x-4">
            <UButton
              to="https://twitter.com"
              target="_blank"
              color="gray"
              variant="ghost"
              icon="i-heroicons-globe-alt"
              aria-label="Website"
            />
            <UButton
              to="https://twitter.com"
              target="_blank"
              color="gray"
              variant="ghost"
              icon="i-heroicons-twitter"
              aria-label="Twitter"
            />
            <UButton
              to="https://github.com"
              target="_blank"
              color="gray"
              variant="ghost"
              icon="i-heroicons-github"
              aria-label="GitHub"
            />
          </div>
        </div>

        <!-- Links -->
        <div>
          <h3
            class="text-sm font-semibold text-gray-900 dark:text-white tracking-wider uppercase"
          >
            Links
          </h3>
          <ul class="mt-4 space-y-2">
            <li>
              <UButton
                to="/links/all"
                variant="link"
                color="gray"
                label="All Links"
                class="px-0"
              />
            </li>
            <li>
              <UButton
                to="/links/add"
                variant="link"
                color="gray"
                label="Create New"
                class="px-0"
              />
            </li>
            <li>
              <UButton
                to="/analytics"
                variant="link"
                color="gray"
                label="Analytics"
                class="px-0"
              />
            </li>
          </ul>
        </div>

        <!-- Resources -->
        <div>
          <h3
            class="text-sm font-semibold text-gray-900 dark:text-white tracking-wider uppercase"
          >
            Resources
          </h3>
          <ul class="mt-4 space-y-2">
            <li>
              <UButton
                to="/docs"
                variant="link"
                color="gray"
                label="Documentation"
                class="px-0"
              />
            </li>
            <li>
              <UButton
                to="/api"
                variant="link"
                color="gray"
                label="API Reference"
                class="px-0"
              />
            </li>
            <li>
              <UButton
                to="/blog"
                variant="link"
                color="gray"
                label="Blog"
                class="px-0"
              />
            </li>
          </ul>
        </div>

        <!-- Legal -->
        <div>
          <h3
            class="text-sm font-semibold text-gray-900 dark:text-white tracking-wider uppercase"
          >
            Legal
          </h3>
          <ul class="mt-4 space-y-2">
            <li>
              <UButton
                to="/privacy"
                variant="link"
                color="gray"
                label="Privacy Policy"
                class="px-0"
              />
            </li>
            <li>
              <UButton
                to="/terms"
                variant="link"
                color="gray"
                label="Terms of Service"
                class="px-0"
              />
            </li>
            <li>
              <UButton
                to="/contact"
                variant="link"
                color="gray"
                label="Contact Us"
                class="px-0"
              />
            </li>
          </ul>
        </div>
      </div>

      <div
        class="py-8 border-t border-gray-200 dark:border-gray-800 flex flex-col md:flex-row justify-between items-center"
      >
        <p class="text-sm text-gray-500 dark:text-gray-400">
          &copy; {{ currentYear }} Bytely. All rights reserved.
        </p>
        <div class="mt-4 md:mt-0 flex space-x-6">
          <UButton
            to="/changelog"
            variant="link"
            color="gray"
            size="sm"
            label="Changelog"
          />
          <UButton
            to="/status"
            variant="link"
            color="gray"
            size="sm"
            label="Status"
          />
        </div>
      </div>
    </div>
  </footer>
</template>
