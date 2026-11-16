<script setup>
import PageHeader from '@/components/ui/PageHeader.vue'
import BaseButton from '@/components/ui/BaseButton.vue'
</script>

<template>
  <div class="pagina pagina--estreita">
    <div class="container stack">
      <PageHeader titulo="Sobre a Sweet Home" subtitulo="Um portal de imóveis simples, claro e feito para quem quer encontrar ou anunciar um lar." />
      <section class="painel stack">
        <p>
          A Sweet Home reúne anúncios de imóveis para compra e aluguel em um só lugar, com busca por tipo,
          localização e características, fotos, contato direto com o anunciante e um precificador para
          ajudar quem vai anunciar.
        </p>
        <p>
          Este é um projeto de portfólio: os anúncios de exemplo e a empresa mostrados no site são fictícios
          e servem para demonstrar a aplicação funcionando de ponta a ponta (front-end em Vue, API em .NET e
          banco SQLite).
        </p>
        <div class="cluster">
          <BaseButton to="/comprar" variant="primario">Ver imóveis</BaseButton>
          <BaseButton to="/contato" variant="secundario">Fale conosco</BaseButton>
        </div>
      </section>
    </div>
  </div>
</template>
