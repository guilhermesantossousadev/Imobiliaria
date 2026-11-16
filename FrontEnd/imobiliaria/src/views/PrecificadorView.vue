<script setup>
import { reactive, ref } from 'vue'
import PageHeader from '@/components/ui/PageHeader.vue'
import BaseField from '@/components/ui/BaseField.vue'
import BaseButton from '@/components/ui/BaseButton.vue'
import BaseAlert from '@/components/ui/BaseAlert.vue'
import BaseCounter from '@/components/ui/BaseCounter.vue'
import BaseSegmented from '@/components/ui/BaseSegmented.vue'
import { imoveisApi } from '@/services/api'

const form = reactive({
  vendaOuAluguel: 'venda', novo: false, tipoImovel: 'apartamento',
  quartos: 2, banheiros: 1, vagas: 1, areaM2: '', areasComuns: [],
})
const resultado = ref(null)
const carregando = ref(false)
const erro = ref('')

const finalidades = [{ valor: 'venda', rotulo: 'Venda' }, { valor: 'aluguel', rotulo: 'Aluguel' }]
const tipos = [{ valor: 'apartamento', rotulo: 'Apartamento' }, { valor: 'casa', rotulo: 'Casa' }]
const simNao = [{ valor: true, rotulo: 'Sim' }, { valor: false, rotulo: 'Não' }]
const diferenciais = ['Piscina', 'Academia', 'Churrasqueira', 'Playground', 'Salão de festas', 'Portaria 24h']

async function calcular() {
  carregando.value = true
  erro.value = ''
  resultado.value = null
  try {
    const { preco } = await imoveisApi.precificar({ ...form, areaM2: Number(form.areaM2) })
    resultado.value = preco
  } catch (e) {
    erro.value = e.message
  } finally {
    carregando.value = false
  }
}

const moeda = (v) => v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })
</script>

<template>
  <div class="pagina pagina--estreita">
    <div class="container">
      <PageHeader titulo="Precifique seu imóvel" subtitulo="Informe as características e receba um valor de referência. É uma estimativa simples, não uma avaliação oficial." />

      <form class="painel stack" @submit.prevent="calcular">
        <BaseSegmented v-model="form.vendaOuAluguel" :options="finalidades" label="Finalidade" />
        <BaseSegmented v-model="form.tipoImovel" :options="tipos" label="Tipo de imóvel" />
        <BaseSegmented v-model="form.novo" :options="simNao" label="O imóvel é novo?" />

        <div class="contadores">
          <BaseCounter v-model="form.quartos" label="Quartos" />
          <BaseCounter v-model="form.banheiros" label="Banheiros" />
          <BaseCounter v-model="form.vagas" label="Vagas de garagem" />
        </div>

        <BaseField v-model="form.areaM2" label="Área (m²)" type="number" min="1" step="any" required />

        <fieldset class="diferenciais">
          <legend>Diferenciais</legend>
          <label v-for="d in diferenciais" :key="d" class="check">
            <input v-model="form.areasComuns" type="checkbox" :value="d" /> {{ d }}
          </label>
        </fieldset>

        <BaseButton type="submit" size="lg" variant="destaque" block :loading="carregando">Calcular valor</BaseButton>
        <BaseAlert v-if="erro" variant="erro">{{ erro }}</BaseAlert>
      </form>

      <section v-if="resultado !== null" class="painel resultado" aria-live="polite">
        <p class="texto-suave" style="margin: 0">Valor estimado {{ form.vendaOuAluguel === 'venda' ? 'de venda' : 'de referência' }}</p>
        <p class="resultado__valor">{{ moeda(resultado) }}</p>
        <BaseButton to="/anunciar" variant="primario">Anunciar meu imóvel</BaseButton>
      </section>
    </div>
  </div>
</template>

<style scoped>
.contadores { display: flex; flex-direction: column; max-width: 340px; }
.diferenciais { border: 1px solid var(--cor-borda); border-radius: var(--raio-md); padding: var(--espaco-3) var(--espaco-4); display: grid; grid-template-columns: repeat(auto-fit, minmax(170px, 1fr)); gap: var(--espaco-2); }
.diferenciais legend { padding-inline: var(--espaco-2); font-size: var(--texto-sm); font-weight: 500; color: var(--cor-primaria); }
.check { display: flex; align-items: center; gap: var(--espaco-2); font-size: var(--texto-sm); cursor: pointer; }
.resultado { margin-top: var(--espaco-5); text-align: center; background: var(--cor-destaque-suave); }
.resultado__valor { font-size: var(--texto-3xl); font-weight: 700; color: var(--cor-primaria); margin: var(--espaco-2) 0 var(--espaco-4); }
</style>
