// Configuração base da API para comunicar com o backend em C#
const API_BASE_URL = 'https://localhost:7057/api';

/**
 * Tipos compartilhados com o Backend
 */
export interface UserReadDto {
  id: string;
  name: string;
  email: string;
}

export interface Paramedico {
  id: string;
  name: string;
  cpf: string;
  telefone: string;
  email: string;
  identificadorProfissional: string;
  cargo: string;
  ocupado: boolean;
}

export interface Ambulancia {
  id: string;
  placa: string;
  marca: string;
  modelo: string;
  tipo: string;
  status: number; // 0: Disponível, 1: Manutenção, 2: Em Uso
}

export interface ParamedicoCreateDTO {
  name: string;
  cpf: string;
  telefone: string;
  email: string;
  identificadorProfissional: string;
  cargo: string;
  ocupado: boolean;
}

export interface AmbulanciaCreateDTO {
  placa: string;
  marca: string;
  modelo: string;
  tipo: string;
}

/**
 * Utilitário de Fetch centralizado
 */
async function request<T>(endpoint: string, options?: RequestInit): Promise<T> {
  const token = localStorage.getItem('token');
  const headers = new Headers(options?.headers);
  headers.set('Content-Type', 'application/json');
  if (token) headers.set('Authorization', `Bearer ${token}`);

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    ...options,
    headers,
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new Error(errorData.message || 'Erro na requisição');
  }

  // Se o status for 204 No Content, retorna vazio (cast any to T)
  if (response.status === 204) return {} as T;

  return response.json();
}

/**
 * Endpoints
 */
export const authApi = {
  login: (data: any) => request<{ token: string; user: UserReadDto }>('/auth/login', { method: 'POST', body: JSON.stringify(data) }),
  me: () => request<UserReadDto>('/auth/me'),
};

export const paramedicosApi = {
  listar: (pagina = 1, quantidade = 50) => request<Paramedico[]>(`/paramedicos?pagina=${pagina}&quantidade=${quantidade}`),
  criar: (data: ParamedicoCreateDTO) => request<Paramedico>('/paramedicos', { method: 'POST', body: JSON.stringify(data) }),
  deletar: (id: string) => request<void>(`/paramedicos/${id}`, { method: 'DELETE' }),
};

export const ambulanciasApi = {
  listar: (pagina = 1, quantidade = 50) => request<Ambulancia[]>(`/ambulancias?pagina=${pagina}&quantidade=${quantidade}`),
  criar: (data: AmbulanciaCreateDTO) => request<Ambulancia>('/ambulancias', { method: 'POST', body: JSON.stringify(data) }),
  deletar: (id: string) => request<void>(`/ambulancias/${id}`, { method: 'DELETE' }),
};
