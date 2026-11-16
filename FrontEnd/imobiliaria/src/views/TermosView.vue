<script setup>
import PageHeader from '@/components/ui/PageHeader.vue'

const secoes = [
  { titulo: '1. Sobre este serviço', texto: 'A Sweet Home é um projeto de demonstração. Os anúncios, preços e contatos exibidos não representam ofertas reais.' },
  { titulo: '2. Cadastro e conta', texto: 'Para anunciar é preciso criar uma conta com dados verdadeiros. Você é responsável por manter sua senha em sigilo e por tudo o que for feito com a sua conta.' },
  { titulo: '3. Anúncios', texto: 'Cada anunciante responde pela veracidade das informações, fotos e valores publicados. Anúncios que violem a lei ou direitos de terceiros podem ser removidos.' },
  { titulo: '4. Precificador', texto: 'O valor calculado é uma estimativa baseada em fatores simples. Ele não substitui a avaliação de um profissional habilitado.' },
  { titulo: '5. Privacidade', texto: 'Os dados informados no cadastro e nos formulários são usados apenas para o funcionamento da aplicação. Senhas são armazenadas de forma protegida (hash).' },
  { titulo: '6. Alterações', texto: 'Estes termos podem ser atualizados a qualquer momento. O uso continuado do serviço implica a aceitação da versão vigente.' },
]
</script>

<template>
  <div class="pagina pagina--estreita">
    <div class="container stack">
      <PageHeader titulo="Termos de uso" />
      <section v-for="s in secoes" :key="s.titulo" class="painel">
        <h2 style="font-size: var(--texto-lg)">{{ s.titulo }}</h2>
        <p style="margin: 0">{{ s.texto }}</p>
      </section>
    </div>
  </div>
</template>
