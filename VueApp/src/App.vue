<script setup lang="ts">
import { RouterView } from 'vue-router';
import NavMenu from "@/components/NavMenu.vue";
</script>

<template>
  <header>
    <NavMenu />
  </header>

  <RouterView />
</template>
