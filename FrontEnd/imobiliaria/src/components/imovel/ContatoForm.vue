<script setup>
import { reactive, ref } from 'vue'
import BaseField from '@/components/ui/BaseField.vue'
import BaseButton from '@/components/ui/BaseButton.vue'
import BaseAlert from '@/components/ui/BaseAlert.vue'
import { mensagensApi } from '@/services/api'

const props = defineProps({
  imovelId: { type: String, default: null },
  titulo: { type: String, default: 'Envie uma mensagem' },
  botao: { type: String, default: 'Enviar mensagem' },
})

const form = reactive({ nome: '', email: '', telefone: '', texto: '' })
const enviando = ref(false)
const feedback = ref(null)

async function enviar() {
  enviando.value = true
  feedback.value = null
  try {
    await mensagensApi.enviar({ ...form, imovelId: props.imovelId, origem: props.imovelId ? 'imovel' : 'contato' })
    feedback.value = { tipo: 'sucesso', texto: 'Mensagem enviada! Entraremos em contato em breve.' }
    Object.assign(form, { nome: '', email: '', telefone: '', texto: '' })
  } catch (e) {
    feedback.value = { tipo: 'erro', texto: e.message }
  } finally {
    enviando.value = false
  }
}
</script>

<template>
  <form class="stack" @submit.prevent="enviar">
    <h3>{{ titulo }}</h3>
    <BaseField v-model="form.nome" label="Nome" autocomplete="name" required />
    <BaseField v-model="form.email" label="E-mail" type="email" autocomplete="email" required />
    <BaseField v-model="form.telefone" label="Telefone" type="tel" autocomplete="tel" />
    <BaseField v-model="form.texto" label="Mensagem" as="textarea" required />
    <BaseButton type="submit" variant="destaque" block :loading="enviando">{{ botao }}</BaseButton>
    <BaseAlert v-if="feedback" :variant="feedback.tipo">{{ feedback.texto }}</BaseAlert>
    <p class="texto-suave" style="font-size: var(--texto-xs); margin: 0">
      Ao enviar você concorda com os <RouterLink to="/termos">termos de uso</RouterLink>.
    </p>
  </form>
</template>
