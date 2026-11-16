<script setup>
import { ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import AppLogo from './AppLogo.vue'
import BaseButton from '@/components/ui/BaseButton.vue'
import { sessao } from '@/stores/sessao'

const route = useRoute()
const router = useRouter()
const aberto = ref(false)

const links = [
  { to: '/comprar', rotulo: 'Comprar' },
  { to: '/alugar', rotulo: 'Alugar' },
  { to: '/anunciar', rotulo: 'Anunciar' },
  { to: '/precificador', rotulo: 'Precificador' },
  { to: '/contato', rotulo: 'Contato' },
]

watch(() => route.fullPath, () => { aberto.value = false })

function sair() {
  sessao.sair()
  router.push('/')
}
</script>

<template>
  <header class="cabecalho">
    <div class="container cabecalho__barra">
      <RouterLink to="/" class="cabecalho__logo" aria-label="Sweet Home — início"><AppLogo /></RouterLink>

      <button class="cabecalho__menu" type="button" :aria-expanded="aberto" aria-controls="menu-principal" @click="aberto = !aberto">
        <span class="sr-only">Abrir menu</span>
        <svg viewBox="0 0 24 24" width="26" height="26" aria-hidden="true">
          <path v-if="!aberto" d="M4 7h16M4 12h16M4 17h16" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
          <path v-else d="M6 6l12 12M18 6 6 18" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
        </svg>
      </button>

      <nav id="menu-principal" class="cabecalho__nav" :class="{ 'cabecalho__nav--aberto': aberto }" aria-label="Principal">
        <ul class="cabecalho__links">
          <li v-for="l in links" :key="l.to">
            <RouterLink :to="l.to" class="cabecalho__link">{{ l.rotulo }}</RouterLink>
          </li>
        </ul>
        <div class="cabecalho__conta">
          <template v-if="sessao.logado.value">
            <BaseButton to="/conta" variant="secundario" size="sm">Olá, {{ sessao.usuario.value.username }}</BaseButton>
            <BaseButton variant="fantasma" size="sm" @click="sair">Sair</BaseButton>
          </template>
          <template v-else>
            <BaseButton to="/entrar" variant="fantasma" size="sm">Entrar</BaseButton>
            <BaseButton to="/cadastro" variant="primario" size="sm">Cadastre-se</BaseButton>
          </template>
        </div>
      </nav>
    </div>
  </header>
</template>

<style scoped>
.cabecalho { position: sticky; top: 0; z-index: 50; background: var(--cor-superficie); border-bottom: 1px solid var(--cor-borda); }
.cabecalho__barra { display: flex; align-items: center; justify-content: space-between; height: var(--altura-header); gap: var(--espaco-4); }
.cabecalho__logo { text-decoration: none; }
.cabecalho__menu { display: none; background: none; border: 0; color: var(--cor-primaria); cursor: pointer; padding: var(--espaco-2); }
.cabecalho__nav { display: flex; align-items: center; gap: var(--espaco-5); }
.cabecalho__links { list-style: none; display: flex; gap: var(--espaco-1); }
.cabecalho__link { display: block; padding: 0.4rem 0.8rem; border-radius: var(--raio-sm); text-decoration: none; font-weight: 600; font-size: var(--texto-sm); color: var(--cor-texto-suave); }
.cabecalho__link:hover { color: var(--cor-tinta); }
.cabecalho__link.router-link-active { color: var(--cor-tinta); box-shadow: inset 0 -3px 0 var(--cor-destaque); border-radius: 0; }
.cabecalho__conta { display: flex; align-items: center; gap: var(--espaco-2); }

@media (max-width: 960px) {
  .cabecalho__menu { display: inline-flex; }
  .cabecalho__nav {
    display: none; position: absolute; top: var(--altura-header); left: 0; right: 0;
    flex-direction: column; align-items: stretch; gap: var(--espaco-4);
    padding: var(--espaco-4); background: var(--cor-superficie); border-bottom: 1px solid var(--cor-borda); box-shadow: var(--sombra-md);
  }
  .cabecalho__nav--aberto { display: flex; }
  .cabecalho__links { flex-direction: column; }
  .cabecalho__conta { flex-wrap: wrap; }
}
</style>
