import { createRouter, createWebHistory } from 'vue-router'
import { sessao } from '@/stores/sessao'

const routes = [
  { path: '/', name: 'home', component: () => import('@/views/HomeView.vue'), meta: { titulo: 'Início' } },
  { path: '/imoveis', name: 'imoveis', component: () => import('@/views/ImoveisView.vue'), meta: { titulo: 'Imóveis' } },
  { path: '/comprar', name: 'comprar', component: () => import('@/views/ImoveisView.vue'), props: { servico: 'Comprar' }, meta: { titulo: 'Comprar' } },
  { path: '/alugar', name: 'alugar', component: () => import('@/views/ImoveisView.vue'), props: { servico: 'Alugar' }, meta: { titulo: 'Alugar' } },
  { path: '/imovel/:id', name: 'imovel', component: () => import('@/views/ImovelDetalheView.vue'), props: true, meta: { titulo: 'Imóvel' } },
  { path: '/anunciar', name: 'anunciar', component: () => import('@/views/AnunciarView.vue'), meta: { titulo: 'Anunciar imóvel', requerLogin: true } },
  { path: '/precificador', name: 'precificador', component: () => import('@/views/PrecificadorView.vue'), meta: { titulo: 'Precificador' } },
  { path: '/entrar', name: 'entrar', component: () => import('@/views/LoginView.vue'), meta: { titulo: 'Entrar' } },
  { path: '/cadastro', name: 'cadastro', component: () => import('@/views/CadastroView.vue'), meta: { titulo: 'Cadastro' } },
  { path: '/conta', name: 'conta', component: () => import('@/views/ContaView.vue'), meta: { titulo: 'Minha conta', requerLogin: true } },
  { path: '/contato', name: 'contato', component: () => import('@/views/ContatoView.vue'), meta: { titulo: 'Contato' } },
  { path: '/sobre', name: 'sobre', component: () => import('@/views/SobreView.vue'), meta: { titulo: 'Sobre' } },
  { path: '/termos', name: 'termos', component: () => import('@/views/TermosView.vue'), meta: { titulo: 'Termos de uso' } },
  { path: '/:pathMatch(.*)*', name: 'nao-encontrada', component: () => import('@/views/NaoEncontradaView.vue'), meta: { titulo: 'Página não encontrada' } },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior: (to, from, salvo) => salvo || { top: 0 },
})

router.beforeEach((to) => {
  if (to.meta.requerLogin && !sessao.logado.value) {
    return { name: 'entrar', query: { redirecionar: to.fullPath } }
  }
})

router.afterEach((to) => {
  document.title = `${to.meta.titulo ?? 'Sweet Home'} — Sweet Home`
})

export default router
