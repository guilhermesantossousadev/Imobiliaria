<script setup>
import { ref, computed } from 'vue'
import { imagemPadrao } from '@/utils/formatar'

const props = defineProps({ imagens: { type: Array, default: () => [] }, titulo: { type: String, default: '' } })

const lista = computed(() => (props.imagens.length ? props.imagens : [imagemPadrao]))
const atual = ref(0)

const ir = (delta) => { atual.value = (atual.value + delta + lista.value.length) % lista.value.length }
</script>

<template>
  <section class="galeria" aria-label="Fotos do imóvel">
    <div class="galeria__principal">
      <img :src="lista[atual]" :alt="`${titulo} — foto ${atual + 1}`" @error="$event.target.src = imagemPadrao" />
      <template v-if="lista.length > 1">
        <button type="button" class="galeria__seta galeria__seta--esq" aria-label="Foto anterior" @click="ir(-1)">‹</button>
        <button type="button" class="galeria__seta galeria__seta--dir" aria-label="Próxima foto" @click="ir(1)">›</button>
        <span class="galeria__contador">{{ atual + 1 }} / {{ lista.length }}</span>
      </template>
    </div>
    <ul v-if="lista.length > 1" class="galeria__miniaturas">
      <li v-for="(src, i) in lista" :key="src + i">
        <button type="button" :class="{ 'is-ativa': i === atual }" :aria-label="`Ver foto ${i + 1}`" @click="atual = i">
          <img :src="src" alt="" loading="lazy" @error="$event.target.src = imagemPadrao" />
        </button>
      </li>
    </ul>
  </section>
</template>

<style scoped>
.galeria__principal { position: relative; aspect-ratio: 16 / 10; border-radius: var(--raio-lg); overflow: hidden; background: var(--cor-primaria-suave); }
.galeria__principal img { width: 100%; height: 100%; object-fit: cover; }
.galeria__seta {
  position: absolute; top: 50%; transform: translateY(-50%); width: 42px; height: 42px; border-radius: var(--raio-sm);
  border: 0; background: rgba(255, 255, 255, 0.94); color: var(--cor-primaria); font-size: 1.6rem; line-height: 1; cursor: pointer; }
.galeria__seta--esq { left: var(--espaco-3); }
.galeria__seta--dir { right: var(--espaco-3); }
.galeria__contador { position: absolute; right: var(--espaco-3); bottom: var(--espaco-3); background: rgba(31, 42, 53, 0.75); color: #fff; font-size: var(--texto-xs); padding: 0.15rem 0.6rem; border-radius: var(--raio-sm); }
.galeria__miniaturas { list-style: none; display: flex; gap: var(--espaco-2); margin-top: var(--espaco-3); overflow-x: auto; padding-bottom: var(--espaco-1); }
.galeria__miniaturas button { width: 84px; height: 60px; padding: 0; border: 2px solid transparent; border-radius: var(--raio-sm); overflow: hidden; cursor: pointer; background: none; opacity: 0.7; }
.galeria__miniaturas button.is-ativa { border-color: var(--cor-destaque); opacity: 1; }
.galeria__miniaturas img { width: 100%; height: 100%; object-fit: cover; }
</style>
