<script setup>
const props = defineProps({
  modelValue: { type: Number, default: 0 },
  label: { type: String, required: true },
  min: { type: Number, default: 0 },
  max: { type: Number, default: 99 },
})
const emit = defineEmits(['update:modelValue'])

const alterar = (delta) => emit('update:modelValue', Math.min(props.max, Math.max(props.min, props.modelValue + delta)))
</script>

<template>
  <div class="contador">
    <span class="contador__rotulo">{{ label }}</span>
    <div class="contador__controles">
      <button type="button" class="contador__btn" :aria-label="`Diminuir ${label}`" :disabled="modelValue <= min" @click="alterar(-1)">−</button>
      <output class="contador__valor" :aria-label="label">{{ modelValue }}</output>
      <button type="button" class="contador__btn" :aria-label="`Aumentar ${label}`" :disabled="modelValue >= max" @click="alterar(1)">+</button>
    </div>
  </div>
</template>

<style scoped>
.contador { display: flex; align-items: center; justify-content: space-between; gap: var(--espaco-3); padding: var(--espaco-2) 0; }
.contador__rotulo { font-size: var(--texto-sm); font-weight: 500; color: var(--cor-primaria); }
.contador__controles { display: inline-flex; align-items: center; gap: var(--espaco-3); }
.contador__btn {
  width: 34px; height: 34px; border-radius: var(--raio-sm); border: 1px solid var(--cor-borda);
  background: var(--cor-superficie); color: var(--cor-primaria); font: inherit; font-size: 1.1rem; cursor: pointer;
}
.contador__btn:hover:not(:disabled) { border-color: var(--cor-primaria); }
.contador__btn:disabled { opacity: 0.4; cursor: not-allowed; }
.contador__valor { min-width: 1.6rem; text-align: center; font-weight: 600; }
</style>
