<script setup>
import { ref } from 'vue'
import AppLogo from './AppLogo.vue'
import BaseButton from '@/components/ui/BaseButton.vue'
import BaseField from '@/components/ui/BaseField.vue'
import BaseAlert from '@/components/ui/BaseAlert.vue'
import { mensagensApi } from '@/services/api'

const email = ref('')
const enviando = ref(false)
const feedback = ref(null)

async function assinar() {
  enviando.value = true
  feedback.value = null
  try {
    await mensagensApi.enviar({ email: email.value, origem: 'newsletter', texto: 'Inscrição na newsletter' })
    feedback.value = { tipo: 'sucesso', texto: 'Inscrição feita! Você receberá as novidades.' }
    email.value = ''
  } catch (e) {
    feedback.value = { tipo: 'erro', texto: e.message }
  } finally {
    enviando.value = false
  }
}
</script>

<template>
  <footer class="rodape">
    <div class="container rodape__grade">
      <div class="rodape__marca">
        <AppLogo />
        <p class="texto-suave">Imóveis para comprar, alugar e anunciar.</p>
      </div>

      <nav aria-label="Rodapé" class="rodape__links">
        <RouterLink to="/comprar">Comprar</RouterLink>
        <RouterLink to="/alugar">Alugar</RouterLink>
        <RouterLink to="/anunciar">Anunciar</RouterLink>
        <RouterLink to="/sobre">Sobre</RouterLink>
        <RouterLink to="/termos">Termos de uso</RouterLink>
      </nav>

      <form class="rodape__news" @submit.prevent="assinar">
        <strong>Receba novidades</strong>
        <div class="rodape__news-linha">
          <BaseField v-model="email" label="E-mail" type="email" placeholder="seu@email.com" required />
          <BaseButton type="submit" variant="destaque" :loading="enviando">Assinar</BaseButton>
        </div>
        <BaseAlert v-if="feedback" :variant="feedback.tipo">{{ feedback.texto }}</BaseAlert>
      </form>
    </div>
    <div class="rodape__fim container texto-suave">
      © {{ new Date().getFullYear() }} Sweet Home — projeto de portfólio.
    </div>
  </footer>
</template>

<style scoped>
.rodape { background: var(--cor-tinta); color: var(--cor-primaria-suave); padding-top: var(--espaco-7); }
.rodape :deep(.logo) { color: var(--cor-fundo); }
.rodape :deep(.campo__rotulo) { color: var(--cor-primaria-suave); }
.rodape :deep(.texto-suave), .rodape .texto-suave { color: var(--cor-ceu); }
.rodape__news strong { color: var(--cor-fundo); font-family: var(--fonte-titulo); font-size: var(--texto-lg); }
.rodape__grade { display: grid; grid-template-columns: 1.2fr 1fr 1.4fr; gap: var(--espaco-6); }
.rodape__links { display: flex; flex-direction: column; gap: var(--espaco-2); }
.rodape__links a { text-decoration: none; color: var(--cor-primaria-suave); }
.rodape__links a:hover { color: var(--cor-destaque); }
.rodape__news { display: flex; flex-direction: column; gap: var(--espaco-2); }
.rodape__news-linha { display: flex; align-items: flex-end; gap: var(--espaco-2); }
.rodape__news-linha > :first-child { flex: 1; }
.rodape__fim { margin-top: var(--espaco-6); padding-block: var(--espaco-5); font-size: var(--texto-sm); border-top: 1px solid rgba(255,255,255,0.12); }
@media (max-width: 800px) { .rodape__grade { grid-template-columns: 1fr; } }
</style>
