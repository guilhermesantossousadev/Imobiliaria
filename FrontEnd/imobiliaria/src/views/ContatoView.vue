<script setup>
import PageHeader from '@/components/ui/PageHeader.vue'
import ContatoForm from '@/components/imovel/ContatoForm.vue'
</script>

<template>
  <div class="pagina pagina--estreita">
    <div class="container">
      <PageHeader titulo="Fale com a gente" subtitulo="Dúvidas, sugestões ou parcerias? Envie uma mensagem e responderemos em breve." />
      <div class="painel">
        <ContatoForm titulo="Envie sua mensagem" />
      </div>
    </div>
  </div>
</template>
