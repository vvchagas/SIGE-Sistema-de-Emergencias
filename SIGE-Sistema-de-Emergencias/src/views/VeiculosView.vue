<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { ambulanciasApi, type AmbulanciaGetDTO } from '../api/index'

const sidebarAberta = ref(false)
const mostrarModalSair = ref(false)
const router = useRouter()
const usuarioNome = ref(localStorage.getItem('usuario_nome') || 'Usuário')

const ambulancias = ref<AmbulanciaGetDTO[]>([])
const carregando = ref(true)
const erro = ref<string | null>(null)
const mostrandoFormulario = ref(false)
const filtroPainel = ref<'todos' | 'disponiveis' | 'ocorrencia' | 'manutencao'>('todos')

const novaAmbulancia = ref({ placa: '', marca: '', modelo: '', tipo: '' })
const salvando = ref(false)
const erroSalvar = ref<string | null>(null)

type StatusKey = 0 | 1 | 2

const statusConfig: Record<StatusKey, { label: string; badgeClass: string; borderClass: string; iconClass: string; bgIcon: string }> = {
  0: {
    label: 'Disponível',
    badgeClass: 'bg-emerald-100 text-emerald-700',
    borderClass: 'border-l-emerald-500',
    iconClass: 'text-emerald-600',
    bgIcon: 'bg-emerald-50',
  },
  1: {
    label: 'Manutenção',
    badgeClass: 'bg-red-100 text-red-700',
    borderClass: 'border-l-red-500',
    iconClass: 'text-red-600',
    bgIcon: 'bg-red-50',
  },
  2: {
    label: 'Em Ocorrência',
    badgeClass: 'bg-blue-100 text-blue-700',
    borderClass: 'border-l-blue-500',
    iconClass: 'text-blue-600',
    bgIcon: 'bg-blue-50',
  },
}

function statusDe(a: AmbulanciaGetDTO) {
  return statusConfig[a.status as StatusKey] ?? statusConfig[0]
}

function formatarTipo(tipo: string) {
  const mapa: Record<string, string> = {
    suporte_basico: 'Suporte Básico',
    suporte_avancado: 'Suporte Avançado',
    uti_movel: 'UTI Móvel',
  }
  return mapa[tipo] ?? tipo
}

const total = computed(() => ambulancias.value.length)
const operacionais = computed(() => ambulancias.value.filter(a => a.status === 0).length)
const emManutencao = computed(() => ambulancias.value.filter(a => a.status === 1).length)
const emOcorrencia = computed(() => ambulancias.value.filter(a => a.status === 2).length)

const porcentagemDisponivel = computed(() =>
  total.value > 0 ? Math.round((operacionais.value / total.value) * 100) : 0,
)

const ambulanciasFiltradas = computed(() => {
  if (filtroPainel.value === 'disponiveis') return ambulancias.value.filter(a => a.status === 0)
  if (filtroPainel.value === 'ocorrencia') return ambulancias.value.filter(a => a.status === 2)
  if (filtroPainel.value === 'manutencao') return ambulancias.value.filter(a => a.status === 1)
  return ambulancias.value
})

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

async function carregarVeiculos() {
  carregando.value = true
  erro.value = null
  try {
    ambulancias.value = await ambulanciasApi.listar()
  } catch (e: unknown) {
    erro.value = e instanceof Error ? e.message : 'Erro ao carregar veículos'
  } finally {
    carregando.value = false
  }
}

async function salvarAmbulancia() {
  salvando.value = true
  erroSalvar.value = null
  try {
    await ambulanciasApi.criar({
      placa: novaAmbulancia.value.placa,
      marca: novaAmbulancia.value.marca,
      modelo: novaAmbulancia.value.modelo,
      tipo: novaAmbulancia.value.tipo,
    })
    await carregarVeiculos()
    mostrandoFormulario.value = false
    novaAmbulancia.value = { placa: '', marca: '', modelo: '', tipo: '' }
  } catch (e: unknown) {
    erroSalvar.value = e instanceof Error ? e.message : 'Erro ao salvar ambulância'
  } finally {
    salvando.value = false
  }
}

async function deletarAmbulancia(id: string) {
  if (!confirm('Tem certeza que deseja excluir esta ambulância?')) return
  try {
    await ambulanciasApi.deletar(id)
    await carregarVeiculos()
  } catch (e: unknown) {
    alert(e instanceof Error ? e.message : 'Erro ao deletar ambulância')
  }
}

onMounted(carregarVeiculos)
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
          class="bg-blue-900 dark:bg-blue-600 text-white rounded-full mx-2 flex items-center px-4 py-3 scale-95 active:scale-90 duration-150 transition-all"
        >
          <span class="material-symbols-outlined mr-3">ambulance</span>
          <span class="font-medium">Veículos</span>
        </RouterLink>
        <RouterLink
          to="/settings"
          @click="sidebarAberta = false"
          class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full"
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
          Gestão de Frotas
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

    <!-- Conteúdo principal -->
    <main class="lg:ml-64 p-6 lg:p-8 pt-28">

      <!-- Abas de filtro -->
      <div class="flex flex-wrap gap-2 mb-8 bg-white rounded-2xl p-2 shadow-sm w-fit">
        <button
          v-for="aba in [
            { key: 'todos', label: 'Todos', count: total },
            { key: 'disponiveis', label: 'Disponíveis', count: operacionais },
            { key: 'ocorrencia', label: 'Em Ocorrência', count: emOcorrencia },
            { key: 'manutencao', label: 'Manutenção', count: emManutencao },
          ]"
          :key="aba.key"
          @click="filtroPainel = aba.key as typeof filtroPainel"
          :class="[
            'px-4 py-2 rounded-xl font-semibold text-sm transition-all flex items-center gap-2',
            filtroPainel === aba.key
              ? 'bg-blue-900 text-white shadow'
              : 'text-slate-500 hover:bg-slate-100',
          ]"
        >
          {{ aba.label }}
          <span
            :class="[
              'text-xs font-bold px-1.5 py-0.5 rounded-full',
              filtroPainel === aba.key ? 'bg-white/20 text-white' : 'bg-slate-100 text-slate-600',
            ]"
          >{{ aba.count }}</span>
        </button>
      </div>

      <!-- Cards de estatísticas -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <!-- Frota Total -->
        <div class="bg-white p-6 rounded-2xl shadow-sm flex items-start justify-between">
          <div>
            <p class="text-xs font-bold text-slate-400 uppercase tracking-wider mb-1">Frota Total</p>
            <p class="text-4xl font-extrabold text-slate-800">
              <span v-if="carregando" class="w-8 h-8 border-2 border-slate-300 border-t-slate-700 rounded-full animate-spin inline-block"></span>
              <span v-else>{{ String(total).padStart(2, '0') }}</span>
            </p>
          </div>
          <div class="flex flex-col items-end gap-2">
            <span class="text-xs font-bold text-blue-600 bg-blue-50 px-2 py-0.5 rounded-full">
              {{ operacionais > 0 ? `+${operacionais} hoje` : 'Atualizado' }}
            </span>
            <div class="w-12 h-12 rounded-xl bg-blue-50 flex items-center justify-center">
              <span class="material-symbols-outlined text-blue-600">airport_shuttle</span>
            </div>
          </div>
        </div>

        <!-- Operacionais -->
        <div class="bg-white p-6 rounded-2xl shadow-sm flex items-start justify-between">
          <div>
            <p class="text-xs font-bold text-slate-400 uppercase tracking-wider mb-1">Operacionais</p>
            <p class="text-4xl font-extrabold text-slate-800">
              <span v-if="carregando" class="w-8 h-8 border-2 border-slate-300 border-t-slate-700 rounded-full animate-spin inline-block"></span>
              <span v-else>{{ String(operacionais).padStart(2, '0') }}</span>
            </p>
          </div>
          <div class="flex flex-col items-end gap-2">
            <span class="text-xs font-bold text-emerald-600 bg-emerald-50 px-2 py-0.5 rounded-full">
              {{ porcentagemDisponivel }}% Disponível
            </span>
            <div class="w-12 h-12 rounded-xl bg-emerald-50 flex items-center justify-center">
              <span class="material-symbols-outlined text-emerald-600">check_circle</span>
            </div>
          </div>
        </div>

        <!-- Em Manutenção -->
        <div class="bg-white p-6 rounded-2xl shadow-sm flex items-start justify-between">
          <div>
            <p class="text-xs font-bold text-slate-400 uppercase tracking-wider mb-1">Em Manutenção</p>
            <p class="text-4xl font-extrabold text-slate-800">
              <span v-if="carregando" class="w-8 h-8 border-2 border-slate-300 border-t-slate-700 rounded-full animate-spin inline-block"></span>
              <span v-else>{{ String(emManutencao).padStart(2, '0') }}</span>
            </p>
          </div>
          <div class="flex flex-col items-end gap-2">
            <span
              :class="[
                'text-xs font-bold px-2 py-0.5 rounded-full',
                emManutencao > 0 ? 'text-amber-600 bg-amber-50' : 'text-slate-400 bg-slate-50',
              ]"
            >
              {{ emManutencao > 0 ? `Crítico: ${emManutencao}` : 'Normal' }}
            </span>
            <div class="w-12 h-12 rounded-xl bg-amber-50 flex items-center justify-center">
              <span class="material-symbols-outlined text-amber-500">warning</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Cabeçalho da lista + botão adicionar -->
      <div class="flex items-center justify-between mb-4">
        <h2 class="text-lg font-bold text-slate-800">Lista de Ambulâncias</h2>
        <button
          @click="mostrandoFormulario = !mostrandoFormulario"
          class="px-4 py-2 bg-blue-900 text-white rounded-full text-sm font-bold hover:bg-blue-800 transition-colors flex items-center gap-2"
        >
          <span class="material-symbols-outlined text-sm">{{ mostrandoFormulario ? 'close' : 'add' }}</span>
          {{ mostrandoFormulario ? 'Fechar' : 'Nova Ambulância' }}
        </button>
      </div>

      <!-- Formulário de cadastro -->
      <div v-if="mostrandoFormulario" class="bg-white rounded-2xl p-6 shadow-sm mb-6">
        <h3 class="font-bold text-indigo-950 mb-4">Cadastrar Nova Ambulância</h3>
        <div v-if="erroSalvar" class="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm">
          ⚠️ {{ erroSalvar }}
        </div>
        <form @submit.prevent="salvarAmbulancia" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <div class="flex flex-col gap-1">
            <label class="text-xs font-bold text-slate-500 uppercase">Placa *</label>
            <input
              type="text"
              v-model="novaAmbulancia.placa"
              required
              placeholder="ABC-1234"
              class="bg-slate-50 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-200 outline-none uppercase"
            />
          </div>
          <div class="flex flex-col gap-1">
            <label class="text-xs font-bold text-slate-500 uppercase">Marca *</label>
            <input
              type="text"
              v-model="novaAmbulancia.marca"
              required
              placeholder="Ex: Mercedes-Benz"
              class="bg-slate-50 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
            />
          </div>
          <div class="flex flex-col gap-1">
            <label class="text-xs font-bold text-slate-500 uppercase">Modelo *</label>
            <input
              type="text"
              v-model="novaAmbulancia.modelo"
              required
              placeholder="Ex: Sprinter 415 CDI"
              class="bg-slate-50 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
            />
          </div>
          <div class="flex flex-col gap-1">
            <label class="text-xs font-bold text-slate-500 uppercase">Tipo *</label>
            <select
              v-model="novaAmbulancia.tipo"
              required
              class="bg-slate-50 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-200 outline-none cursor-pointer"
            >
              <option value="" disabled>Selecione...</option>
              <option value="suporte_basico">Suporte Básico</option>
              <option value="suporte_avancado">Suporte Avançado</option>
              <option value="uti_movel">UTI Móvel</option>
            </select>
          </div>
          <div class="md:col-span-2 lg:col-span-4 flex gap-3 mt-2">
            <button
              type="submit"
              :disabled="salvando"
              class="px-6 py-2 bg-blue-900 text-white rounded-lg text-sm font-bold hover:bg-blue-800 transition-colors disabled:opacity-60 flex items-center gap-2"
            >
              <span v-if="salvando" class="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></span>
              {{ salvando ? 'Salvando...' : 'Salvar Ambulância' }}
            </button>
          </div>
        </form>
      </div>

      <!-- Erro ao carregar -->
      <div
        v-if="erro"
        class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm mb-6"
      >
        ⚠️ {{ erro }}
        <button @click="carregarVeiculos" class="ml-3 underline font-semibold">Tentar novamente</button>
      </div>

      <!-- Skeleton de carregamento -->
      <div v-if="carregando" class="space-y-3">
        <div v-for="i in 4" :key="i" class="bg-white rounded-2xl h-24 animate-pulse"></div>
      </div>

      <!-- Lista vazia -->
      <div
        v-else-if="!carregando && ambulanciasFiltradas.length === 0 && !erro"
        class="flex flex-col items-center justify-center py-20 text-slate-400"
      >
        <span class="material-symbols-outlined text-6xl mb-3">airport_shuttle</span>
        <p class="font-medium text-lg">
          {{
            filtroPainel === 'todos'
              ? 'Nenhuma ambulância cadastrada'
              : 'Nenhuma ambulância nesta categoria'
          }}
        </p>
      </div>

      <!-- Lista de ambulâncias -->
      <div v-else class="space-y-3">
        <div
          v-for="amb in ambulanciasFiltradas"
          :key="amb.id"
          :class="[
            'bg-white rounded-2xl shadow-sm flex items-center gap-4 p-5 border-l-4 hover:shadow-md transition-shadow',
            statusDe(amb).borderClass,
          ]"
        >
          <!-- Ícone -->
          <div
            :class="[
              'w-14 h-14 rounded-xl flex items-center justify-center shrink-0',
              statusDe(amb).bgIcon,
            ]"
          >
            <span :class="['material-symbols-outlined text-2xl', statusDe(amb).iconClass]">airport_shuttle</span>
          </div>

          <!-- Info principal -->
          <div class="flex-1 min-w-0">
            <div class="flex items-center gap-3 flex-wrap mb-1">
              <p class="font-extrabold text-slate-800 text-lg tracking-wide">{{ amb.placa }}</p>
              <span :class="['px-2.5 py-0.5 rounded-full text-xs font-bold', statusDe(amb).badgeClass]">
                {{ statusDe(amb).label }}
              </span>
            </div>
            <p class="text-sm text-slate-500">{{ amb.marca }} {{ amb.modelo }}</p>
            <p class="text-xs text-slate-400 mt-0.5">{{ formatarTipo(amb.tipo) }}</p>
          </div>

          <!-- Status info condicional -->
          <div class="hidden md:flex flex-col items-end gap-1 text-right">
            <template v-if="amb.status === 2">
              <p class="text-xs font-bold text-slate-400 uppercase">Em Chamado</p>
              <p class="text-sm font-semibold text-blue-700">Em Ocorrência</p>
            </template>
            <template v-else-if="amb.status === 1">
              <p class="text-xs font-bold text-slate-400 uppercase">Status</p>
              <p class="text-sm font-semibold text-red-600">Indisponível</p>
            </template>
            <template v-else>
              <p class="text-xs font-bold text-slate-400 uppercase">Status</p>
              <p class="text-sm font-semibold text-emerald-600">Pronta para uso</p>
            </template>
          </div>

          <!-- Botão excluir -->
          <button
            @click="deletarAmbulancia(amb.id)"
            class="p-2 text-slate-300 hover:text-red-500 transition-colors rounded-lg hover:bg-red-50"
            title="Excluir ambulância"
          >
            <span class="material-symbols-outlined text-xl">delete</span>
          </button>
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
