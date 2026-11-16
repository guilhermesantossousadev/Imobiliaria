<script setup>
defineProps({ variant: { type: String, default: 'info' } }) // info | sucesso | erro
</script>

<template>
  <div class="alerta" :class="`alerta--${variant}`" :role="variant === 'erro' ? 'alert' : 'status'">
    <slot />
  </div>
</template>

<style scoped>
.alerta { padding: var(--espaco-3) var(--espaco-4); border-radius: var(--raio-md); font-size: var(--texto-sm); border: 1px solid transparent; }
.alerta--info { background: var(--cor-info-fundo); color: var(--cor-info); border-color: color-mix(in srgb, var(--cor-info) 25%, transparent); }
.alerta--sucesso { background: var(--cor-sucesso-fundo); color: var(--cor-sucesso); border-color: color-mix(in srgb, var(--cor-sucesso) 25%, transparent); }
.alerta--erro { background: var(--cor-erro-fundo); color: var(--cor-erro); border-color: color-mix(in srgb, var(--cor-erro) 25%, transparent); }
</style>
