<script setup>
// Casario colonial desenhado em SVG: cada casa é descrita por posição, tamanho,
// cor da parede e tipo de telhado. As janelas e a porta são calculadas.
const CHAO = 200
const CORES = { cal: '#f5f6f2', ocre: '#e0a526', verde: '#1f6b54', ceu: '#9db4e8', rosa: '#e7b7a8' }

const casas = [
  { x: 0, w: 130, h: 92, cor: 'ocre', telhado: 'duas-aguas' },
  { x: 130, w: 150, h: 150, cor: 'cal', telhado: 'duas-aguas', andares: 2 },
  { x: 280, w: 110, h: 100, cor: 'verde', telhado: 'platibanda' },
  { x: 390, w: 170, h: 170, cor: 'ceu', telhado: 'duas-aguas', andares: 2 },
  { x: 560, w: 120, h: 96, cor: 'cal', telhado: 'duas-aguas' },
  { x: 680, w: 150, h: 140, cor: 'rosa', telhado: 'platibanda', andares: 2 },
  { x: 830, w: 130, h: 104, cor: 'ocre', telhado: 'duas-aguas' },
  { x: 960, w: 160, h: 158, cor: 'cal', telhado: 'duas-aguas', andares: 2 },
  { x: 1120, w: 140, h: 96, cor: 'verde', telhado: 'duas-aguas' },
].map((c, i) => {
  const topo = CHAO - c.h
  const andares = c.andares || 1
  const alturaAndar = c.h / andares
  let colunas = Math.max(3, Math.floor(c.w / 52))
  if (colunas % 2 === 0) colunas++ // sempre ímpar: a porta fica no eixo central
  const passo = c.w / colunas
  const janelas = []
  const porta = { x: c.x + c.w / 2 - 11, y: CHAO - 46, w: 22, h: 46 }

  for (let a = 0; a < andares; a++) {
    const yBase = topo + a * alturaAndar + alturaAndar * 0.22
    for (let k = 0; k < colunas; k++) {
      const cx = c.x + passo * k + passo / 2
      const noTerreo = a === andares - 1
      // no térreo, a coluna central é ocupada pela porta
      if (noTerreo && k === (colunas - 1) / 2) continue
      janelas.push({ x: cx - 9, y: yBase, w: 18, h: 28 })
    }
  }
  const beiral = 8
  const telhado = c.telhado === 'duas-aguas'
    ? `${c.x - beiral},${topo} ${c.x + c.w / 2},${topo - c.w * 0.2} ${c.x + c.w + beiral},${topo}`
    : null
  return { ...c, id: i, topo, janelas, porta, telhado, fill: CORES[c.cor], beiral }
})
</script>

<template>
  <svg class="casario" viewBox="0 0 1260 210" preserveAspectRatio="xMidYMax slice" aria-hidden="true" focusable="false">
    <g v-for="c in casas" :key="c.id" class="casa" :style="{ '--i': c.id }">
      <rect :x="c.x" :y="c.topo" :width="c.w" :height="c.h" :fill="c.fill" stroke="#101a33" stroke-width="1.5" />
      <polygon v-if="c.telhado" :points="c.telhado" fill="#101a33" />
      <rect v-else :x="c.x - 3" :y="c.topo - 10" :width="c.w + 6" height="12" fill="#101a33" />
      <rect v-for="(j, k) in c.janelas" :key="k" :x="j.x" :y="j.y" :width="j.w" :height="j.h" fill="#f5f6f2" stroke="#1b3fa0" stroke-width="3.5" />
      <rect :x="c.porta.x" :y="c.porta.y" :width="c.porta.w" :height="c.porta.h" fill="#1b3fa0" />
      <circle :cx="c.porta.x + c.porta.w - 5" :cy="c.porta.y + c.porta.h / 2" r="1.6" fill="#e0a526" />
    </g>
    <rect x="0" y="198" width="1260" height="12" fill="#101a33" />
  </svg>
</template>

<style scoped>
.casario { display: block; width: 100%; height: auto; max-height: 210px; }
.casa { animation: erguer 700ms cubic-bezier(0.2, 0.7, 0.2, 1) both; animation-delay: calc(var(--i) * 70ms); transform-box: fill-box; }
@keyframes erguer { from { opacity: 0; transform: translateY(24px); } to { opacity: 1; transform: none; } }
</style>
