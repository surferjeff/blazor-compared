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
                <td>{{f.Date}}</td>
                <td>{{f.TemperatureC}}</td>
                <td>{{f.TemperatureF}}</td>
                <td>{{f.Summary}}</td>
            </tr>
        </tbody>    
    </table>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';

interface Forecast {
    Date: string,
    TemperatureC: number,
    TemperatureF: number,
    Summary: string,
}

const forecasts = ref<Forecast[]>([]);

async function fetchForecasts() {
  const res = await fetch('http://localhost:5164');
  forecasts.value = await res.json() as Forecast[];
}

onMounted(fetchForecasts);


</script>