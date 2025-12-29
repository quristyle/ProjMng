<template>
  <router-link to="/" class="lg:hidden">
    <img class="dark:hidden w-[3rem]" src="/images/logo/logo-icon.png" alt="Logo" />
    <img class="hidden dark:block w-[3rem]" src="/images/logo/logo-icon.png" alt="Logo" />
  </router-link>
</template>

<script setup>
import { RouterLink } from 'vue-router'
</script>
