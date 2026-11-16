const moeda = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL', maximumFractionDigits: 0 })
const numero = new Intl.NumberFormat('pt-BR')

export const formatarPreco = (valor, tipoServico) => {
  const texto = moeda.format(valor || 0)
  return String(tipoServico).toLowerCase() === 'alugar' ? `${texto}/mês` : texto
}

export const formatarArea = (valor) => `${numero.format(valor || 0)} m²`

export const rotuloServico = (tipoServico) =>
  String(tipoServico).toLowerCase() === 'alugar' ? 'Aluguel' : 'Venda'

export const TIPOS_IMOVEL = ['Apartamento', 'Casa', 'Comercial', 'Lote', 'Rural', 'Terreno']

export const imagemPadrao =
  "data:image/svg+xml;utf8," +
  encodeURIComponent(
    '<svg xmlns="http://www.w3.org/2000/svg" width="640" height="420" viewBox="0 0 640 420"><rect width="640" height="420" fill="#e6ecf1"/><path d="M220 230 320 150l100 80v70H220z" fill="#b8c7d2"/></svg>'
  )

// --- Validações de documentos ---
const soDigitos = (v) => String(v).replace(/\D/g, '')

export function validarCpf(valor) {
  const cpf = soDigitos(valor)
  if (cpf.length !== 11 || /^(\d)\1+$/.test(cpf)) return false
  for (const tamanho of [9, 10]) {
    let soma = 0
    for (let i = 0; i < tamanho; i++) soma += Number(cpf[i]) * (tamanho + 1 - i)
    const digito = ((soma * 10) % 11) % 10
    if (digito !== Number(cpf[tamanho])) return false
  }
  return true
}

export function validarCnpj(valor) {
  const cnpj = soDigitos(valor)
  if (cnpj.length !== 14 || /^(\d)\1+$/.test(cnpj)) return false
  for (const tamanho of [12, 13]) {
    const pesos = tamanho === 12 ? [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2] : [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]
    const soma = pesos.reduce((acc, peso, i) => acc + Number(cnpj[i]) * peso, 0)
    const resto = soma % 11
    const digito = resto < 2 ? 0 : 11 - resto
    if (digito !== Number(cnpj[tamanho])) return false
  }
  return true
}

export function mascararCpf(valor) {
  return soDigitos(valor).slice(0, 11)
    .replace(/(\d{3})(\d)/, '$1.$2').replace(/(\d{3})(\d)/, '$1.$2').replace(/(\d{3})(\d{1,2})$/, '$1-$2')
}

export function mascararCnpj(valor) {
  return soDigitos(valor).slice(0, 14)
    .replace(/^(\d{2})(\d)/, '$1.$2').replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3')
    .replace(/\.(\d{3})(\d)/, '.$1/$2').replace(/(\d{4})(\d)/, '$1-$2')
}

export function mascararCep(valor) {
  return soDigitos(valor).slice(0, 8).replace(/(\d{5})(\d)/, '$1-$2')
}
