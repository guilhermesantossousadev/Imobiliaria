<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import PageHeader from '@/components/ui/PageHeader.vue'
import BaseBadge from '@/components/ui/BaseBadge.vue'
import BaseButton from '@/components/ui/BaseButton.vue'
import BaseAlert from '@/components/ui/BaseAlert.vue'
import EmptyState from '@/components/ui/EmptyState.vue'
import LoadingState from '@/components/ui/LoadingState.vue'
import ImovelGaleria from '@/components/imovel/ImovelGaleria.vue'
import ContatoForm from '@/components/imovel/ContatoForm.vue'
import { imoveisApi, ApiError } from '@/services/api'
import { formatarPreco, formatarArea, rotuloServico } from '@/utils/formatar'

const props = defineProps({ id: { type: String, required: true } })

const imovel = ref(null)
const carregando = ref(true)
const erro = ref('')
const naoEncontrado = ref(false)

async function carregar() {
  carregando.value = true
  erro.value = ''
  naoEncontrado.value = false
  try {
    imovel.value = await imoveisApi.porId(props.id)
  } catch (e) {
    if (e instanceof ApiError && e.status === 404) naoEncontrado.value = true
    else erro.value = e.message
  } finally {
    carregando.value = false
  }
}
onMounted(carregar)
watch(() => props.id, carregar)

const caracteristicas = computed(() => {
  const i = imovel.value
  if (!i) return []
  return [
    ['Tipo', i.tipo],
    ['Área', formatarArea(i.area)],
    i.quartos != null && ['Quartos', i.quartos],
    i.suites != null && ['Suítes', i.suites],
    i.banheiros != null && ['Banheiros', i.banheiros],
    i.vagas != null && ['Vagas', i.vagas],
    i.andar != null && ['Andar', i.andar],
    i.tipoNegocio && ['Tipo de negócio', i.tipoNegocio],
    ['Idade do imóvel', i.anos > 0 ? `${i.anos} ${i.anos === 1 ? 'ano' : 'anos'}` : 'Novo / em construção'],
  ].filter(Boolean)
})
</script>

<template>
  <div class="pagina">
    <div class="container">
      <BaseButton to="/imoveis" variant="fantasma" size="sm">← Voltar para os imóveis</BaseButton>

      <LoadingState v-if="carregando" />
      <BaseAlert v-else-if="erro" variant="erro">{{ erro }}</BaseAlert>
      <EmptyState v-else-if="naoEncontrado" titulo="Imóvel não encontrado">
        Este anúncio pode ter sido removido.
      </EmptyState>

      <div v-else-if="imovel" class="detalhe">
        <div class="detalhe__principal stack">
          <ImovelGaleria :imagens="imovel.urlsImagens" :titulo="imovel.titulo" />

          <div class="cluster">
            <BaseBadge :variant="imovel.tipoServico?.toLowerCase() === 'alugar' ? 'verde' : 'destaque'">{{ rotuloServico(imovel.tipoServico) }}</BaseBadge>
            <BaseBadge>{{ imovel.tipo }}</BaseBadge>
          </div>
          <PageHeader :titulo="imovel.titulo" :subtitulo="imovel.endereco" />

          <section class="painel">
            <h2 class="titulo-secao">Características</h2>
            <dl class="caracteristicas">
              <div v-for="[rotulo, valor] in caracteristicas" :key="rotulo">
                <dt>{{ rotulo }}</dt>
                <dd>{{ valor }}</dd>
              </div>
            </dl>
          </section>

          <section class="painel">
            <h2 class="titulo-secao">Descrição</h2>
            <p class="descricao">{{ imovel.descricao }}</p>
          </section>

          <section v-if="imovel.areasComuns?.length" class="painel">
            <h2 class="titulo-secao">Diferenciais e áreas comuns</h2>
            <ul class="lista-chips">
              <li v-for="a in imovel.areasComuns" :key="a">{{ a }}</li>
            </ul>
          </section>
        </div>

        <aside class="detalhe__lateral">
          <div class="painel stack lateral-fixa">
            <p class="preco">{{ formatarPreco(imovel.preco, imovel.tipoServico) }}</p>
            <ContatoForm :imovel-id="imovel.id" titulo="Fale com o anunciante" botao="Enviar mensagem" />
          </div>
        </aside>
      </div>
    </div>
  </div>
</template>

<style scoped>
.detalhe { display: grid; grid-template-columns: minmax(0, 1fr) 360px; gap: var(--espaco-6); margin-top: var(--espaco-4); align-items: start; }
.lateral-fixa { position: sticky; top: calc(var(--altura-header) + var(--espaco-4)); }
.preco { margin: 0; font-family: var(--fonte-titulo); font-size: var(--texto-2xl); font-weight: 800; letter-spacing: -0.02em; color: var(--cor-tinta); }
.titulo-secao { font-size: var(--texto-lg); }
.descricao { white-space: pre-line; margin: 0; }
.caracteristicas { display: grid; grid-template-columns: repeat(auto-fill, minmax(150px, 1fr)); gap: var(--espaco-4); margin: 0; }
.caracteristicas dt { font-size: var(--texto-xs); color: var(--cor-texto-suave); }
.caracteristicas dd { margin: 0; font-weight: 600; color: var(--cor-primaria); }
.lista-chips { list-style: none; display: flex; flex-wrap: wrap; gap: var(--espaco-2); }
.lista-chips li { background: var(--cor-primaria-suave); color: var(--cor-tinta); padding: 0.3rem 0.9rem; border-radius: var(--raio-sm); font-size: var(--texto-sm); }
@media (max-width: 960px) { .detalhe { grid-template-columns: 1fr; } .lateral-fixa { position: static; } }
</style>
