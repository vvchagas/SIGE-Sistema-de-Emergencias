<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import {
  paramedicosApi,
  ambulanciasApi,
  type ParamedicoGetDTO,
  type AmbulanciaGetDTO,
} from '../api/index'

const sidebarAberta = ref(false)
const mostrarModalSair = ref(false)
const router = useRouter()
const usuarioNome = ref(localStorage.getItem('usuario_nome') || 'Usuário')

type Aba = 'paramedicos' | 'ambulancias'
const abaAtiva = ref<Aba>('paramedicos')

// ---- Toast de sucesso ----
const toast = ref<string | null>(null)
function mostrarToast(msg: string) {
  toast.value = msg
  setTimeout(() => (toast.value = null), 2500)
}

// ============ PARAMÉDICOS ============
const paramedicos = ref<ParamedicoGetDTO[]>([])
const carregandoParam = ref(true)
const erroParam = ref<string | null>(null)
const salvandoParam = ref(false)
const erroSalvarParam = ref<string | null>(null)

const novoParamedico = ref({
  name: '',
  cpf: '',
  telefone: '',
  email: '',
  identificadorProfissional: '',
  cargo: '',
})

const cargoConfig: Record<string, { label: string; badgeClass: string }> = {
  medico: { label: 'Médico', badgeClass: 'bg-blue-100 text-blue-700' },
  enfermeiro: { label: 'Enfermeiro', badgeClass: 'bg-emerald-100 text-emerald-700' },
  condutor: { label: 'Condutor', badgeClass: 'bg-amber-100 text-amber-700' },
  paramedico: { label: 'Paramédico', badgeClass: 'bg-indigo-100 text-indigo-700' },
}

function cargoDe(cargo: string) {
  return cargoConfig[cargo] ?? { label: cargo || 'Não definido', badgeClass: 'bg-slate-100 text-slate-600' }
}

function nomeDe(p: ParamedicoGetDTO) {
  return p.nome ?? (p as { name?: string }).name ?? ''
}

function iniciaisDe(nome: string | undefined | null) {
  const limpo = (nome ?? '').trim()
  if (!limpo) return '?'
  const partes = limpo.split(/\s+/)
  const primeira = partes[0]?.[0] ?? ''
  const ultima = partes.length > 1 ? partes[partes.length - 1]?.[0] ?? '' : ''
  return (primeira + ultima).toUpperCase()
}

function idCurto(id: string | undefined | null) {
  return (id ?? '').replace(/-/g, '').slice(0, 4).toUpperCase()
}

async function carregarParamedicos() {
  carregandoParam.value = true
  erroParam.value = null
  try {
    paramedicos.value = await paramedicosApi.listar()
  } catch (e: unknown) {
    erroParam.value = e instanceof Error ? e.message : 'Erro ao carregar paramédicos'
  } finally {
    carregandoParam.value = false
  }
}

async function salvarParamedico() {
  salvandoParam.value = true
  erroSalvarParam.value = null
  try {
    await paramedicosApi.criar({
      name: novoParamedico.value.name,
      cpf: novoParamedico.value.cpf,
      telefone: novoParamedico.value.telefone,
      email: novoParamedico.value.email,
      identificadorProfissional: novoParamedico.value.identificadorProfissional,
      cargo: novoParamedico.value.cargo,
      ocupado: false,
    })
    await carregarParamedicos()
    novoParamedico.value = {
      name: '',
      cpf: '',
      telefone: '',
      email: '',
      identificadorProfissional: '',
      cargo: '',
    }
    mostrarToast('Paramédico cadastrado com sucesso!')
  } catch (e: unknown) {
    erroSalvarParam.value = e instanceof Error ? e.message : 'Erro ao cadastrar paramédico'
  } finally {
    salvandoParam.value = false
  }
}

async function deletarParamedico(id: string) {
  if (!confirm('Tem certeza que deseja remover este paramédico?')) return
  try {
    await paramedicosApi.deletar(id)
    await carregarParamedicos()
    mostrarToast('Paramédico removido.')
  } catch (e: unknown) {
    alert(e instanceof Error ? e.message : 'Erro ao remover paramédico')
  }
}

// ============ AMBULÂNCIAS ============
const ambulancias = ref<AmbulanciaGetDTO[]>([])
const carregandoAmb = ref(true)
const erroAmb = ref<string | null>(null)
const salvandoAmb = ref(false)
const erroSalvarAmb = ref<string | null>(null)

const novaAmbulancia = ref({ placa: '', marca: '', modelo: '' })

// Edição de status inline
const editandoStatusId = ref<string | null>(null)
const statusEmEdicao = ref<number>(0)
const salvandoStatus = ref(false)

const statusConfig: Record<number, { label: string; badgeClass: string }> = {
  0: { label: 'Disponível', badgeClass: 'bg-emerald-100 text-emerald-700' },
  1: { label: 'Em Manutenção', badgeClass: 'bg-red-100 text-red-700' },
  2: { label: 'Em Ocorrência', badgeClass: 'bg-blue-100 text-blue-700' },
}

function statusDe(status: number) {
  return statusConfig[status] ?? { label: 'Desconhecido', badgeClass: 'bg-slate-100 text-slate-600' }
}

function iniciarEdicaoStatus(a: AmbulanciaGetDTO) {
  editandoStatusId.value = a.id
  statusEmEdicao.value = a.status
}

function cancelarEdicaoStatus() {
  editandoStatusId.value = null
}

async function salvarStatus(id: string) {
  salvandoStatus.value = true
  try {
    await ambulanciasApi.atualizar(id, { status: statusEmEdicao.value })
    await carregarAmbulancias()
    editandoStatusId.value = null
    mostrarToast('Status atualizado!')
  } catch (e: unknown) {
    alert(e instanceof Error ? e.message : 'Erro ao atualizar status')
  } finally {
    salvandoStatus.value = false
  }
}

async function carregarAmbulancias() {
  carregandoAmb.value = true
  erroAmb.value = null
  try {
    ambulancias.value = await ambulanciasApi.listar()
  } catch (e: unknown) {
    erroAmb.value = e instanceof Error ? e.message : 'Erro ao carregar ambulâncias'
  } finally {
    carregandoAmb.value = false
  }
}

async function salvarAmbulancia() {
  salvandoAmb.value = true
  erroSalvarAmb.value = null
  try {
    await ambulanciasApi.criar({
      placa: novaAmbulancia.value.placa,
      marca: novaAmbulancia.value.marca,
      modelo: novaAmbulancia.value.modelo,
      tipo: '',
    })
    await carregarAmbulancias()
    novaAmbulancia.value = { placa: '', marca: '', modelo: '' }
    mostrarToast('Ambulância cadastrada com sucesso!')
  } catch (e: unknown) {
    erroSalvarAmb.value = e instanceof Error ? e.message : 'Erro ao cadastrar ambulância'
  } finally {
    salvandoAmb.value = false
  }
}

async function deletarAmbulancia(id: string) {
  if (!confirm('Tem certeza que deseja excluir esta ambulância?')) return
  try {
    await ambulanciasApi.deletar(id)
    await carregarAmbulancias()
    mostrarToast('Ambulância removida.')
  } catch (e: unknown) {
    alert(e instanceof Error ? e.message : 'Erro ao remover ambulância')
  }
}

const totalParam = computed(() => paramedicos.value.length)
const totalAmb = computed(() => ambulancias.value.length)

const tituloAba = computed(() =>
  abaAtiva.value === 'paramedicos' ? 'Configurações - Paramédicos' : 'Configurações - Ambulâncias',
)

function trocarAba(aba: Aba) {
  abaAtiva.value = aba
}

// ---- Sidebar / Sair ----
function abrirModalSair() {
  sidebarAberta.value = false
  mostrarModalSair.value = true
}

function confirmarSaida() {
  localStorage.removeItem('token')
  localStorage.removeItem('usuario_nome')
  localStorage.removeItem('usuario_cargo')
  localStorage.removeItem('usuario_id')
  mostrarModalSair.value = false
  router.push('/login')
}

onMounted(() => {
  carregarParamedicos()
  carregarAmbulancias()
})
</script>

<template>
  <div class="min-h-screen bg-slate-50">
    <!-- Overlay mobile -->
    <div
      v-if="sidebarAberta"
      class="fixed inset-0 bg-black/40 z-40 lg:hidden"
      @click="sidebarAberta = false"
    ></div>

    <!-- Sidebar -->
    <aside
      :class="[
        'fixed left-0 top-0 h-full flex flex-col py-6 bg-slate-50 dark:bg-slate-900 w-64 border-r-0 z-50 transition-transform duration-300',
        sidebarAberta ? 'translate-x-0' : '-translate-x-full lg:translate-x-0',
      ]"
    >
      <div class="px-6 mb-10">
        <span class="text-xl font-bold text-blue-900 dark:text-blue-50 tracking-tight"
          >SIGE Emergências</span
        >
      </div>
      <nav class="flex-1 space-y-1 flex flex-col">
        <RouterLink
          to="/dashboard"
          @click="sidebarAberta = false"
          class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full"
        >
          <span class="material-symbols-outlined mr-3">dashboard</span>
          <span class="font-medium">Dashboard</span>
        </RouterLink>
        <RouterLink
          to="/chamado"
          @click="sidebarAberta = false"
          class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full"
        >
          <span class="material-symbols-outlined mr-3">pending_actions</span>
          <span class="font-medium">Chamados</span>
        </RouterLink>
        <RouterLink
          to="/equipe"
          @click="sidebarAberta = false"
          class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full"
        >
          <span class="material-symbols-outlined mr-3">groups_2</span>
          <span class="font-medium">Equipe</span>
        </RouterLink>
        <RouterLink
          to="/veiculos"
          @click="sidebarAberta = false"
          class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full"
        >
          <span class="material-symbols-outlined mr-3">ambulance</span>
          <span class="font-medium">Veículos</span>
        </RouterLink>
        <RouterLink
          to="/settings"
          @click="sidebarAberta = false"
          class="bg-blue-900 dark:bg-blue-600 text-white rounded-full mx-2 flex items-center px-4 py-3 scale-95 active:scale-90 duration-150 transition-all"
        >
          <span class="material-symbols-outlined mr-3">settings</span>
          <span class="font-medium">Configurações</span>
        </RouterLink>
        <div class="flex-1"></div>
        <button
          @click="abrirModalSair"
          class="text-red-500 hover:text-red-700 px-4 py-3 hover:bg-red-50 dark:hover:bg-red-900/20 transition-all flex items-center mx-2 rounded-full mb-4 w-full text-left cursor-pointer"
        >
          <span class="material-symbols-outlined mr-3">logout</span>
          <span class="font-medium">Sair</span>
        </button>
      </nav>
    </aside>

    <!-- Header -->
    <header
      class="flex justify-between items-center w-full lg:pl-72 px-4 lg:pr-8 h-20 fixed top-0 bg-white/80 dark:bg-slate-950/80 backdrop-blur-md z-40 border-b border-slate-100 dark:border-slate-800"
    >
      <div class="flex items-center gap-3">
        <button
          class="lg:hidden p-2 rounded-lg hover:bg-slate-100 transition-colors"
          @click="sidebarAberta = !sidebarAberta"
        >
          <span class="material-symbols-outlined">menu</span>
        </button>
        <h1 class="text-2xl font-bold text-blue-900 dark:text-blue-400 tracking-tight">
          {{ tituloAba }}
        </h1>
      </div>
      <RouterLink
        to="/perfil"
        class="flex items-center gap-2 text-slate-700 hover:text-blue-700 transition-colors rounded-full font-medium"
      >
        <strong class="hidden sm:block">{{ usuarioNome }}</strong>
        <span class="material-symbols-outlined">account_circle</span>
      </RouterLink>
    </header>

    <!-- Conteúdo -->
    <main class="lg:ml-64 p-6 lg:p-8 pt-28">
      <!-- Toast -->
      <Transition name="fade">
        <div
          v-if="toast"
          class="fixed top-24 right-6 z-50 bg-emerald-600 text-white px-4 py-3 rounded-xl shadow-lg flex items-center gap-2 font-medium text-sm"
        >
          <span class="material-symbols-outlined text-lg">check_circle</span>
          {{ toast }}
        </div>
      </Transition>

      <!-- Seletor de abas -->
      <div class="flex gap-8 mb-8 border-b border-slate-200">
        <button
          @click="trocarAba('ambulancias')"
          :class="[
            'relative pb-3 text-lg font-bold transition-colors flex items-center gap-2',
            abaAtiva === 'ambulancias' ? 'text-blue-900' : 'text-slate-400 hover:text-slate-600',
          ]"
        >
          Ambulâncias
          <span
            class="text-xs font-bold px-1.5 py-0.5 rounded-full bg-slate-100 text-slate-500"
            >{{ totalAmb }}</span
          >
          <span
            v-if="abaAtiva === 'ambulancias'"
            class="absolute -bottom-px left-0 right-0 h-0.5 bg-blue-900 rounded-full"
          ></span>
        </button>
        <button
          @click="trocarAba('paramedicos')"
          :class="[
            'relative pb-3 text-lg font-bold transition-colors flex items-center gap-2',
            abaAtiva === 'paramedicos' ? 'text-blue-900' : 'text-slate-400 hover:text-slate-600',
          ]"
        >
          Paramédicos
          <span
            class="text-xs font-bold px-1.5 py-0.5 rounded-full bg-slate-100 text-slate-500"
            >{{ totalParam }}</span
          >
          <span
            v-if="abaAtiva === 'paramedicos'"
            class="absolute -bottom-px left-0 right-0 h-0.5 bg-blue-900 rounded-full"
          ></span>
        </button>
      </div>

      <!-- ============ ABA PARAMÉDICOS ============ -->
      <div v-if="abaAtiva === 'paramedicos'">
        <!-- Hero -->
        <div class="bg-blue-900 rounded-2xl p-6 mb-6 text-white flex items-center gap-4">
          <div class="w-14 h-14 rounded-xl bg-white/15 flex items-center justify-center shrink-0">
            <span class="material-symbols-outlined text-3xl">medical_services</span>
          </div>
          <div>
            <h2 class="text-xl font-bold">Gestão de Paramédicos</h2>
            <p class="text-blue-200 text-sm">Cadastre e gerencie a equipe de atendimento</p>
          </div>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-5 gap-6">
          <!-- Form adicionar -->
          <div class="lg:col-span-2 bg-white rounded-2xl shadow-sm p-6 h-fit">
            <h3 class="font-bold text-slate-800 mb-5 flex items-center gap-2">
              <span class="material-symbols-outlined text-blue-900">person_add</span>
              Adicionar Novo Paramédico
            </h3>

            <div
              v-if="erroSalvarParam"
              class="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm"
            >
              ⚠️ {{ erroSalvarParam }}
            </div>

            <form @submit.prevent="salvarParamedico" class="space-y-4">
              <div class="flex flex-col gap-1">
                <label class="text-xs font-bold text-slate-500 uppercase">Nome Completo *</label>
                <input
                  type="text"
                  v-model="novoParamedico.name"
                  required
                  placeholder="Ex: João da Silva"
                  class="bg-slate-50 rounded-lg px-3 py-2.5 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
                />
              </div>
              <div class="grid grid-cols-2 gap-4">
                <div class="flex flex-col gap-1">
                  <label class="text-xs font-bold text-slate-500 uppercase">CPF *</label>
                  <input
                    type="text"
                    v-model="novoParamedico.cpf"
                    required
                    placeholder="Somente números"
                    class="bg-slate-50 rounded-lg px-3 py-2.5 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
                  />
                </div>
                <div class="flex flex-col gap-1">
                  <label class="text-xs font-bold text-slate-500 uppercase">Telefone *</label>
                  <input
                    type="tel"
                    v-model="novoParamedico.telefone"
                    required
                    placeholder="(DDD) Número"
                    class="bg-slate-50 rounded-lg px-3 py-2.5 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
                  />
                </div>
              </div>
              <div class="flex flex-col gap-1">
                <label class="text-xs font-bold text-slate-500 uppercase">E-mail *</label>
                <input
                  type="email"
                  v-model="novoParamedico.email"
                  required
                  placeholder="email@exemplo.com"
                  class="bg-slate-50 rounded-lg px-3 py-2.5 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
                />
              </div>
              <div class="grid grid-cols-2 gap-4">
                <div class="flex flex-col gap-1">
                  <label class="text-xs font-bold text-slate-500 uppercase">Identificador *</label>
                  <input
                    type="text"
                    v-model="novoParamedico.identificadorProfissional"
                    required
                    placeholder="CRM / COREN"
                    class="bg-slate-50 rounded-lg px-3 py-2.5 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
                  />
                </div>
                <div class="flex flex-col gap-1">
                  <label class="text-xs font-bold text-slate-500 uppercase">Cargo *</label>
                  <select
                    v-model="novoParamedico.cargo"
                    required
                    class="bg-slate-50 rounded-lg px-3 py-2.5 text-sm focus:ring-2 focus:ring-blue-200 outline-none cursor-pointer"
                  >
                    <option value="" disabled>Selecione...</option>
                    <option value="medico">Médico</option>
                    <option value="enfermeiro">Enfermeiro</option>
                    <option value="condutor">Condutor</option>
                    <option value="paramedico">Paramédico</option>
                  </select>
                </div>
              </div>

              <button
                type="submit"
                :disabled="salvandoParam"
                class="w-full py-3 bg-blue-900 hover:bg-blue-800 text-white font-bold rounded-xl transition-colors flex items-center justify-center gap-2 disabled:opacity-60"
              >
                <span
                  v-if="salvandoParam"
                  class="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"
                ></span>
                {{ salvandoParam ? 'Cadastrando...' : 'Cadastrar Paramédico' }}
              </button>
            </form>
          </div>

          <!-- Lista cadastrados -->
          <div class="lg:col-span-3 bg-white rounded-2xl shadow-sm p-6">
            <div class="flex items-center justify-between mb-5">
              <h3 class="font-bold text-slate-800 flex items-center gap-2">
                <span class="material-symbols-outlined text-blue-900">groups_2</span>
                Paramédicos Cadastrados
              </h3>
              <span class="text-xs font-bold text-slate-400 bg-slate-100 px-2.5 py-1 rounded-full"
                >{{ totalParam }} total</span
              >
            </div>

            <!-- Erro -->
            <div
              v-if="erroParam"
              class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm mb-4"
            >
              ⚠️ {{ erroParam }}
              <button @click="carregarParamedicos" class="ml-3 underline font-semibold">
                Tentar novamente
              </button>
            </div>

            <!-- Skeleton -->
            <div v-if="carregandoParam" class="space-y-3">
              <div v-for="i in 4" :key="i" class="bg-slate-100 rounded-xl h-16 animate-pulse"></div>
            </div>

            <!-- Vazio -->
            <div
              v-else-if="paramedicos.length === 0 && !erroParam"
              class="flex flex-col items-center justify-center py-16 text-slate-400"
            >
              <span class="material-symbols-outlined text-5xl mb-3">person_off</span>
              <p class="font-medium">Nenhum paramédico cadastrado</p>
            </div>

            <!-- Lista -->
            <div v-else class="space-y-3">
              <div
                v-for="p in paramedicos"
                :key="p.id"
                class="flex items-center gap-4 p-3 rounded-xl hover:bg-slate-50 transition-colors border border-slate-100"
              >
                <div
                  class="w-11 h-11 rounded-full bg-blue-900 text-white flex items-center justify-center font-bold text-sm shrink-0"
                >
                  {{ iniciaisDe(p.nome) }}
                </div>
                <div class="flex-1 min-w-0">
                  <p class="font-semibold text-slate-800 truncate">{{ p.nome }}</p>
                  <p class="text-xs text-slate-400">ID-{{ idCurto(p.id) }}</p>
                </div>
                <span
                  :class="['px-2.5 py-0.5 rounded-full text-xs font-bold', cargoDe(p.cargo).badgeClass]"
                >
                  {{ cargoDe(p.cargo).label }}
                </span>
                <button
                  @click="deletarParamedico(p.id)"
                  class="p-2 text-slate-300 hover:text-red-500 transition-colors rounded-lg hover:bg-red-50"
                  title="Remover paramédico"
                >
                  <span class="material-symbols-outlined text-xl">delete</span>
                </button>
              </div>
            </div>

            <!-- Footer Exibir Todos -->
            <div
              v-if="!carregandoParam && paramedicos.length > 0"
              class="mt-5 pt-4 border-t border-slate-100 text-center"
            >
              <a href="#" class="text-sm font-bold text-blue-900 hover:underline">Exibir Todos</a>
            </div>
          </div>
        </div>
      </div>

      <!-- ============ ABA AMBULÂNCIAS ============ -->
      <div v-else>
        <!-- Hero -->
        <div class="bg-blue-900 rounded-2xl p-6 mb-6 text-white flex items-center gap-4">
          <div class="w-14 h-14 rounded-xl bg-white/15 flex items-center justify-center shrink-0">
            <span class="material-symbols-outlined text-3xl">ambulance</span>
          </div>
          <div>
            <h2 class="text-xl font-bold">Frota de Emergência</h2>
            <p class="text-blue-200 text-sm">
              Gerencie as unidades móveis de atendimento e assegure a prontidão operacional de cada
              veículo.
            </p>
          </div>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-5 gap-6">
          <!-- Form adicionar -->
          <div class="lg:col-span-2 bg-white rounded-2xl shadow-sm p-6 h-fit">
            <h3 class="font-bold text-slate-800 flex items-center gap-2">
              <span class="material-symbols-outlined text-blue-900">add_box</span>
              Adicionar Nova Ambulância
            </h3>
            <p class="text-xs text-slate-400 mb-5 mt-1">Preencha os dados técnicos do veículo</p>

            <div
              v-if="erroSalvarAmb"
              class="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm"
            >
              ⚠️ {{ erroSalvarAmb }}
            </div>

            <form @submit.prevent="salvarAmbulancia" class="space-y-4">
              <div class="flex flex-col gap-1">
                <label class="text-xs font-bold text-slate-500 uppercase">Placa do Veículo *</label>
                <input
                  type="text"
                  v-model="novaAmbulancia.placa"
                  required
                  placeholder="ABC-1234"
                  class="bg-slate-50 rounded-lg px-3 py-2.5 text-sm focus:ring-2 focus:ring-blue-200 outline-none uppercase"
                />
              </div>
              <div class="flex flex-col gap-1">
                <label class="text-xs font-bold text-slate-500 uppercase">Marca *</label>
                <input
                  type="text"
                  v-model="novaAmbulancia.marca"
                  required
                  placeholder="Ex: Mercedes-Benz"
                  class="bg-slate-50 rounded-lg px-3 py-2.5 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
                />
              </div>
              <div class="flex flex-col gap-1">
                <label class="text-xs font-bold text-slate-500 uppercase">Modelo do Veículo *</label>
                <input
                  type="text"
                  v-model="novaAmbulancia.modelo"
                  required
                  placeholder="Ex: Sprinter 415 CDI"
                  class="bg-slate-50 rounded-lg px-3 py-2.5 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
                />
              </div>

              <button
                type="submit"
                :disabled="salvandoAmb"
                class="w-full py-3 bg-blue-900 hover:bg-blue-800 text-white font-bold rounded-xl transition-colors flex items-center justify-center gap-2 disabled:opacity-60"
              >
                <span
                  v-if="salvandoAmb"
                  class="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"
                ></span>
                {{ salvandoAmb ? 'Cadastrando...' : 'Cadastrar Ambulância' }}
              </button>
            </form>
          </div>

          <!-- Tabela cadastradas -->
          <div class="lg:col-span-3 bg-white rounded-2xl shadow-sm p-6">
            <div class="flex items-center justify-between mb-5">
              <h3 class="font-bold text-slate-800 flex items-center gap-2">
                <span class="material-symbols-outlined text-blue-900">local_shipping</span>
                Ambulâncias Cadastradas
              </h3>
              <span class="text-xs font-bold text-slate-400 bg-slate-100 px-2.5 py-1 rounded-full"
                >{{ totalAmb }} total</span
              >
            </div>

            <!-- Erro -->
            <div
              v-if="erroAmb"
              class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm mb-4"
            >
              ⚠️ {{ erroAmb }}
              <button @click="carregarAmbulancias" class="ml-3 underline font-semibold">
                Tentar novamente
              </button>
            </div>

            <!-- Skeleton -->
            <div v-if="carregandoAmb" class="space-y-3">
              <div v-for="i in 4" :key="i" class="bg-slate-100 rounded-xl h-14 animate-pulse"></div>
            </div>

            <!-- Vazio -->
            <div
              v-else-if="ambulancias.length === 0 && !erroAmb"
              class="flex flex-col items-center justify-center py-16 text-slate-400"
            >
              <span class="material-symbols-outlined text-5xl mb-3">no_crash</span>
              <p class="font-medium uppercase tracking-wide text-sm">Aguardando novos registros</p>
            </div>

            <!-- Tabela -->
            <div v-else class="overflow-x-auto">
              <table class="w-full text-left">
                <thead>
                  <tr class="text-xs font-bold text-slate-400 uppercase tracking-wider border-b border-slate-100">
                    <th class="pb-3 pr-4">Identificação</th>
                    <th class="pb-3 pr-4">Modelo</th>
                    <th class="pb-3 pr-4">Status</th>
                    <th class="pb-3 text-right">Ações</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-50">
                  <tr v-for="a in ambulancias" :key="a.id" class="hover:bg-slate-50 transition-colors">
                    <td class="py-3 pr-4">
                      <div class="flex items-center gap-3">
                        <div class="w-9 h-9 rounded-lg bg-blue-50 flex items-center justify-center shrink-0">
                          <span class="material-symbols-outlined text-blue-600 text-xl">ambulance</span>
                        </div>
                        <p class="font-semibold text-slate-800 text-sm">{{ a.placa }}</p>
                      </div>
                    </td>
                    <td class="py-3 pr-4 text-sm text-slate-600">{{ a.marca }} {{ a.modelo }}</td>
                    <td class="py-3 pr-4">
                      <!-- Editor de status inline -->
                      <div v-if="editandoStatusId === a.id" class="flex items-center gap-2">
                        <select
                          v-model.number="statusEmEdicao"
                          class="bg-slate-50 border border-slate-200 rounded-lg px-2 py-1 text-xs focus:ring-2 focus:ring-blue-200 outline-none cursor-pointer"
                        >
                          <option :value="0">Disponível</option>
                          <option :value="1">Em Manutenção</option>
                          <option :value="2">Em Ocorrência</option>
                        </select>
                        <button
                          @click="salvarStatus(a.id)"
                          :disabled="salvandoStatus"
                          class="p-1 text-emerald-600 hover:bg-emerald-50 rounded transition-colors disabled:opacity-50"
                          title="Salvar"
                        >
                          <span class="material-symbols-outlined text-lg">check</span>
                        </button>
                        <button
                          @click="cancelarEdicaoStatus"
                          class="p-1 text-slate-400 hover:bg-slate-100 rounded transition-colors"
                          title="Cancelar"
                        >
                          <span class="material-symbols-outlined text-lg">close</span>
                        </button>
                      </div>
                      <!-- Badge de status -->
                      <span
                        v-else
                        :class="['px-2.5 py-0.5 rounded-full text-xs font-bold', statusDe(a.status).badgeClass]"
                      >
                        {{ statusDe(a.status).label }}
                      </span>
                    </td>
                    <td class="py-3 text-right whitespace-nowrap">
                      <button
                        @click="iniciarEdicaoStatus(a)"
                        class="p-2 text-slate-300 hover:text-blue-600 transition-colors rounded-lg hover:bg-blue-50"
                        title="Editar status"
                      >
                        <span class="material-symbols-outlined text-xl">edit</span>
                      </button>
                      <button
                        @click="deletarAmbulancia(a.id)"
                        class="p-2 text-slate-300 hover:text-red-500 transition-colors rounded-lg hover:bg-red-50"
                        title="Excluir ambulância"
                      >
                        <span class="material-symbols-outlined text-xl">delete</span>
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>

  <!-- Modal de saída -->
  <Teleport to="body">
    <div
      v-if="mostrarModalSair"
      class="fixed inset-0 bg-black/50 z-[100] flex items-center justify-center p-4"
    >
      <div class="bg-white rounded-2xl shadow-2xl p-8 max-w-sm w-full">
        <div class="flex flex-col items-center text-center">
          <span class="material-symbols-outlined text-red-500 text-6xl mb-4">logout</span>
          <h2 class="text-xl font-bold text-slate-800 mb-2">Sair do sistema?</h2>
          <p class="text-slate-500 mb-6">
            Tem certeza que deseja sair? Você precisará fazer login novamente.
          </p>
          <div class="flex gap-3 w-full">
            <button
              @click="mostrarModalSair = false"
              class="flex-1 py-3 border-2 border-slate-200 text-slate-600 font-bold rounded-xl hover:bg-slate-50 transition-colors"
            >
              Cancelar
            </button>
            <button
              @click="confirmarSaida"
              class="flex-1 py-3 bg-red-500 text-white font-bold rounded-xl hover:bg-red-600 transition-colors"
            >
              Sim, sair
            </button>
          </div>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease, transform 0.3s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}
</style>
