<script setup>
defineProps({ titulo: { type: String, required: true } })
</script>

<template>
  <div class="vazio">
    <svg class="vazio__icone" viewBox="0 0 48 48" aria-hidden="true">
      <path d="M8 23 24 9l16 14v15a2 2 0 0 1-2 2H10a2 2 0 0 1-2-2z" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linejoin="round" />
    </svg>
    <h3>{{ titulo }}</h3>
    <p class="texto-suave"><slot /></p>
  </div>
</template>

<style scoped>
.vazio { text-align: center; padding: var(--espaco-7) var(--espaco-4); color: var(--cor-texto-suave); }
.vazio__icone { width: 56px; height: 56px; margin: 0 auto var(--espaco-3); color: var(--cor-apoio); }
.vazio h3 { color: var(--cor-primaria); }
</style>
