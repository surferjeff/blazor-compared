<script setup lang="ts">
import { RouterView } from 'vue-router';
import NavMenu from "@/components/NavMenu.vue";
</script>

<template>
  <div class="page">
    <div class="sidebar">
      <NavMenu />
    </div>

    <main>
      <div class="top-row px-4">
        <a href="/about">About</a>
      </div>

      <article class="content px-4">
        <RouterView />
      </article>
    </main>
  </div>
</template>
