<script setup>
import EmptyState from '@/components/ui/EmptyState.vue'
import BaseButton from '@/components/ui/BaseButton.vue'
</script>

<template>
  <div class="pagina">
    <div class="container">
      <EmptyState titulo="Página não encontrada">
        O endereço que você tentou acessar não existe.
      </EmptyState>
      <div class="cluster" style="justify-content: center"><BaseButton to="/">Voltar ao início</BaseButton></div>
    </div>
  </div>
</template>
