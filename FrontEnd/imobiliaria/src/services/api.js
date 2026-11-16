// Cliente HTTP único do front-end. A URL base vem de VITE_API_URL (.env).
const BASE_URL = (import.meta.env.VITE_API_URL || 'http://localhost:5070').replace(/\/$/, '')

export class ApiError extends Error {
  constructor(message, status) {
    super(message)
    this.status = status
  }
}

async function request(path, { method = 'GET', body, formData } = {}) {
  const options = { method, headers: { Accept: 'application/json' } }
  if (formData) {
    options.body = formData
  } else if (body !== undefined) {
    options.headers['Content-Type'] = 'application/json'
    options.body = JSON.stringify(body)
  }

  let response
  try {
    response = await fetch(`${BASE_URL}${path}`, options)
  } catch {
    throw new ApiError('Não foi possível conectar ao servidor. Verifique se a API está rodando.', 0)
  }

  const texto = await response.text()
  let dados = null
  if (texto) {
    try { dados = JSON.parse(texto) } catch { dados = texto }
  }

  if (!response.ok) {
    const mensagem = (dados && typeof dados === 'object' && (dados.mensagem || dados.title)) || 'Ocorreu um erro inesperado.'
    throw new ApiError(mensagem, response.status)
  }
  return dados
}

const qs = (params) => new URLSearchParams(params).toString()

export const imoveisApi = {
  listar: () => request('/Imovel/ListarImoveis'),
  porId: (id) => request(`/Imovel/ListarImovelPorId?${qs({ id })}`),
  porTipoServico: (tipoServico) => request(`/Imovel/ListarImoveisPorTipoServico?${qs({ tipoServico })}`),
  adicionar: (imovel) => request('/Imovel/AdicionarImovel', { method: 'POST', body: imovel }),
  enviarImagem: (imovelId, arquivo) => {
    const formData = new FormData()
    formData.append('imagem', arquivo)
    return request(`/Imovel/UploadImage?${qs({ imovelId })}`, { method: 'PUT', formData })
  },
  remover: (id) => request(`/Imovel/RemoverImovel?${qs({ id })}`, { method: 'DELETE' }),
  precificar: (dados) => request('/Imovel/PrecificarImovel', { method: 'POST', body: dados }),
}

export const usuariosApi = {
  entrar: (username, password) => request('/Usuario/LogarUsuario', { method: 'POST', body: { username, password } }),
  cadastrar: (dados) => request('/Usuario/AdicionarUsuario', { method: 'POST', body: dados }),
  enviarFoto: (usuarioId, arquivo) => {
    const formData = new FormData()
    formData.append('imagem', arquivo)
    return request(`/Usuario/UploadImage?${qs({ usuarioId })}`, { method: 'PUT', formData })
  },
  trocarUsername: (id, valor) => request('/Usuario/EditarUsername', { method: 'PUT', body: { id, valor } }),
  trocarSenha: (id, valor) => request('/Usuario/EditarSenha', { method: 'PUT', body: { id, valor } }),
  remover: (id) => request(`/Usuario/RemoverUsuario?${qs({ id })}`, { method: 'DELETE' }),
  porId: (id) => request(`/Usuario/ListarUsuarioPorId?${qs({ id })}`),
}

export const mensagensApi = {
  enviar: (dados) => request('/Mensagem/EnviarMensagem', { method: 'POST', body: dados }),
  listar: () => request('/Mensagem/ListarMensagens'),
}

// Consulta pública de CEP (ViaCEP). Retorna null se não encontrar.
export async function buscarCep(cep) {
  const limpo = String(cep).replace(/\D/g, '')
  if (limpo.length !== 8) return null
  try {
    const resposta = await fetch(`https://viacep.com.br/ws/${limpo}/json/`)
    const dados = await resposta.json()
    return dados.erro ? null : dados
  } catch {
    return null
  }
}
