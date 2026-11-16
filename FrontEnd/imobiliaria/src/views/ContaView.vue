<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import PageHeader from '@/components/ui/PageHeader.vue'
import BaseField from '@/components/ui/BaseField.vue'
import BaseButton from '@/components/ui/BaseButton.vue'
import BaseAlert from '@/components/ui/BaseAlert.vue'
import BaseBadge from '@/components/ui/BaseBadge.vue'
import { usuariosApi, imoveisApi, mensagensApi } from '@/services/api'
import { sessao } from '@/stores/sessao'
import { formatarPreco } from '@/utils/formatar'

const router = useRouter()
const usuario = computed(() => sessao.usuario.value)

const novoUsername = ref(usuario.value?.username || '')
const novaSenha = reactive({ valor: '', confirmar: '' })
const feedback = reactive({ perfil: null, senha: null, foto: null })
const confirmandoExclusao = ref(false)

const dizer = (chave, tipo, texto) => { feedback[chave] = { tipo, texto } }

async function trocarUsername() {
  try {
    await usuariosApi.trocarUsername(usuario.value.id, novoUsername.value)
    sessao.atualizar({ username: novoUsername.value })
    dizer('perfil', 'sucesso', 'Nome de usuário atualizado.')
  } catch (e) { dizer('perfil', 'erro', e.message) }
}

async function trocarSenha() {
  if (novaSenha.valor.length < 6) return dizer('senha', 'erro', 'Use pelo menos 6 caracteres.')
  if (novaSenha.valor !== novaSenha.confirmar) return dizer('senha', 'erro', 'As senhas não conferem.')
  try {
    await usuariosApi.trocarSenha(usuario.value.id, novaSenha.valor)
    novaSenha.valor = novaSenha.confirmar = ''
    dizer('senha', 'sucesso', 'Senha alterada.')
  } catch (e) { dizer('senha', 'erro', e.message) }
}

async function trocarFoto(evento) {
  const arquivo = evento.target.files[0]
  if (!arquivo) return
  try {
    const url = await usuariosApi.enviarFoto(usuario.value.id, arquivo)
    sessao.atualizar({ fotoDePerfilURL: url })
    dizer('foto', 'sucesso', 'Foto atualizada.')
  } catch (e) { dizer('foto', 'erro', e.message) }
}

async function excluirConta() {
  try {
    await usuariosApi.remover(usuario.value.id)
    sessao.sair()
    router.push('/')
  } catch (e) {
    confirmandoExclusao.value = false
    dizer('perfil', 'erro', e.message)
  }
}

function sair() {
  sessao.sair()
  router.push('/')
}

// --- Área do administrador ---
const imoveis = ref([])
const mensagens = ref([])
const erroAdmin = ref('')

async function carregarAdmin() {
  try {
    ;[imoveis.value, mensagens.value] = await Promise.all([imoveisApi.listar(), mensagensApi.listar()])
  } catch (e) { erroAdmin.value = e.message }
}

async function removerImovel(imovel) {
  if (!confirm(`Remover o anúncio "${imovel.titulo}"?`)) return
  try {
    await imoveisApi.remover(imovel.id)
    imoveis.value = imoveis.value.filter((i) => i.id !== imovel.id)
  } catch (e) { erroAdmin.value = e.message }
}

onMounted(() => { if (sessao.admin.value) carregarAdmin() })
</script>

<template>
  <div class="pagina">
    <div v-if="usuario" class="container stack">
      <PageHeader titulo="Minha conta" />

      <section class="painel perfil">
        <img :src="usuario.fotoDePerfilURL" alt="Foto de perfil" class="perfil__foto" />
        <div class="stack perfil__dados">
          <div>
            <h2 style="margin: 0">{{ usuario.nome }}</h2>
            <p class="texto-suave" style="margin: 0">@{{ usuario.username }} · <BaseBadge>{{ usuario.tipo }}</BaseBadge></p>
          </div>
          <p class="texto-suave" style="margin: 0">{{ usuario.endereco }}</p>
          <div class="cluster">
            <label class="btn-arquivo">
              Trocar foto
              <input type="file" accept="image/png,image/jpeg,image/webp,image/gif" @change="trocarFoto" />
            </label>
            <BaseButton variant="fantasma" size="sm" @click="sair">Sair</BaseButton>
            <BaseButton to="/anunciar" variant="destaque" size="sm">Anunciar imóvel</BaseButton>
          </div>
          <BaseAlert v-if="feedback.foto" :variant="feedback.foto.tipo">{{ feedback.foto.texto }}</BaseAlert>
        </div>
      </section>

      <div class="grid grid--2">
        <form class="painel stack" @submit.prevent="trocarUsername">
          <h2>Nome de usuário</h2>
          <BaseField v-model="novoUsername" label="Novo usuário" required />
          <BaseButton type="submit">Salvar</BaseButton>
          <BaseAlert v-if="feedback.perfil" :variant="feedback.perfil.tipo">{{ feedback.perfil.texto }}</BaseAlert>
        </form>

        <form class="painel stack" @submit.prevent="trocarSenha">
          <h2>Alterar senha</h2>
          <BaseField v-model="novaSenha.valor" label="Nova senha" type="password" autocomplete="new-password" required />
          <BaseField v-model="novaSenha.confirmar" label="Confirmar nova senha" type="password" autocomplete="new-password" required />
          <BaseButton type="submit">Alterar senha</BaseButton>
          <BaseAlert v-if="feedback.senha" :variant="feedback.senha.tipo">{{ feedback.senha.texto }}</BaseAlert>
        </form>
      </div>

      <section v-if="sessao.admin.value" class="painel stack">
        <h2>Administração</h2>
        <BaseAlert v-if="erroAdmin" variant="erro">{{ erroAdmin }}</BaseAlert>

        <h3>Mensagens recebidas ({{ mensagens.length }})</h3>
        <ul v-if="mensagens.length" class="lista">
          <li v-for="m in mensagens" :key="m.id">
            <div class="cluster">
              <BaseBadge>{{ m.origem }}</BaseBadge>
              <strong>{{ m.nome || m.email }}</strong>
              <span class="texto-suave">{{ m.email }} {{ m.telefone }}</span>
            </div>
            <p style="margin: var(--espaco-1) 0 0">{{ m.texto }}</p>
            <RouterLink v-if="m.imovelId" :to="{ name: 'imovel', params: { id: m.imovelId } }">Ver imóvel</RouterLink>
          </li>
        </ul>
        <p v-else class="texto-suave">Nenhuma mensagem ainda.</p>

        <h3>Anúncios ({{ imoveis.length }})</h3>
        <ul class="lista">
          <li v-for="i in imoveis" :key="i.id" class="lista__linha">
            <RouterLink :to="{ name: 'imovel', params: { id: i.id } }">{{ i.titulo }}</RouterLink>
            <span class="texto-suave">{{ formatarPreco(i.preco, i.tipoServico) }}</span>
            <BaseButton variant="perigo" size="sm" @click="removerImovel(i)">Remover</BaseButton>
          </li>
        </ul>
      </section>

      <section class="painel stack zona-perigo">
        <h2>Excluir conta</h2>
        <p class="texto-suave" style="margin: 0">Essa ação é permanente e remove seus dados de acesso.</p>
        <div v-if="!confirmandoExclusao"><BaseButton variant="secundario" @click="confirmandoExclusao = true">Excluir minha conta</BaseButton></div>
        <div v-else class="cluster">
          <strong>Tem certeza?</strong>
          <BaseButton variant="perigo" @click="excluirConta">Sim, excluir</BaseButton>
          <BaseButton variant="fantasma" @click="confirmandoExclusao = false">Cancelar</BaseButton>
        </div>
      </section>
    </div>
  </div>
</template>

<style scoped>
.perfil { display: flex; gap: var(--espaco-5); align-items: center; flex-wrap: wrap; }
.perfil__foto { width: 110px; height: 110px; border-radius: 50%; object-fit: cover; border: 3px solid var(--cor-primaria-suave); }
.perfil__dados { flex: 1; min-width: 240px; gap: var(--espaco-2); }
.btn-arquivo { display: inline-block; cursor: pointer; padding: 0.4rem 0.9rem; font-size: var(--texto-sm); font-weight: 600; border: 1px solid var(--cor-borda); border-radius: var(--raio-sm); color: var(--cor-primaria); background: var(--cor-superficie); }
.btn-arquivo:hover { border-color: var(--cor-primaria); }
.btn-arquivo input { display: none; }
.lista { list-style: none; display: flex; flex-direction: column; gap: var(--espaco-3); }
.lista > li { padding-bottom: var(--espaco-3); border-bottom: 1px solid var(--cor-borda); }
.lista__linha { display: flex; align-items: center; justify-content: space-between; gap: var(--espaco-3); flex-wrap: wrap; }
.zona-perigo { border-color: color-mix(in srgb, var(--cor-erro) 30%, var(--cor-borda)); }
h2 { font-size: var(--texto-lg); margin: 0; }
</style>
