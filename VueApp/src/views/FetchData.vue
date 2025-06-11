<template>
    <h1>Weather forecast</h1>

    <p>This component demonstrates fetching data from a service.</p>

    <p v-if="forecasts.length < 1"><em>Loading...</em></p>

    <table class="table" v-else>
        <thead>
            <tr>
                <th>Date</th>
                <th>Temp. (C)</th>
                <th>Temp. (F)</th>
                <th>Summary</th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="f in forecasts" :key="f.Date">
                <td>{{f.date.substr(0,10)}}</td>
                <td>{{f.temperatureC}}</td>
                <td>{{f.temperatureF}}</td>
                <td>{{f.summary}}</td>
            </tr>
        </tbody>    
    </table>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';

interface Forecast {
    date: string,
    temperatureC: number,
    temperatureF: number,
    summary: string,
}

const forecasts = ref<Forecast[]>([]);

async function fetchForecasts() {
  try {
    const res = await fetch('/Api/Forecasts');
    forecasts.value = await res.json() as Forecast[];
  } finally {
      window.setTimeout(fetchForecasts, 2000);
  }
}

onMounted(() => window.setTimeout(fetchForecasts, 2000));


</script>