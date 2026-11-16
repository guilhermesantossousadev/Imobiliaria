<template>
  <div class="carregando" role="status">
    <span class="carregando__giro" aria-hidden="true" />
    <span><slot>Carregando…</slot></span>
  </div>
</template>

<style scoped>
.carregando { display: flex; align-items: center; justify-content: center; gap: var(--espaco-3); padding: var(--espaco-7); color: var(--cor-texto-suave); }
.carregando__giro { width: 22px; height: 22px; border-radius: 50%; border: 3px solid var(--cor-primaria-suave); border-top-color: var(--cor-primaria); animation: girar 800ms linear infinite; }
@keyframes girar { to { transform: rotate(360deg); } }
</style>
