<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { authApi, type UserReadDto } from '../api/index'

const sidebarAberta = ref(false)
const mostrarModalSair = ref(false)
const router = useRouter()
const usuarioNome = ref(localStorage.getItem('usuario_nome') || 'Usuário')

const usuario = ref<UserReadDto | null>(null)
const carregando = ref(true)
const erro = ref<string | null>(null)

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

const iniciais = computed(() => {
  const nome = usuario.value?.nome?.trim()
  if (!nome) return '?'
  const partes = nome.split(/\s+/)
  const primeira = partes[0]?.[0] ?? ''
  const ultima = partes.length > 1 ? (partes[partes.length - 1]?.[0] ?? '') : ''
  return (primeira + ultima).toUpperCase() || '?'
})

async function carregarPerfil() {
  carregando.value = true
  erro.value = null
  try {
    usuario.value = await authApi.me()
  } catch (e: unknown) {
    erro.value = e instanceof Error ? e.message : 'Erro ao carregar perfil'
  } finally {
    carregando.value = false
  }
}

onMounted(carregarPerfil)
</script>

<template>
  <div>
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
      class="flex justify-between items-center w-full lg:pl-72 px-4 lg:pr-8 h-20 fixed top-0 bg-white dark:bg-slate-950 z-40 border-b border-slate-100 dark:border-slate-800"
    >
      <div class="flex items-center gap-3">
        <button
          class="lg:hidden p-2 rounded-lg hover:bg-slate-100 transition-colors"
          @click="sidebarAberta = !sidebarAberta"
        >
          <span class="material-symbols-outlined">menu</span>
        </button>
        <h1 class="text-2xl font-bold text-blue-900 dark:text-blue-400 tracking-tight">
          Meu Perfil
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
    <main class="lg:ml-64 p-6 lg:p-8 pt-32 lg:pt-36 min-h-screen bg-slate-50">

      <!-- Erro -->
      <div
        v-if="erro"
        class="bg-red-50 border border-red-200 text-red-700 px-4 py-4 rounded-2xl text-sm mb-6 flex items-center gap-3"
      >
        <span class="material-symbols-outlined text-red-500">error</span>
        <div>
          <p class="font-bold">Não foi possível carregar o perfil</p>
          <p>{{ erro }}</p>
        </div>
        <button @click="carregarPerfil" class="ml-auto underline font-semibold text-red-700">
          Tentar novamente
        </button>
      </div>

      <!-- Skeleton -->
      <div v-if="carregando && !erro">
        <div class="bg-white rounded-2xl shadow-sm p-8 mb-6 flex items-center gap-6 animate-pulse">
          <div class="w-24 h-24 rounded-full bg-slate-200 shrink-0"></div>
          <div class="space-y-3 flex-1">
            <div class="h-6 bg-slate-200 rounded-lg w-48"></div>
            <div class="h-4 bg-slate-100 rounded-lg w-64"></div>
            <div class="h-4 bg-slate-100 rounded-lg w-32"></div>
          </div>
        </div>
        <div class="bg-white rounded-2xl shadow-sm p-8 animate-pulse">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div v-for="i in 4" :key="i" class="space-y-2">
              <div class="h-3 bg-slate-100 rounded w-24"></div>
              <div class="h-10 bg-slate-100 rounded-xl"></div>
            </div>
          </div>
        </div>
      </div>

      <!-- Dados do perfil -->
      <template v-if="!carregando && !erro && usuario">
        <!-- Card de avatar -->
        <div class="bg-white rounded-2xl shadow-sm p-8 mb-6 flex flex-col sm:flex-row items-center sm:items-start gap-6">
          <div
            class="w-24 h-24 rounded-full bg-blue-900 flex items-center justify-center shrink-0 shadow-md"
          >
            <span class="text-3xl font-extrabold text-white">{{ iniciais }}</span>
          </div>
          <div class="text-center sm:text-left min-w-0 flex-1">
            <h2 class="text-2xl font-extrabold text-slate-800 break-words">{{ usuario.nome }}</h2>
            <p class="text-slate-500 mt-1 break-words">{{ usuario.email }}</p>
            <span
              v-if="usuario.cargo"
              class="inline-block mt-2 px-3 py-1 bg-blue-100 text-blue-800 text-xs font-bold rounded-full uppercase tracking-wide"
            >
              {{ usuario.cargo }}
            </span>
          </div>
        </div>

        <!-- Detalhes -->
        <div class="bg-white rounded-2xl shadow-sm p-8">
          <h3 class="text-base font-bold text-slate-700 mb-6 flex items-center gap-2">
            <span class="material-symbols-outlined text-blue-900">badge</span>
            Informações Pessoais
          </h3>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Nome -->
            <div>
              <label class="text-xs font-bold text-slate-400 uppercase tracking-wider block mb-1">
                Nome completo
              </label>
              <div class="bg-slate-50 rounded-xl px-4 py-3 text-slate-800 font-medium flex items-center gap-2">
                <span class="material-symbols-outlined text-slate-400 text-lg">person</span>
                {{ usuario.nome }}
              </div>
            </div>

            <!-- Email -->
            <div>
              <label class="text-xs font-bold text-slate-400 uppercase tracking-wider block mb-1">
                E-mail
              </label>
              <div class="bg-slate-50 rounded-xl px-4 py-3 text-slate-800 font-medium flex items-center gap-2">
                <span class="material-symbols-outlined text-slate-400 text-lg">mail</span>
                {{ usuario.email }}
              </div>
            </div>

            <!-- CPF -->
            <div>
              <label class="text-xs font-bold text-slate-400 uppercase tracking-wider block mb-1">
                CPF
              </label>
              <div class="bg-slate-50 rounded-xl px-4 py-3 text-slate-800 font-medium flex items-center gap-2">
                <span class="material-symbols-outlined text-slate-400 text-lg">fingerprint</span>
                {{ usuario.cpf ?? '—' }}
              </div>
            </div>

            <!-- Telefone -->
            <div>
              <label class="text-xs font-bold text-slate-400 uppercase tracking-wider block mb-1">
                Telefone
              </label>
              <div class="bg-slate-50 rounded-xl px-4 py-3 text-slate-800 font-medium flex items-center gap-2">
                <span class="material-symbols-outlined text-slate-400 text-lg">phone</span>
                {{ usuario.telefone ?? '—' }}
              </div>
            </div>

            <!-- Cargo -->
            <div class="md:col-span-2">
              <label class="text-xs font-bold text-slate-400 uppercase tracking-wider block mb-1">
                Cargo
              </label>
              <div class="bg-slate-50 rounded-xl px-4 py-3 text-slate-800 font-medium flex items-center gap-2">
                <span class="material-symbols-outlined text-slate-400 text-lg">work</span>
                {{ usuario.cargo ?? 'Não informado' }}
              </div>
            </div>
          </div>
        </div>
      </template>
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
