<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const sidebarAberta = ref(false)
const mostrarModalSair = ref(false)
const router = useRouter()

const usuarioNome = ref(localStorage.getItem('usuario_nome') || 'Usuário')

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
</script>

<template>
  <div class="flex h-screen bg-slate-100 text-slate-800">
    <!-- Overlay mobile -->
    <div
      v-if="sidebarAberta"
      class="fixed inset-0 bg-black/40 z-40 lg:hidden"
      @click="sidebarAberta = false"
    ></div>

    <!-- SIDEBAR -->
    <aside
      :class="[
        'fixed left-0 top-0 h-full flex flex-col py-6 bg-slate-50 dark:bg-slate-900 w-64 border-r z-50 transition-transform duration-300',
        sidebarAberta ? 'translate-x-0' : '-translate-x-full lg:translate-x-0',
      ]"
    >
      <div class="px-6 mb-10">
        <span class="text-xl font-bold text-blue-900 dark:text-blue-50 tracking-tight"
          >SIGE Emergências</span
        >
      </div>
      <nav class="flex-1 space-y-1 flex flex-col px-2">
        <RouterLink
          to="/dashboard"
          @click="sidebarAberta = false"
          class="flex items-center gap-3 px-4 py-3 text-slate-500 hover:bg-slate-100 rounded-full transition-all"
        >
          <span class="material-symbols-outlined">dashboard</span>
          <span class="font-medium">Dashboard</span>
        </RouterLink>
        <RouterLink
          to="/chamado"
          @click="sidebarAberta = false"
          class="flex items-center gap-3 px-4 py-3 text-slate-500 hover:bg-slate-100 rounded-full transition-all"
        >
          <span class="material-symbols-outlined">pending_actions</span>
          <span class="font-medium">Chamados</span>
        </RouterLink>
        <RouterLink
          to="/equipe"
          @click="sidebarAberta = false"
          class="flex items-center gap-3 px-4 py-3 text-slate-500 hover:bg-slate-100 rounded-full transition-all"
        >
          <span class="material-symbols-outlined">groups_2</span>
          <span class="font-medium">Equipe</span>
        </RouterLink>
        <RouterLink
          to="/veiculos"
          @click="sidebarAberta = false"
          class="flex items-center gap-3 px-4 py-3 text-slate-500 hover:bg-slate-100 rounded-full transition-all"
        >
          <span class="material-symbols-outlined">ambulance</span>
          <span class="font-medium">Veículos</span>
        </RouterLink>
        <RouterLink
          to="/settings"
          @click="sidebarAberta = false"
          class="flex items-center gap-3 px-4 py-3 text-slate-500 hover:bg-slate-100 rounded-full transition-all"
        >
          <span class="material-symbols-outlined">settings</span>
          <span class="font-medium">Configurações</span>
        </RouterLink>
        <div class="flex-1"></div>
        <button
          @click="abrirModalSair"
          class="flex items-center gap-3 px-4 py-3 text-red-500 hover:bg-red-50 rounded-full transition-all w-full text-left cursor-pointer"
        >
          <span class="material-symbols-outlined">logout</span>
          <span class="font-medium">Sair</span>
        </button>
      </nav>
    </aside>

    <!-- CONTENT -->
    <main class="flex-1 lg:ml-64 p-8 overflow-y-auto">
      <header class="flex justify-between items-center mb-8">
        <div class="flex items-center gap-3">
          <button
            class="lg:hidden p-2 rounded-lg hover:bg-slate-100 transition-colors"
            @click="sidebarAberta = !sidebarAberta"
          >
            <span class="material-symbols-outlined">menu</span>
          </button>
          <h1 class="text-2xl font-bold text-blue-900">Configurações</h1>
        </div>
        <RouterLink
          to="/perfil"
          class="flex items-center gap-2 text-black hover:text-blue-600 transition-colors rounded-full font-bold"
        >
          <span class="hidden sm:block">{{ usuarioNome }}</span>
          <span class="material-symbols-outlined">account_circle</span>
        </RouterLink>
      </header>

      <!-- Mensagem de indisponibilidade -->
      <div class="flex flex-col items-center justify-center py-20">
        <span class="material-symbols-outlined text-slate-300 text-8xl mb-6">construction</span>
        <h2 class="text-2xl font-bold text-slate-400 mb-2">Em manutenção</h2>
        <p class="text-slate-400 text-center max-w-md">
          Esta funcionalidade ainda não está disponível no momento.
        </p>
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