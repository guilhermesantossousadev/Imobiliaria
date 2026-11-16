<script setup>
import { useId } from 'vue'

defineProps({
  modelValue: { type: [String, Number], default: '' },
  label: { type: String, required: true },
  as: { type: String, default: 'input' }, // input | select | textarea
  type: { type: String, default: 'text' },
  error: { type: String, default: '' },
  hint: { type: String, default: '' },
})
defineEmits(['update:modelValue'])

const id = useId()
</script>

<template>
  <div class="campo" :class="{ 'campo--erro': error }">
    <label :for="id" class="campo__rotulo">{{ label }}</label>

    <select v-if="as === 'select'" :id="id" class="campo__controle" :value="modelValue"
      v-bind="$attrs" @change="$emit('update:modelValue', $event.target.value)">
      <slot />
    </select>
    <textarea v-else-if="as === 'textarea'" :id="id" class="campo__controle" rows="4" :value="modelValue"
      v-bind="$attrs" @input="$emit('update:modelValue', $event.target.value)" />
    <input v-else :id="id" class="campo__controle" :type="type" :value="modelValue"
      v-bind="$attrs" @input="$emit('update:modelValue', $event.target.value)" />

    <p v-if="error" class="campo__msg campo__msg--erro" role="alert">{{ error }}</p>
    <p v-else-if="hint" class="campo__msg">{{ hint }}</p>
  </div>
</template>

<script>
export default { inheritAttrs: false }
</script>

<style scoped>
.campo { display: flex; flex-direction: column; gap: var(--espaco-1); }
.campo__rotulo { font-size: var(--texto-sm); font-weight: 500; color: var(--cor-primaria); }
.campo__controle {
  font: inherit; color: var(--cor-texto); background: var(--cor-superficie);
  border: 1px solid var(--cor-borda); border-radius: var(--raio-md);
  padding: 0.65rem 0.9rem; width: 100%;
  transition: border-color var(--transicao), box-shadow var(--transicao);
}
.campo__controle:hover { border-color: var(--cor-apoio); }
.campo__controle:focus { outline: none; border-color: var(--cor-primaria); box-shadow: 0 0 0 3px var(--cor-primaria-suave); }
textarea.campo__controle { resize: vertical; min-height: 110px; }
.campo--erro .campo__controle { border-color: var(--cor-erro); }
.campo__msg { margin: 0; font-size: var(--texto-xs); color: var(--cor-texto-suave); }
.campo__msg--erro { color: var(--cor-erro); }
</style>
