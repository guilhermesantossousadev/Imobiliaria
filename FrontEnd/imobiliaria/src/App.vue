<script setup>
import AppHeader from '@/components/layout/AppHeader.vue'
import AppFooter from '@/components/layout/AppFooter.vue'
</script>

<template>
  <a href="#conteudo" class="pular-link">Pular para o conteúdo</a>
  <AppHeader />
  <main id="conteudo">
    <RouterView />
  </main>
  <AppFooter />
</template>

<style>
#app { min-height: 100vh; display: flex; flex-direction: column; }
main { flex: 1; }
.pular-link { position: absolute; left: -999px; top: 0; background: var(--cor-primaria); color: #fff; padding: 0.5rem 1rem; z-index: 100; }
.pular-link:focus { left: 0; }
</style>
