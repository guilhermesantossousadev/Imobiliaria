<script setup>
defineProps({
  modelValue: { type: [String, Boolean], default: '' },
  options: { type: Array, required: true }, // [{ valor, rotulo }]
  label: { type: String, default: '' },
})
defineEmits(['update:modelValue'])
</script>

<template>
  <div class="seg" role="radiogroup" :aria-label="label || undefined">
    <p v-if="label" class="seg__rotulo">{{ label }}</p>
    <div class="seg__opcoes">
      <button v-for="o in options" :key="String(o.valor)" type="button" role="radio"
        :aria-checked="modelValue === o.valor" class="seg__opcao" :class="{ 'seg__opcao--ativa': modelValue === o.valor }"
        @click="$emit('update:modelValue', o.valor)">
        {{ o.rotulo }}
      </button>
    </div>
  </div>
</template>

<style scoped>
.seg__rotulo { margin: 0 0 var(--espaco-1); font-size: var(--texto-sm); font-weight: 500; color: var(--cor-primaria); }
.seg__opcoes { display: inline-flex; flex-wrap: wrap; gap: var(--espaco-2); }
.seg__opcao {
  font: inherit; font-size: var(--texto-sm); font-weight: 500; cursor: pointer;
  padding: 0.5rem 1.1rem; border-radius: var(--raio-sm);
  border: 1px solid var(--cor-borda); background: var(--cor-superficie); color: var(--cor-primaria);
  transition: all var(--transicao);
}
.seg__opcao:hover { border-color: var(--cor-primaria); }
.seg__opcao--ativa { background: var(--cor-primaria); border-color: var(--cor-primaria); color: #fff; }
</style>
