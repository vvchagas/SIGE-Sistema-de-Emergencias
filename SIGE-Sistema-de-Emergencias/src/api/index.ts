// Configuração base da API para comunicar com o backend em C#
const API_BASE_URL = 'https://localhost:7057'

/**
 * Tipos compartilhados com o Backend
 */

// Auth DTOs
export interface LoginRequestDto {
  email: string
  senha: string
}

export interface RegisterRequestDto {
  nome: string
  email: string
  cpf: string
  nascimento: string
  telefone: string
  cargo: string
  senha: string
}

export interface AutenticacaoRespostaDto {
  sucesso: boolean
  mensagem: string
  token: string
  nome: string
  cargo: string
  roles: string[]
  erros: string[] | null
}

export interface UserReadDto {
  id: string
  nome: string
  email: string
  cpf?: string
  telefone?: string
  cargo?: string
}

// Chamado DTOs
export interface ChamadoRequestDto {
  nomeSolicitante: string
  telefoneSolicitante: string
  tipoEmergencia: string
  descricao: string
  statusPaciente: number
  nivelPrioridade: number
  ambulanciaId: string
  paramedicos: string[]
  logradouro: string
  numero: number
  bairro: string
  cidade: string
  pontoReferencia?: string
}

export interface ChamadoAtualizarDto {
  status?: number
  descricao?: string
  observacoes?: string
}

export interface ChamadoGetDto {
  id: string
  nomeSolicitante: string
  telefoneSolicitante: string
  tipoEmergencia: string
  descricao: string
  statusChamado: number
  nivelPrioridade: number
  statusPaciente: number
  dataAbertura: string
  dataEncerramento?: string
  ambulanciaId: string
  paramedicos?: { id: string; name: string; cargo: string }[]
  logradouro: string
  numero: number
  bairro: string
  cidade: string
  pontoReferencia?: string
  observacoes?: string
}

// Paramedico DTOs
export interface ParamedicoCreateDTO {
  name: string
  cpf: string
  telefone: string
  email: string
  identificadorProfissional: string
  cargo: string
  ocupado: boolean
}

export interface ParamedicoGetDTO {
  id: string
  name: string
  cpf: string
  telefone: string
  email: string
  identificadorProfissional: string
  cargo: string
  ocupado: boolean
}

// Ambulancia DTOs
export interface AmbulanciaCreateDTO {
  placa: string
  marca: string
  modelo: string
  tipo: string
}

export interface AmbulanciaGetDTO {
  id: string
  placa: string
  marca: string
  modelo: string
  tipo: string
  status: number
  criadoEm: string
}

/**
 * Utilitário de Fetch centralizado
 */
async function request<T>(endpoint: string, options?: RequestInit): Promise<T> {
  const token = localStorage.getItem('token')
  const headers = new Headers(options?.headers)
  headers.set('Content-Type', 'application/json')
  if (token) headers.set('Authorization', `Bearer ${token}`)

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    ...options,
    headers,
  })

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}))
    let mensagem: string = errorData.mensagem || errorData.message || ''

    if (errorData.errors && !Array.isArray(errorData.errors) && typeof errorData.errors === 'object') {
      const detalhes = Object.values(errorData.errors).flat().join(' ')
      if (detalhes) mensagem = mensagem ? `${mensagem} ${detalhes}` : detalhes
    }

    const listaErros = Array.isArray(errorData.erros)
      ? errorData.erros
      : Array.isArray(errorData.errors)
        ? errorData.errors
        : null
    if (listaErros && listaErros.length) {
      const textos = listaErros
        .map((e: unknown) =>
          typeof e === 'string' ? e : (e as { description?: string })?.description ?? '',
        )
        .filter(Boolean)
        .join(' ')
      if (textos) mensagem = mensagem ? `${mensagem} ${textos}` : textos
    }

    if (!mensagem) mensagem = errorData.title || `Erro ${response.status}: ${response.statusText}`
    throw new Error(mensagem)
  }

  if (response.status === 204) return {} as T

  return response.json()
}

/**
 * Endpoints de Autenticação
 */
export const authApi = {
  login: (data: LoginRequestDto) =>
    request<AutenticacaoRespostaDto>('/auth/login', {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  registrar: (data: RegisterRequestDto) =>
    request<AutenticacaoRespostaDto>('/auth/registrar', {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  me: () => request<UserReadDto>('/auth/me'),
}

/**
 * Endpoints de Chamados
 */
export const chamadosApi = {
  listar: (pagina = 1, quantidade = 50): Promise<ChamadoGetDto[]> =>
    request<ChamadoGetDto[]>(`/chamados?pagina=${pagina}&quantidade=${quantidade}`),

  criar: (data: ChamadoRequestDto) =>
    request<ChamadoGetDto>('/chamados', {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  atualizar: (id: string, data: ChamadoAtualizarDto) =>
    request<ChamadoGetDto>(`/chamados/${id}`, {
      method: 'PATCH',
      body: JSON.stringify(data),
    }),

  obter: (id: string): Promise<ChamadoGetDto> =>
    request<ChamadoGetDto>(`/chamados/${id}`),

  despachar: (id: string, idAmbulancia: string) =>
    request<ChamadoGetDto>(`/chamados/${id}/despachar?idAmbulancia=${idAmbulancia}`, {
      method: 'POST',
    }),

  encerrar: (id: string) =>
    request<ChamadoGetDto>(`/chamados/${id}/encerrar`, { method: 'POST' }),
}

/**
 * Endpoints de Paramédicos
 */
export const paramedicosApi = {
  listar: (pagina = 1, quantidade = 50): Promise<ParamedicoGetDTO[]> =>
    request<ParamedicoGetDTO[]>(`/paramedicos?pagina=${pagina}&quantidade=${quantidade}`),

  criar: (data: ParamedicoCreateDTO) =>
    request<ParamedicoGetDTO>('/paramedicos', {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  atualizar: (id: string, data: Partial<ParamedicoCreateDTO>) =>
    request<ParamedicoGetDTO>(`/paramedicos/${id}`, {
      method: 'PATCH',
      body: JSON.stringify(data),
    }),

  deletar: (id: string) =>
    request<void>(`/paramedicos/${id}`, { method: 'DELETE' }),
}

/**
 * Endpoints de Ambulâncias
 */
export const ambulanciasApi = {
  listar: (pagina = 1, quantidade = 50): Promise<AmbulanciaGetDTO[]> =>
    request<AmbulanciaGetDTO[]>(`/ambulancias?pagina=${pagina}&quantidade=${quantidade}`),

  criar: (data: AmbulanciaCreateDTO) =>
    request<AmbulanciaGetDTO>('/ambulancias', {
      method: 'POST',
      body: JSON.stringify(data),
    }),

  atualizarStatus: (id: string, status: number) =>
    request<AmbulanciaGetDTO>(`/ambulancias/${id}/status?status=${status}`, { method: 'PATCH' }),

  deletar: (id: string) =>
    request<void>(`/ambulancias?id=${id}`, { method: 'DELETE' }),
}