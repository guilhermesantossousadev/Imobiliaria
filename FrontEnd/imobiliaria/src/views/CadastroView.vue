<script setup>
import { reactive, ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import PageHeader from '@/components/ui/PageHeader.vue'
import BaseField from '@/components/ui/BaseField.vue'
import BaseButton from '@/components/ui/BaseButton.vue'
import BaseAlert from '@/components/ui/BaseAlert.vue'
import { usuariosApi, buscarCep } from '@/services/api'
import { sessao } from '@/stores/sessao'
import { validarCpf, validarCnpj, mascararCpf, mascararCnpj, mascararCep } from '@/utils/formatar'

const router = useRouter()

const form = reactive({
  nome: '', idade: '', genero: '', tipoDoc: 'CPF', identificacao: '',
  cep: '', rua: '', numero: '', bairro: '', cidade: '', unidadeFederativa: '',
  username: '', password: '', confirmar: '',
})
const foto = ref(null)
const enviando = ref(false)
const erroGeral = ref('')
const erros = reactive({})

const maxDoc = computed(() => (form.tipoDoc === 'CPF' ? 14 : 18))

function aoDigitarDoc(valor) {
  form.identificacao = form.tipoDoc === 'CPF' ? mascararCpf(valor) : mascararCnpj(valor)
}

async function aoDigitarCep(valor) {
  form.cep = mascararCep(valor)
  if (form.cep.length === 9) {
    const dados = await buscarCep(form.cep)
    if (dados) {
      form.rua = dados.logradouro || form.rua
      form.bairro = dados.bairro || form.bairro
      form.cidade = dados.localidade || form.cidade
      form.unidadeFederativa = dados.uf || form.unidadeFederativa
    }
  }
}

function validar() {
  Object.keys(erros).forEach((k) => delete erros[k])
  const docOk = form.tipoDoc === 'CPF' ? validarCpf(form.identificacao) : validarCnpj(form.identificacao)
  if (!docOk) erros.identificacao = `${form.tipoDoc} inválido.`
  if (form.password.length < 6) erros.password = 'Use pelo menos 6 caracteres.'
  if (form.password !== form.confirmar) erros.confirmar = 'As senhas não conferem.'
  if (Number(form.idade) < 18) erros.idade = 'É necessário ter 18 anos ou mais.'
  return Object.keys(erros).length === 0
}

async function cadastrar() {
  erroGeral.value = ''
  if (!validar()) return
  enviando.value = true
  try {
    await usuariosApi.cadastrar({
      nome: form.nome, idade: Number(form.idade), genero: form.genero,
      identificacao: `${form.tipoDoc}: ${form.identificacao}`,
      cep: form.cep, rua: form.rua, numero: Number(form.numero) || 0,
      bairro: form.bairro, cidade: form.cidade, unidadeFederativa: form.unidadeFederativa,
      username: form.username, password: form.password,
    })
    // Entra automaticamente e envia a foto (opcional)
    let usuario = await usuariosApi.entrar(form.username, form.password)
    if (foto.value) {
      try {
        await usuariosApi.enviarFoto(usuario.id, foto.value)
        usuario = await usuariosApi.porId(usuario.id)
      } catch { /* foto é opcional: segue sem ela */ }
    }
    sessao.entrar(usuario)
    router.push('/')
  } catch (e) {
    erroGeral.value = e.message
  } finally {
    enviando.value = false
  }
}
</script>

<template>
  <div class="pagina pagina--estreita">
    <div class="container">
      <PageHeader titulo="Crie sua conta" subtitulo="Cadastre-se para anunciar imóveis e gerenciar seu perfil." />
      <form class="painel stack" @submit.prevent="cadastrar">
        <BaseAlert v-if="erroGeral" variant="erro">{{ erroGeral }}</BaseAlert>

        <h2 class="secao">Dados pessoais</h2>
        <div class="grid grid--2">
          <BaseField v-model="form.nome" label="Nome completo" autocomplete="name" required />
          <BaseField v-model="form.idade" label="Idade" type="number" min="18" max="120" :error="erros.idade" required />
          <BaseField v-model="form.genero" label="Gênero" as="select" required>
            <option value="" disabled>Selecione…</option>
            <option>Feminino</option>
            <option>Masculino</option>
            <option>Outro</option>
          </BaseField>
          <BaseField v-model="form.tipoDoc" label="Documento" as="select" @update:model-value="form.identificacao = ''">
            <option>CPF</option>
            <option>CNPJ</option>
          </BaseField>
          <BaseField :model-value="form.identificacao" :label="form.tipoDoc" :maxlength="maxDoc" :error="erros.identificacao" required
            inputmode="numeric" @update:model-value="aoDigitarDoc" />
        </div>

        <h2 class="secao">Endereço</h2>
        <div class="grid grid--2">
          <BaseField :model-value="form.cep" label="CEP" placeholder="00000-000" inputmode="numeric" hint="Preenchemos o restante automaticamente." required @update:model-value="aoDigitarCep" />
          <BaseField v-model="form.rua" label="Rua" required />
          <BaseField v-model="form.numero" label="Número" type="number" min="0" required />
          <BaseField v-model="form.bairro" label="Bairro" required />
          <BaseField v-model="form.cidade" label="Cidade" required />
          <BaseField v-model="form.unidadeFederativa" label="UF" maxlength="2" required />
        </div>

        <h2 class="secao">Acesso</h2>
        <div class="grid grid--2">
          <BaseField v-model="form.username" label="Usuário" autocomplete="username" required />
          <div class="campo-arquivo">
            <label for="foto">Foto de perfil (opcional)</label>
            <input id="foto" type="file" accept="image/png,image/jpeg,image/webp,image/gif" @change="foto = $event.target.files[0] || null" />
          </div>
          <BaseField v-model="form.password" label="Senha" type="password" autocomplete="new-password" :error="erros.password" required />
          <BaseField v-model="form.confirmar" label="Confirmar senha" type="password" autocomplete="new-password" :error="erros.confirmar" required />
        </div>

        <BaseButton type="submit" size="lg" block :loading="enviando">Criar conta</BaseButton>
        <p class="texto-suave" style="margin: 0">Já tem conta? <RouterLink to="/entrar">Entrar</RouterLink></p>
      </form>
    </div>
  </div>
</template>

<style scoped>
.secao { font-size: var(--texto-lg); margin: var(--espaco-2) 0 0; }
.campo-arquivo { display: flex; flex-direction: column; gap: var(--espaco-1); font-size: var(--texto-sm); }
.campo-arquivo label { font-weight: 500; color: var(--cor-primaria); }
</style>
