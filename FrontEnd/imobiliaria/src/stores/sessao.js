import { reactive, computed } from 'vue'

const CHAVE = 'sweethome.usuario'

function carregar() {
  try {
    return JSON.parse(localStorage.getItem(CHAVE)) || null
  } catch {
    return null
  }
}

const estado = reactive({ usuario: carregar() })

export const sessao = {
  usuario: computed(() => estado.usuario),
  logado: computed(() => !!estado.usuario),
  admin: computed(() => estado.usuario?.tipo === 'Administrador'),

  entrar(usuario) {
    estado.usuario = usuario
    try { localStorage.setItem(CHAVE, JSON.stringify(usuario)) } catch { /* armazenamento indisponível */ }
  },
  atualizar(parcial) {
    this.entrar({ ...estado.usuario, ...parcial })
  },
  sair() {
    estado.usuario = null
    try { localStorage.removeItem(CHAVE) } catch { /* ignora */ }
  },
}
