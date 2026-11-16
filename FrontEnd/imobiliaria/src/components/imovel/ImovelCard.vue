<script setup>
import { computed } from 'vue'
import BaseBadge from '@/components/ui/BaseBadge.vue'
import { formatarPreco, formatarArea, rotuloServico, imagemPadrao } from '@/utils/formatar'

const props = defineProps({ imovel: { type: Object, required: true } })

const foto = computed(() => props.imovel.urlsImagens?.[0] || imagemPadrao)
const aluguel = computed(() => props.imovel.tipoServico?.toLowerCase() === 'alugar')
const detalhes = computed(() => {
  const i = props.imovel
  return [
    i.quartos ? `${i.quartos} ${i.quartos === 1 ? 'quarto' : 'quartos'}` : null,
    i.banheiros ? `${i.banheiros} ${i.banheiros === 1 ? 'banheiro' : 'banheiros'}` : null,
    i.vagas ? `${i.vagas} ${i.vagas === 1 ? 'vaga' : 'vagas'}` : null,
  ].filter(Boolean)
})
</script>

<template>
  <RouterLink :to="{ name: 'imovel', params: { id: imovel.id } }" class="cartao">
    <div class="cartao__foto">
      <img :src="foto" :alt="imovel.titulo" loading="lazy" @error="$event.target.src = imagemPadrao" />
      <BaseBadge class="cartao__selo" :variant="aluguel ? 'verde' : 'destaque'">{{ rotuloServico(imovel.tipoServico) }}</BaseBadge>
    </div>
    <div class="cartao__corpo">
      <p class="cartao__preco">{{ formatarPreco(imovel.preco, imovel.tipoServico) }}</p>
      <h3 class="cartao__titulo">{{ imovel.titulo }}</h3>
      <p class="cartao__local">{{ imovel.bairro }}, {{ imovel.cidade }}/{{ imovel.unidadeFederativa }}</p>
      <p class="cartao__meta">
        <span>{{ imovel.tipo }}</span>
        <span>{{ formatarArea(imovel.area) }}</span>
        <span v-for="d in detalhes" :key="d">{{ d }}</span>
      </p>
    </div>
  </RouterLink>
</template>

<style scoped>
.cartao { display: flex; flex-direction: column; text-decoration: none; color: inherit; }
/* A foto tem o recorte de uma empena de casa */
.cartao__foto { position: relative; aspect-ratio: 16 / 11; background: var(--cor-primaria-suave); clip-path: polygon(0 18%, 50% 0, 100% 18%, 100% 100%, 0 100%); }
.cartao__foto img { width: 100%; height: 100%; object-fit: cover; transition: transform 400ms ease; }
.cartao:hover .cartao__foto img { transform: scale(1.04); }
.cartao__selo { position: absolute; bottom: var(--espaco-3); left: var(--espaco-3); }
.cartao__corpo { flex: 1; display: flex; flex-direction: column; gap: var(--espaco-1); padding: var(--espaco-4); background: var(--cor-superficie); border: 1px solid var(--cor-borda); border-top: 3px solid var(--cor-primaria); }
.cartao__preco { margin: 0; font-family: var(--fonte-titulo); font-size: var(--texto-xl); font-weight: 800; letter-spacing: -0.02em; color: var(--cor-tinta); }
.cartao__titulo { margin: 0; font-size: var(--texto-md); font-weight: 600; letter-spacing: 0; line-height: 1.3; display: -webkit-box; -webkit-line-clamp: 2; line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; }
.cartao:hover .cartao__titulo { text-decoration: underline; text-underline-offset: 3px; }
.cartao__local { margin: 0; font-size: var(--texto-sm); color: var(--cor-texto-suave); }
.cartao__meta { margin: auto 0 0; padding-top: var(--espaco-3); display: flex; flex-wrap: wrap; gap: var(--espaco-1) var(--espaco-3); font-size: var(--texto-xs); color: var(--cor-texto-suave); }
.cartao__meta span + span { padding-left: var(--espaco-3); border-left: 1px solid var(--cor-borda); }
</style>
