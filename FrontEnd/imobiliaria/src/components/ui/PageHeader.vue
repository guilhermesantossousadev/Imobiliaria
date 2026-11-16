<script setup>
defineProps({ titulo: { type: String, required: true }, subtitulo: { type: String, default: '' } })
</script>

<template>
  <header class="cabecalho-pagina">
    <h1>{{ titulo }}</h1>
    <p v-if="subtitulo" class="texto-suave cabecalho-pagina__sub">{{ subtitulo }}</p>
    <slot />
  </header>
</template>

<style scoped>
.cabecalho-pagina { margin-bottom: var(--espaco-6); }
.cabecalho-pagina h1 { font-size: var(--texto-2xl); }
.cabecalho-pagina__sub { font-size: var(--texto-lg); max-width: 60ch; }
</style>
