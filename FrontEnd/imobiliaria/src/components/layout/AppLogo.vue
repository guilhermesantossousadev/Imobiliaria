<template>
  <span class="logo">
    <svg viewBox="0 0 32 32" width="34" height="34" aria-hidden="true">
      <rect width="32" height="32" rx="4" fill="var(--cor-primaria)" />
      <path d="M5 15 16 6l11 9v11H5z" fill="var(--cor-fundo)" />
      <path d="M5 15 16 6l11 9" fill="none" stroke="var(--cor-destaque)" stroke-width="2.4" stroke-linejoin="round" />
      <rect x="13.5" y="17" width="5" height="9" fill="var(--cor-verde)" />
    </svg>
    <span class="logo__texto">Sweet Home</span>
  </span>
</template>

<style scoped>
.logo { display: inline-flex; align-items: center; gap: var(--espaco-2); color: var(--cor-tinta); }
.logo__texto { font-family: var(--fonte-titulo); font-weight: 800; font-size: 1.35rem; letter-spacing: -0.03em; }
</style>
