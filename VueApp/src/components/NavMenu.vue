<template>
    <div class="top-row ps-3 navbar navbar-dark">
        <div class="container-fluid">
            <a class="navbar-brand" href="">BlazorApp</a>
            <button title="Navigation menu" class="navbar-toggler" @onclick="ToggleNavMenu">
                <span class="navbar-toggler-icon"></span>
            </button>
        </div>
    </div>

    <div :class="navMenuCssClass">
        <nav class="flex-column">
            <div class="nav-item px-3">
                <router-link class="nav-link" to="/">
                    <span class="oi oi-home" aria-hidden="true"></span> Home
                </router-link>
            </div>
            <div class="nav-item px-3">
                <router-link class="nav-link" to="/counter">
                    <span class="oi oi-plus" aria-hidden="true"></span> Counter
                </router-link>
            </div>
            <div class="nav-item px-3">
                <router-link class="nav-link" to="/fetchdata">
                    <span class="oi oi-list-rich" aria-hidden="true"></span> Fetch data
                </router-link>
            </div>
        </nav>
    </div>
</template>

<script setup lang="ts">
import { ref } from "Vue";

const navMenuCssClass = ref<"collapse", null>("collapse");

function ToggleNavMenu() {
    navMenuCssClass.value = navMenuCssClass.value === "collapse" ? null : "collapse";
}
</script>

<style lang="css" scoped>
.navbar-toggler {
    background-color: rgba(255, 255, 255, 0.1);
}

.top-row {
    height: 3.5rem;
    background-color: rgba(0, 0, 0, 0.4);
}

.navbar-brand {
    font-size: 1.1rem;
}

.oi {
    width: 2rem;
    font-size: 1.1rem;
    vertical-align: text-top;
    top: -2px;
}

.nav-item {
    font-size: 0.9rem;
    padding-bottom: 0.5rem;
}

.nav-item:first-of-type {
    padding-top: 1rem;
}

.nav-item:last-of-type {
    padding-bottom: 1rem;
}

.nav-link {
    color: #d7d7d7;
    border-radius: 4px;
    height: 3rem;
    display: flex;
    align-items: center;
    line-height: 3rem;
}

.nav-item .router-link-active {
    background-color: rgba(255, 255, 255, 0.25);
    color: white;
}

.nav-link:hover {
    background-color: rgba(255, 255, 255, 0.1);
    color: white;
}

@media (min-width: 641px) {
    .navbar-toggler {
        display: none;
    }

    .collapse {
        /* Never collapse the sidebar for wide screens */
        display: block;
    }
}
</style>