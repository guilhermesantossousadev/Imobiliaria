<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import PageHeader from '@/components/ui/PageHeader.vue'
import BaseField from '@/components/ui/BaseField.vue'
import BaseButton from '@/components/ui/BaseButton.vue'
import BaseAlert from '@/components/ui/BaseAlert.vue'
import BaseSegmented from '@/components/ui/BaseSegmented.vue'
import EmptyState from '@/components/ui/EmptyState.vue'
import LoadingState from '@/components/ui/LoadingState.vue'
import ImovelCard from '@/components/imovel/ImovelCard.vue'
import { imoveisApi } from '@/services/api'
import { TIPOS_IMOVEL } from '@/utils/formatar'

// servico fixo ('Comprar' | 'Alugar') vem da rota; sem ele mostra todos
const props = defineProps({ servico: { type: String, default: '' } })

const route = useRoute()
const router = useRouter()

const imoveis = ref([])
const carregando = ref(true)
const erro = ref('')

const filtros = ref({
  tipo: route.query.tipo || '',
  q: route.query.q || '',
  precoMax: '',
  ordem: 'recentes',
})
const servicoAtual = ref(props.servico || route.query.servico || '')

const opcoesServico = [
  { valor: '', rotulo: 'Todos' },
  { valor: 'Comprar', rotulo: 'Comprar' },
  { valor: 'Alugar', rotulo: 'Alugar' },
]

async function carregar() {
  carregando.value = true
  erro.value = ''
  try {
    imoveis.value = await imoveisApi.listar()
  } catch (e) {
    erro.value = e.message
  } finally {
    carregando.value = false
  }
}
onMounted(carregar)

// Mantém filtros em sincronia quando a rota muda (ex.: busca da home)
watch(() => route.fullPath, () => {
  filtros.value.tipo = route.query.tipo || ''
  filtros.value.q = route.query.q || ''
  servicoAtual.value = props.servico || route.query.servico || ''
})

function trocarServico(valor) {
  servicoAtual.value = valor
  if (!props.servico) router.replace({ query: { ...route.query, servico: valor || undefined } })
}

const titulo = computed(() => {
  const s = props.servico || servicoAtual.value
  return s === 'Comprar' ? 'Imóveis para comprar' : s === 'Alugar' ? 'Imóveis para alugar' : 'Todos os imóveis'
})

const normalizar = (t) => String(t ?? '').normalize('NFD').replace(/[̀-ͯ]/g, '').toLowerCase()

const filtrados = computed(() => {
  const f = filtros.value
  const termo = normalizar(f.q)
  const lista = imoveis.value.filter((i) => {
    if (servicoAtual.value && normalizar(i.tipoServico) !== normalizar(servicoAtual.value)) return false
    if (f.tipo && normalizar(i.tipo) !== normalizar(f.tipo)) return false
    if (f.precoMax && i.preco > Number(f.precoMax)) return false
    if (termo) {
      const palheiro = normalizar([i.titulo, i.bairro, i.cidade, i.rua, i.unidadeFederativa, i.descricao, i.tipo, ...(i.areasComuns || [])].join(' '))
      if (!palheiro.includes(termo)) return false
    }
    return true
  })
  const ordenadores = {
    recentes: () => 0,
    menor: (a, b) => a.preco - b.preco,
    maior: (a, b) => b.preco - a.preco,
    area: (a, b) => b.area - a.area,
  }
  return [...lista].sort(ordenadores[f.ordem])
})

function limpar() {
  filtros.value = { tipo: '', q: '', precoMax: '', ordem: 'recentes' }
  if (!props.servico) servicoAtual.value = ''
  router.replace({ query: {} })
}
</script>

<template>
  <div class="pagina">
    <div class="container">
      <PageHeader :titulo="titulo" :subtitulo="carregando ? '' : `${filtrados.length} ${filtrados.length === 1 ? 'imóvel encontrado' : 'imóveis encontrados'}`" />

      <form class="painel filtros" @submit.prevent>
        <BaseSegmented v-if="!servico" :model-value="servicoAtual" :options="opcoesServico" label="Finalidade" @update:model-value="trocarServico" />
        <div class="filtros__campos">
          <BaseField v-model="filtros.q" label="Buscar" placeholder="Cidade, bairro, palavra-chave…" />
          <BaseField v-model="filtros.tipo" label="Tipo" as="select">
            <option value="">Todos</option>
            <option v-for="t in TIPOS_IMOVEL" :key="t" :value="t">{{ t }}</option>
          </BaseField>
          <BaseField v-model="filtros.precoMax" label="Preço máximo (R$)" type="number" min="0" step="1000" placeholder="Sem limite" />
          <BaseField v-model="filtros.ordem" label="Ordenar por" as="select">
            <option value="recentes">Padrão</option>
            <option value="menor">Menor preço</option>
            <option value="maior">Maior preço</option>
            <option value="area">Maior área</option>
          </BaseField>
          <BaseButton variant="fantasma" @click="limpar">Limpar</BaseButton>
        </div>
      </form>

      <LoadingState v-if="carregando" />
      <BaseAlert v-else-if="erro" variant="erro">
        {{ erro }} <BaseButton size="sm" variant="secundario" @click="carregar">Tentar novamente</BaseButton>
      </BaseAlert>
      <EmptyState v-else-if="!filtrados.length" titulo="Nenhum imóvel encontrado">
        Ajuste os filtros ou limpe a busca para ver mais opções.
      </EmptyState>
      <div v-else class="grid grid--cards resultados">
        <ImovelCard v-for="i in filtrados" :key="i.id" :imovel="i" />
      </div>
    </div>
  </div>
</template>

<style scoped>
.filtros { display: flex; flex-direction: column; gap: var(--espaco-4); margin-bottom: var(--espaco-6); }
.filtros__campos { display: grid; grid-template-columns: 2fr 1fr 1fr 1fr auto; gap: var(--espaco-3); align-items: end; }
@media (max-width: 960px) { .filtros__campos { grid-template-columns: 1fr 1fr; } }
@media (max-width: 560px) { .filtros__campos { grid-template-columns: 1fr; } }
</style>
