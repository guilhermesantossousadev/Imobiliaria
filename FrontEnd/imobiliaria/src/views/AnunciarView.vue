<script setup>
import { reactive, ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import PageHeader from '@/components/ui/PageHeader.vue'
import BaseField from '@/components/ui/BaseField.vue'
import BaseButton from '@/components/ui/BaseButton.vue'
import BaseAlert from '@/components/ui/BaseAlert.vue'
import BaseCounter from '@/components/ui/BaseCounter.vue'
import BaseSegmented from '@/components/ui/BaseSegmented.vue'
import { imoveisApi, buscarCep } from '@/services/api'
import { TIPOS_IMOVEL, mascararCep } from '@/utils/formatar'

const router = useRouter()

const form = reactive({
  tipoServico: 'Comprar', tipo: 'Apartamento',
  cep: '', rua: '', numero: '', bairro: '', cidade: '', unidadeFederativa: '',
  area: '', preco: '', anos: 0,
  quartos: 0, suites: 0, banheiros: 0, vagas: 0, andar: 0, tipoNegocio: '',
  titulo: '', descricao: '', areasComuns: '',
})
const imagens = ref([])
const enviando = ref(false)
const erro = ref('')
const aviso = ref('')

const servicos = [{ valor: 'Comprar', rotulo: 'Venda' }, { valor: 'Alugar', rotulo: 'Aluguel' }]

// Quais campos específicos cada tipo de imóvel usa
const campos = computed(() => ({
  quartos: ['Apartamento', 'Casa', 'Rural'].includes(form.tipo),
  suites: ['Apartamento', 'Casa', 'Rural'].includes(form.tipo),
  banheiros: ['Apartamento', 'Casa', 'Rural', 'Comercial'].includes(form.tipo),
  vagas: ['Apartamento', 'Casa', 'Rural', 'Comercial'].includes(form.tipo),
  andar: form.tipo === 'Apartamento',
  negocio: form.tipo === 'Comercial',
}))

async function aoDigitarCep(valor) {
  form.cep = mascararCep(valor)
  if (form.cep.length === 9) {
    const d = await buscarCep(form.cep)
    if (d) {
      form.rua = d.logradouro || form.rua
      form.bairro = d.bairro || form.bairro
      form.cidade = d.localidade || form.cidade
      form.unidadeFederativa = d.uf || form.unidadeFederativa
    }
  }
}

function escolherImagens(evento) {
  imagens.value = Array.from(evento.target.files).slice(0, 10)
}

async function publicar() {
  erro.value = ''
  aviso.value = ''
  enviando.value = true
  try {
    const c = campos.value
    const numero = Number(form.numero) || null
    const endereco = [form.rua, numero, form.bairro, form.cidade, form.unidadeFederativa].filter(Boolean).join(', ') + (form.cep ? ` - ${form.cep}` : '')

    const { id } = await imoveisApi.adicionar({
      tipo: form.tipo,
      tipoServico: form.tipoServico,
      area: Number(form.area),
      preco: Number(form.preco),
      anos: Number(form.anos) || 0,
      cep: form.cep, rua: form.rua, numero, bairro: form.bairro, cidade: form.cidade,
      unidadeFederativa: form.unidadeFederativa, endereco,
      titulo: form.titulo, descricao: form.descricao,
      areasComuns: form.areasComuns.split(',').map((a) => a.trim()).filter(Boolean),
      urlsImagens: [],
      quartos: c.quartos ? form.quartos : null,
      suites: c.suites ? form.suites : null,
      banheiros: c.banheiros ? form.banheiros : null,
      vagas: c.vagas ? form.vagas : null,
      andar: c.andar ? form.andar : null,
      tipoNegocio: c.negocio ? form.tipoNegocio : null,
    })

    // Envia as fotos em sequência; falhas não cancelam o anúncio
    let falhas = 0
    for (const arquivo of imagens.value) {
      try { await imoveisApi.enviarImagem(id, arquivo) } catch { falhas++ }
    }
    if (falhas) aviso.value = `${falhas} foto(s) não puderam ser enviadas.`
    router.push({ name: 'imovel', params: { id } })
  } catch (e) {
    erro.value = e.message
  } finally {
    enviando.value = false
  }
}
</script>

<template>
  <div class="pagina pagina--estreita">
    <div class="container">
      <PageHeader titulo="Anuncie seu imóvel" subtitulo="Preencha os dados abaixo. Não sabe quanto pedir? Use o precificador." />
      <BaseButton to="/precificador" variant="secundario" size="sm">Precificar meu imóvel</BaseButton>

      <form class="stack formulario" @submit.prevent="publicar">
        <BaseAlert v-if="erro" variant="erro">{{ erro }}</BaseAlert>
        <BaseAlert v-if="aviso" variant="info">{{ aviso }}</BaseAlert>

        <section class="painel stack">
          <h2>O imóvel</h2>
          <BaseSegmented v-model="form.tipoServico" :options="servicos" label="Finalidade" />
          <BaseField v-model="form.tipo" label="Tipo de imóvel" as="select">
            <option v-for="t in TIPOS_IMOVEL" :key="t">{{ t }}</option>
          </BaseField>
          <BaseField v-model="form.titulo" label="Título do anúncio" required maxlength="120" />
          <BaseField v-model="form.descricao" label="Descrição" as="textarea" required />
        </section>

        <section class="painel stack">
          <h2>Localização</h2>
          <div class="grid grid--2">
            <BaseField :model-value="form.cep" label="CEP" placeholder="00000-000" inputmode="numeric" hint="Preenchemos o endereço automaticamente." required @update:model-value="aoDigitarCep" />
            <BaseField v-model="form.rua" label="Rua" required />
            <BaseField v-model="form.numero" label="Número" type="number" min="0" />
            <BaseField v-model="form.bairro" label="Bairro" required />
            <BaseField v-model="form.cidade" label="Cidade" required />
            <BaseField v-model="form.unidadeFederativa" label="UF" maxlength="2" required />
          </div>
        </section>

        <section class="painel stack">
          <h2>Características</h2>
          <div class="grid grid--2">
            <BaseField v-model="form.area" label="Área (m²)" type="number" min="1" step="any" required />
            <BaseField v-model="form.preco" :label="form.tipoServico === 'Alugar' ? 'Aluguel mensal (R$)' : 'Preço (R$)'" type="number" min="1" step="any" required />
            <BaseField v-model.number="form.anos" label="Idade do imóvel (anos)" type="number" min="0" hint="0 = novo ou em construção." />
            <BaseField v-if="campos.negocio" v-model="form.tipoNegocio" label="Tipo de negócio" placeholder="Ex.: Loja, Sala comercial" />
          </div>
          <div class="grid grid--2 contadores">
            <BaseCounter v-if="campos.quartos" v-model="form.quartos" label="Quartos" />
            <BaseCounter v-if="campos.suites" v-model="form.suites" label="Suítes" />
            <BaseCounter v-if="campos.banheiros" v-model="form.banheiros" label="Banheiros" />
            <BaseCounter v-if="campos.vagas" v-model="form.vagas" label="Vagas de garagem" />
            <BaseCounter v-if="campos.andar" v-model="form.andar" label="Andar" />
          </div>
          <BaseField v-model="form.areasComuns" label="Diferenciais e áreas comuns" placeholder="Piscina, Academia, Churrasqueira" hint="Separe por vírgula." />
        </section>

        <section class="painel stack">
          <h2>Fotos</h2>
          <input type="file" accept="image/png,image/jpeg,image/webp,image/gif" multiple @change="escolherImagens" />
          <p class="texto-suave" style="margin: 0; font-size: var(--texto-sm)">Até 10 fotos, de até 5 MB cada (JPG, PNG, WebP ou GIF).</p>
        </section>

        <BaseButton type="submit" size="lg" variant="destaque" block :loading="enviando">Publicar anúncio</BaseButton>
      </form>
    </div>
  </div>
</template>

<style scoped>
.formulario { margin-top: var(--espaco-5); }
h2 { font-size: var(--texto-lg); margin: 0; }
.contadores { column-gap: var(--espaco-6); }
</style>
