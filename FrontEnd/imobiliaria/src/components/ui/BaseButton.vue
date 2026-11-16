<script setup>
import { computed } from 'vue'

const props = defineProps({
  variant: { type: String, default: 'primario' }, // primario | destaque | secundario | fantasma | perigo
  size: { type: String, default: 'md' }, // sm | md | lg
  to: { type: [String, Object], default: null },
  type: { type: String, default: 'button' },
  loading: { type: Boolean, default: false },
  block: { type: Boolean, default: false },
  disabled: { type: Boolean, default: false },
})

const classes = computed(() => ['btn', `btn--${props.variant}`, `btn--${props.size}`, { 'btn--block': props.block }])
</script>

<template>
  <RouterLink v-if="to" :to="to" :class="classes">
    <slot />
  </RouterLink>
  <button v-else :type="type" :class="classes" :disabled="disabled || loading" :aria-busy="loading">
    <span v-if="loading" class="btn__spinner" aria-hidden="true" />
    <slot />
  </button>
</template>

<style scoped>
.btn {
  display: inline-flex; align-items: center; justify-content: center; gap: var(--espaco-2);
  font: inherit; font-weight: 600; text-decoration: none; cursor: pointer;
  border: 1px solid transparent; border-radius: var(--raio-sm);
  transition: background var(--transicao), border-color var(--transicao), transform var(--transicao), box-shadow var(--transicao);
}
.btn:active { transform: translateY(1px); }
.btn:disabled { opacity: 0.55; cursor: not-allowed; }
.btn--sm { padding: 0.4rem 0.9rem; font-size: var(--texto-sm); }
.btn--md { padding: 0.65rem 1.4rem; font-size: var(--texto-md); }
.btn--lg { padding: 0.85rem 1.9rem; font-size: var(--texto-lg); }
.btn--block { width: 100%; }

.btn--primario { background: var(--cor-primaria); color: var(--cor-texto-inverso); }
.btn--primario:hover:not(:disabled) { background: var(--cor-primaria-hover); }
.btn--destaque { background: var(--cor-destaque); color: var(--cor-primaria); }
.btn--destaque:hover:not(:disabled) { background: var(--cor-destaque-hover); }
.btn--secundario { background: var(--cor-superficie); color: var(--cor-primaria); border-color: var(--cor-borda); }
.btn--secundario:hover:not(:disabled) { border-color: var(--cor-primaria); }
.btn--fantasma { background: transparent; color: var(--cor-primaria); }
.btn--fantasma:hover:not(:disabled) { background: var(--cor-primaria-suave); }
.btn--perigo { background: var(--cor-erro); color: #fff; }
.btn--perigo:hover:not(:disabled) { filter: brightness(0.92); }

.btn__spinner {
  width: 1em; height: 1em; border-radius: 50%;
  border: 2px solid currentColor; border-right-color: transparent;
  animation: girar 700ms linear infinite;
}
@keyframes girar { to { transform: rotate(360deg); } }
</style>
