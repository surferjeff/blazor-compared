<template>
<h1>Counter</h1>

<p role="status">Current count: {{ currentCount }}</p>

<button class="btn btn-primary" @click="currentCount += 1">Click me</button>
</template>

<script setup lang="ts">
import { ref } from "vue";

const currentCount = ref(0);
</script>