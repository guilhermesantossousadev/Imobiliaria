<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import BaseButton from '@/components/ui/BaseButton.vue'
import BaseField from '@/components/ui/BaseField.vue'
import BaseSegmented from '@/components/ui/BaseSegmented.vue'
import LoadingState from '@/components/ui/LoadingState.vue'
import BaseAlert from '@/components/ui/BaseAlert.vue'
import ImovelCard from '@/components/imovel/ImovelCard.vue'
import CasarioFachada from '@/components/layout/CasarioFachada.vue'
import { imoveisApi } from '@/services/api'
import { TIPOS_IMOVEL } from '@/utils/formatar'

const router = useRouter()

const busca = ref({ servico: 'Comprar', tipo: '', texto: '' })
const opcoesServico = [
  { valor: 'Comprar', rotulo: 'Comprar' },
  { valor: 'Alugar', rotulo: 'Alugar' },
]

const imoveis = ref([])
const carregando = ref(true)
const erro = ref('')

const destaques = computed(() => imoveis.value.slice(0, 6))

onMounted(async () => {
  try {
    imoveis.value = await imoveisApi.listar()
  } catch (e) {
    erro.value = e.message
  } finally {
    carregando.value = false
  }
})

function buscar() {
  const query = {}
  if (busca.value.tipo) query.tipo = busca.value.tipo
  if (busca.value.texto.trim()) query.q = busca.value.texto.trim()
  router.push({ name: busca.value.servico === 'Alugar' ? 'alugar' : 'comprar', query })
}

const passos = [
  { titulo: 'Descubra quanto pedir', texto: 'Informe as características e veja um valor de referência no precificador.' },
  { titulo: 'Publique com fotos', texto: 'Preencha os dados do imóvel e envie até 10 fotos. O anúncio entra no ar na hora.' },
  { titulo: 'Receba mensagens', texto: 'Os interessados escrevem direto no seu anúncio e você responde pelo contato deixado.' },
]
</script>

<template>
  <section class="heroi">
    <div class="container heroi__conteudo">
      <h1 class="heroi__titulo">Seu próximo endereço começa aqui.</h1>
      <p class="heroi__sub">Casas, apartamentos, lotes e salas para comprar, alugar ou anunciar.</p>

      <form class="heroi__busca" @submit.prevent="buscar">
        <BaseSegmented v-model="busca.servico" :options="opcoesServico" label="O que você procura?" />
        <div class="heroi__campos">
          <BaseField v-model="busca.tipo" label="Tipo de imóvel" as="select">
            <option value="">Todos os tipos</option>
            <option v-for="t in TIPOS_IMOVEL" :key="t" :value="t">{{ t }}</option>
          </BaseField>
          <BaseField v-model="busca.texto" label="Cidade, bairro ou palavra-chave" placeholder="Ex.: Belo Horizonte" />
          <BaseButton type="submit" variant="primario" size="lg">Buscar imóveis</BaseButton>
        </div>
      </form>
    </div>
    <CasarioFachada />
  </section>

  <section class="pagina">
    <div class="container">
      <div class="secao-topo">
        <h2>Imóveis em destaque</h2>
        <BaseButton to="/imoveis" variant="fantasma">Ver todos os imóveis</BaseButton>
      </div>
      <LoadingState v-if="carregando" />
      <BaseAlert v-else-if="erro" variant="erro">{{ erro }}</BaseAlert>
      <div v-else class="grid grid--cards">
        <ImovelCard v-for="i in destaques" :key="i.id" :imovel="i" />
      </div>
    </div>
  </section>

  <section class="anunciar">
    <div class="container anunciar__grade">
      <div>
        <h2>Tem um imóvel para anunciar?</h2>
        <p>Publicar é grátis e leva poucos minutos.</p>
        <div class="cluster">
          <BaseButton to="/anunciar" variant="primario" size="lg">Anunciar imóvel</BaseButton>
          <BaseButton to="/precificador" variant="secundario" size="lg">Precificar</BaseButton>
        </div>
      </div>
      <ol class="passos">
        <li v-for="(p, n) in passos" :key="p.titulo">
          <span class="passos__num" aria-hidden="true">{{ n + 1 }}</span>
          <div>
            <h3>{{ p.titulo }}</h3>
            <p>{{ p.texto }}</p>
          </div>
        </li>
      </ol>
    </div>
  </section>
</template>

<style scoped>
.heroi { background: var(--cor-primaria); padding-top: var(--espaco-8); overflow: hidden; }
.heroi__conteudo { padding-bottom: var(--espaco-7); }
.heroi__titulo { color: var(--cor-fundo); max-width: 13ch; margin-bottom: var(--espaco-4); text-wrap: balance; }
.heroi__sub { color: var(--cor-primaria-suave); font-size: var(--texto-lg); max-width: 44ch; margin-bottom: var(--espaco-6); }
.heroi__busca {
  display: flex; flex-direction: column; gap: var(--espaco-4); max-width: 920px;
  background: var(--cor-superficie); padding: var(--espaco-5);
  border-radius: var(--raio-md); box-shadow: var(--sombra-placa);
}
.heroi__campos { display: grid; grid-template-columns: 1fr 1.6fr auto; gap: var(--espaco-3); align-items: end; }
.secao-topo { display: flex; align-items: baseline; justify-content: space-between; gap: var(--espaco-3); margin-bottom: var(--espaco-5); }
.secao-topo h2 { margin: 0; }
.anunciar { background: var(--cor-destaque-suave); border-top: 1px solid var(--cor-borda); padding-block: var(--espaco-8); }
.anunciar__grade { display: grid; grid-template-columns: 1fr 1.2fr; gap: var(--espaco-7); align-items: start; }
.anunciar p { color: var(--cor-texto-suave); }
.passos { list-style: none; display: flex; flex-direction: column; gap: var(--espaco-5); }
.passos li { display: grid; grid-template-columns: 3.2rem 1fr; gap: var(--espaco-4); padding-bottom: var(--espaco-5); border-bottom: 1px solid var(--cor-borda); }
.passos li:last-child { border-bottom: 0; padding-bottom: 0; }
.passos__num { font-family: var(--fonte-titulo); font-weight: 800; font-size: 2.6rem; line-height: 1; color: var(--cor-primaria); }
.passos h3 { font-size: var(--texto-lg); margin-bottom: var(--espaco-1); }
.passos p { margin: 0; }
@media (max-width: 800px) {
  .heroi__campos { grid-template-columns: 1fr; }
  .anunciar__grade { grid-template-columns: 1fr; gap: var(--espaco-6); }
}
</style>
