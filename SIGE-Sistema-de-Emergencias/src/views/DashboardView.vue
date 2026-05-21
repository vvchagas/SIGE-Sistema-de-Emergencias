<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ChamadoGetDto, ParamedicoGetDTO, chamadosApi, paramedicosApi } from '@/api/index'

const router = useRouter()
const sidebarAberta = ref(false)
const mostrarModalSair = ref(false)

const chamados = ref<ChamadoGetDto[]>([])
const paramedicos = ref<ParamedicoGetDTO[]>([])
const carregandoChamados = ref(true)
const carregandoParamedicos = ref(true)
const erroChamados = ref<string | null>(null)
const erroParamedicos = ref<string | null>(null)

const usuarioNome = ref(localStorage.getItem('usuario_nome') || 'Usuário')

const totalAbertos = computed(() => chamados.value.filter((c: ChamadoGetDto) => c.status === 0).length)
const totalEmAtendimento = computed(() => chamados.value.filter((c: ChamadoGetDto) => c.status === 1).length)
const totalFinalizados = computed(() => chamados.value.filter((c: ChamadoGetDto) => c.status === 2).length)

const paramedicosDisponiveis = computed(() => paramedicos.value.filter((p: ParamedicoGetDTO) => !p.ocupado))

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

const statusLabel: Record<number, string> = {
  0: 'ABERTO',
  1: 'Em Atendimento',
  2: 'Finalizado',
}

const statusClasse: Record<number, string> = {
  0: 'text-red-600',
  1: 'text-amber-600',
  2: 'text-emerald-600',
}

const statusBg: Record<number, string> = {
  0: 'bg-red-100 border-l-red-500',
  1: 'bg-amber-100 border-l-amber-500',
  2: 'bg-emerald-100 border-l-emerald-500',
}

const prioridadeLabel: Record<number, string> = {
  0: 'Emergência',
  1: 'Urgência',
  2: 'Pouco Urgente',
}

const prioridadeCor: Record<number, string> = {
  0: 'text-red-600 bg-red-100',
  1: 'text-amber-600 bg-amber-100',
  2: 'text-emerald-600 bg-emerald-100',
}

function tempoRelativo(dataStr?: string): string {
  if (!dataStr) return ''
  const diff = Math.floor((Date.now() - new Date(dataStr).getTime()) / 60000)
  if (diff < 1) return 'agora'
  if (diff === 1) return '1 min atrás'
  return `${diff} min atrás`
}

async function carregarDados() {
  carregandoChamados.value = true
  erroChamados.value = null
  try {
    chamados.value = await chamadosApi.listar(1, 20)
  } catch (e: unknown) {
    erroChamados.value = e instanceof Error ? e.message : 'Erro ao carregar chamados'
  } finally {
    carregandoChamados.value = false
  }

  carregandoParamedicos.value = true
  erroParamedicos.value = null
  try {
    paramedicos.value = await paramedicosApi.listar(1, 50)
  } catch (e: unknown) {
    erroParamedicos.value = e instanceof Error ? e.message : 'Erro ao carregar paramédicos'
  } finally {
    carregandoParamedicos.value = false
  }
}

onMounted(carregarDados)
</script>

<template>
  <div>
    <div
      v-if="sidebarAberta"
      class="fixed inset-0 bg-black/40 z-40 lg:hidden"
      @click="sidebarAberta = false"
    ></div>

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
          class="bg-blue-900 dark:bg-blue-600 text-white rounded-full mx-2 flex items-center px-4 py-3 scale-95 active:scale-90 duration-150 transition-all"
        >
          <span class="material-symbols-outlined mr-3" data-icon="dashboard">dashboard</span>
          <span class="font-medium">Dashboard</span>
        </RouterLink>
        <RouterLink
          to="/chamado"
          @click="sidebarAberta = false"
          class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full"
        >
          <span class="material-symbols-outlined mr-3" data-icon="pending_actions"
            >pending_actions</span
          >
          <span class="font-medium">Chamados</span>
        </RouterLink>
        <RouterLink
          to="/equipe"
          @click="sidebarAberta = false"
          class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full"
        >
          <span class="material-symbols-outlined mr-3" data-icon="groups_2">groups_2</span>
          <span class="font-medium">Equipe</span>
        </RouterLink>
        <RouterLink
          to="/veiculos"
          @click="sidebarAberta = false"
          class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full"
        >
          <span class="material-symbols-outlined mr-3" data-icon="ambulance">ambulance</span>
          <span class="font-medium">Veículos</span>
        </RouterLink>
        <RouterLink
          to="/settings"
          @click="sidebarAberta = false"
          class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full"
        >
          <span class="material-symbols-outlined mr-3" data-icon="settings">settings</span>
          <span class="font-medium">Configurações</span>
        </RouterLink>
        <div class="flex-1"></div>
        <button
          @click="abrirModalSair"
          class="text-red-500 hover:text-red-700 px-4 py-3 hover:bg-red-50 dark:hover:bg-red-900/20 transition-all flex items-center mx-2 rounded-full mb-4 w-full text-left cursor-pointer"
        >
          <span class="material-symbols-outlined mr-3" data-icon="logout">logout</span>
          <span class="font-medium">Sair</span>
        </button>
      </nav>
    </aside>
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
        <h1
          class="text-2xl font-bold font-headline text-blue-900 dark:text-blue-400 tracking-tight"
        >
          Dashboard
        </h1>
      </div>
      <div class="flex items-center space-x-6">
        <div
          class="flex items-center gap-3 cursor-pointer hover:bg-slate-50 p-2 -mr-2 rounded-lg transition-colors"
        >
          <RouterLink
            to="/perfil"
            class="flex gap-3 text-black hover:text-blue-600 rounded-full transition-colors"
          >
            <p class="hidden sm:block">
              <strong>{{ usuarioNome }}</strong>
            </p>
            <span class="material-symbols-outlined" data-icon="account_circle">account_circle</span>
          </RouterLink>
        </div>
      </div>
    </header>
    <main class="lg:pl-72 px-4 lg:pr-8 pt-28 pb-12 min-h-screen">
      <section class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-10">
        <div
          class="bg-white p-6 rounded-xl shadow-sm flex items-center space-x-4 border-l-4 border-l-red-500"
        >
          <div class="w-12 h-12 rounded-full bg-red-100 flex items-center justify-center">
            <span class="material-symbols-outlined text-red-600">pending</span>
          </div>
          <div>
            <p class="text-sm font-medium text-slate-500 uppercase tracking-wider">Abertos</p>
            <p class="text-3xl font-extrabold text-slate-800">
              <span
                v-if="carregandoChamados"
                class="w-6 h-6 border-2 border-slate-800 border-t-transparent rounded-full animate-spin inline-block"
              ></span>
              <span v-else>{{ totalAbertos }}</span>
            </p>
          </div>
        </div>
        <div
          class="bg-white p-6 rounded-xl shadow-sm flex items-center space-x-4 border-l-4 border-l-amber-500"
        >
          <div class="w-12 h-12 rounded-full bg-amber-100 flex items-center justify-center">
            <span class="material-symbols-outlined text-amber-600">emergency</span>
          </div>
          <div>
            <p class="text-sm font-medium text-slate-500 uppercase tracking-wider">Em Atendimento</p>
            <p class="text-3xl font-extrabold text-slate-800">
              <span
                v-if="carregandoChamados"
                class="w-6 h-6 border-2 border-slate-800 border-t-transparent rounded-full animate-spin inline-block"
              ></span>
              <span v-else>{{ totalEmAtendimento }}</span>
            </p>
          </div>
        </div>
        <div
          class="bg-white p-6 rounded-xl shadow-sm flex items-center space-x-4 border-l-4 border-l-emerald-500"
        >
          <div class="w-12 h-12 rounded-full bg-emerald-100 flex items-center justify-center">
            <span class="material-symbols-outlined text-emerald-600">check_circle</span>
          </div>
          <div>
            <p class="text-sm font-medium text-slate-500 uppercase tracking-wider">Finalizados</p>
            <p class="text-3xl font-extrabold text-slate-800">
              <span
                v-if="carregandoChamados"
                class="w-6 h-6 border-2 border-slate-800 border-t-transparent rounded-full animate-spin inline-block"
              ></span>
              <span v-else>{{ totalFinalizados }}</span>
            </p>
          </div>
        </div>
      </section>

      <div class="grid grid-cols-1 xl:grid-cols-12 gap-8">
        <section class="xl:col-span-8 space-y-4">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-xl font-bold text-slate-800">Chamados em Tempo Real</h2>
            <button
              @click="carregarDados"
              :disabled="carregandoChamados"
              class="px-3 py-1 bg-blue-900 text-white rounded-full text-xs font-semibold flex items-center gap-2 hover:bg-blue-800 transition-colors disabled:opacity-50"
            >
              <span
                class="material-symbols-outlined text-sm"
                :class="{ 'animate-spin': carregandoChamados }"
                >refresh</span
              >
              Atualizar
            </button>
          </div>

          <div
            v-if="erroChamados"
            class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm"
          >
            ⚠️ {{ erroChamados }}
          </div>

          <div v-if="carregandoChamados && !erroChamados" class="space-y-3">
            <div
              v-for="i in 3"
              :key="i"
              class="bg-white p-5 rounded-xl h-20 animate-pulse"
            ></div>
          </div>

          <div
            v-else-if="!carregandoChamados && chamados.length === 0 && !erroChamados"
            class="flex flex-col items-center justify-center py-16 text-slate-400"
          >
            <span class="material-symbols-outlined text-6xl mb-3">inbox</span>
            <p class="font-medium">Nenhum chamado no momento</p>
            <RouterLink
              to="/chamado"
              class="mt-4 px-4 py-2 bg-blue-900 text-white rounded-full text-sm font-bold hover:bg-blue-800 transition-opacity"
            >
              + Novo Chamado
            </RouterLink>
          </div>

          <div
            v-for="chamado in chamados"
            :key="chamado.id"
            :class="['bg-white p-5 rounded-xl flex items-center border-l-8 shadow-sm hover:translate-x-1 transition-transform cursor-pointer', statusBg[chamado.status] as string]"
          >
            <div class="flex-1">
              <div class="flex items-center space-x-3 mb-1">
                <h3 class="font-bold text-lg text-slate-800">{{ chamado.nomeSolicitante }}</h3>
                <span
                  :class="['px-2 py-0.5 rounded-full text-xs font-bold', prioridadeCor[chamado.nivelPrioridade] as string]"
                >
                  {{ prioridadeLabel[chamado.nivelPrioridade] }}
                </span>
              </div>
              <p class="text-sm text-slate-500 mb-1">{{ chamado.tipoEmergencia }} - {{ chamado.descricao }}</p>
              <div class="flex items-center space-x-4 text-xs text-slate-400">
                <span class="flex items-center">
                  <span class="material-symbols-outlined text-base mr-1">schedule</span>
                  {{ tempoRelativo(chamado.criadoEm) }}
                </span>
                <span class="flex items-center">
                  <span class="material-symbols-outlined text-base mr-1">location_on</span>
                  {{ chamado.bairro }}, {{ chamado.cidade }}
                </span>
              </div>
            </div>
            <div class="text-right">
              <span :class="['block text-xs font-bold mb-1', statusClasse[chamado.status]]">
                {{ statusLabel[chamado.status] }}
              </span>
              <RouterLink
                v-if="chamado.status === 0"
                to="/chamado"
                class="bg-blue-900 text-white text-xs font-bold px-4 py-2 rounded-full hover:bg-blue-800 transition-colors inline-block"
              >
                ATENDER
              </RouterLink>
            </div>
          </div>
        </section>

        <section class="xl:col-span-4 space-y-4">
          <div class="mb-4">
            <h2 class="text-xl font-bold text-slate-800">Paramédicos Disponíveis</h2>
            <p class="text-xs text-slate-400">Prontos para designação imediata</p>
          </div>
          <div class="bg-white rounded-2xl p-4 space-y-3 shadow-sm">
            <div v-if="carregandoParamedicos" class="space-y-2">
              <div
                v-for="i in 3"
                :key="i"
                class="bg-slate-100 p-3 rounded-xl h-16 animate-pulse"
              ></div>
            </div>

            <div v-else-if="erroParamedicos" class="text-sm text-red-600 py-2">
              ⚠️ {{ erroParamedicos }}
            </div>

            <div
              v-else-if="paramedicosDisponiveis.length === 0"
              class="text-sm text-slate-400 py-4 text-center"
            >
              Nenhum paramédico disponível no momento.
            </div>

            <div
              v-for="para in paramedicosDisponiveis"
              :key="para.id"
              class="bg-slate-50 p-3 rounded-xl flex items-center space-x-3 shadow-sm"
            >
              <div
                class="w-12 h-12 rounded-full border-2 border-emerald-400 flex items-center justify-center bg-emerald-50 shrink-0"
              >
                <span class="material-symbols-outlined text-emerald-600">person</span>
              </div>
              <div class="flex-1 min-w-0">
                <p class="font-bold text-slate-800 truncate">{{ para.nome }}</p>
                <p class="text-xs text-slate-400">{{ para.cargo }}</p>
              </div>
            </div>
          </div>
          <RouterLink to="/equipe">
            <button
              class="w-full py-3 border-2 border-dashed border-slate-300 rounded-xl text-slate-500 font-bold text-sm hover:bg-slate-50 hover:border-slate-400 transition-all"
            >
              Ver todos paramédicos
            </button>
          </RouterLink>
        </section>
      </div>
    </main>
  </div>

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
            Tem certeza que deseja sair? Você precisará fazer login novamente para acessar o
            sistema.
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