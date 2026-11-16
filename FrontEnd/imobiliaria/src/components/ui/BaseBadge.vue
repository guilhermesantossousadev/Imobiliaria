<script setup>
defineProps({ variant: { type: String, default: 'neutro' } }) // neutro | destaque | primario | verde
</script>

<template>
  <span class="selo" :class="`selo--${variant}`"><slot /></span>
</template>

<style scoped>
.selo { display: inline-block; padding: 0.2rem 0.65rem; border-radius: var(--raio-sm); font-size: var(--texto-xs); font-weight: 600;  }
.selo--neutro { background: var(--cor-primaria-suave); color: var(--cor-primaria); }
.selo--destaque { background: var(--cor-destaque); color: var(--cor-primaria); }
.selo--primario { background: var(--cor-primaria); color: #fff; }
.selo--verde { background: var(--cor-verde); color: #fff; }
</style>
