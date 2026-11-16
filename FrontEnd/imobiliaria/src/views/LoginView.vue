<script setup>
import { reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import BaseField from '@/components/ui/BaseField.vue'
import BaseButton from '@/components/ui/BaseButton.vue'
import BaseAlert from '@/components/ui/BaseAlert.vue'
import { usuariosApi } from '@/services/api'
import { sessao } from '@/stores/sessao'

const route = useRoute()
const router = useRouter()

const form = reactive({ username: '', password: '' })
const carregando = ref(false)
const erro = ref('')

async function entrar() {
  carregando.value = true
  erro.value = ''
  try {
    const usuario = await usuariosApi.entrar(form.username, form.password)
    sessao.entrar(usuario)
    router.push(route.query.redirecionar || '/')
  } catch (e) {
    erro.value = e.status === 401 ? 'Usuário ou senha inválidos.' : e.message
  } finally {
    carregando.value = false
  }
}
</script>

<template>
  <div class="pagina pagina--estreita">
    <div class="container">
      <form class="painel stack" @submit.prevent="entrar">
        <h1>Entrar</h1>
        <BaseAlert v-if="erro" variant="erro">{{ erro }}</BaseAlert>
        <BaseField v-model="form.username" label="Usuário" autocomplete="username" required />
        <BaseField v-model="form.password" label="Senha" type="password" autocomplete="current-password" required />
        <BaseButton type="submit" size="lg" block :loading="carregando">Entrar</BaseButton>
        <p class="texto-suave" style="margin: 0">
          Ainda não tem conta? <RouterLink to="/cadastro">Cadastre-se</RouterLink>
        </p>
      </form>
    </div>
  </div>
</template>
