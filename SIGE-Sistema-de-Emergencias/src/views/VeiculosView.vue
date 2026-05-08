<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { ambulanciasApi, type Ambulancia } from '../api/index';

const sidebarAberta = ref(false);
const mostrarModalSair = ref(false);
const router = useRouter();

function abrirModalSair() {
  sidebarAberta.value = false;
  mostrarModalSair.value = true;
}

function confirmarSaida() {
  mostrarModalSair.value = false;
  router.push('/login');
}

const ambulancias = ref<Ambulancia[]>([]);
const carregando = ref(true);
const erro = ref<string | null>(null);

const statusLabel: Record<number, string> = {
  0: 'Disponível',
  1: 'Manutenção',
  2: 'Em Uso',
};

const statusClass: Record<number, string> = {
  0: 'bg-emerald-100 text-emerald-700',
  1: 'bg-amber-100 text-amber-700',
  2: 'bg-blue-100 text-blue-700',
};

async function carregarVeiculos() {
  carregando.value = true;
  try {
    ambulancias.value = await ambulanciasApi.listar();
  } catch (e: unknown) {
    erro.value = e instanceof Error ? e.message : 'Erro ao carregar veículos';
  } finally {
    carregando.value = false;
  }
}

onMounted(carregarVeiculos);</script>

<template>
  <div>
    <div v-if="sidebarAberta" class="fixed inset-0 bg-black/40 z-40 lg:hidden" @click="sidebarAberta = false"></div>
    <aside
      :class="['fixed left-0 top-0 h-full flex flex-col py-6 bg-slate-50 dark:bg-slate-900 w-64 border-r-0 z-50 transition-transform duration-300', sidebarAberta ? 'translate-x-0' : '-translate-x-full lg:translate-x-0']">
      <div class="px-6 mb-10">
        <span class="text-xl font-bold text-blue-900 dark:text-blue-50 tracking-tight">SIGE Emergências</span>
      </div>
      <nav class="flex-1 space-y-1 flex flex-col">
        <RouterLink to="/dashboard" @click="sidebarAberta = false"
          class="text-slate-500 hover:text-blue-900 px-4 py-3 hover:bg-slate-200/50 transition-all flex items-center mx-2 rounded-full">
          <span class="material-symbols-outlined mr-3">dashboard</span>
          <span class="font-medium">Dashboard</span>
        </RouterLink>
        <RouterLink to="/chamado" @click="sidebarAberta = false"
          class="text-slate-500 hover:text-blue-900 px-4 py-3 hover:bg-slate-200/50 transition-all flex items-center mx-2 rounded-full">
          <span class="material-symbols-outlined mr-3">pending_actions</span>
          <span class="font-medium">Chamados</span>
        </RouterLink>
        <RouterLink to="/equipe" @click="sidebarAberta = false"
          class="text-slate-500 hover:text-blue-900 px-4 py-3 hover:bg-slate-200/50 transition-all flex items-center mx-2 rounded-full">
          <span class="material-symbols-outlined mr-3">groups_2</span>
          <span class="font-medium">Equipe</span>
        </RouterLink>
        <RouterLink to="/veiculos" @click="sidebarAberta = false"
          class="bg-blue-900 text-white rounded-full mx-2 flex items-center px-4 py-3 scale-95 transition-all">
          <span class="material-symbols-outlined mr-3">ambulance</span>
          <span class="font-medium">Veículos</span>
        </RouterLink>
        <RouterLink to="/settings" @click="sidebarAberta = false"
          class="text-slate-500 hover:text-blue-900 px-4 py-3 hover:bg-slate-200/50 transition-all flex items-center mx-2 rounded-full">
          <span class="material-symbols-outlined mr-3">settings</span>
          <span class="font-medium">Configurações</span>
        </RouterLink>
        <div class="flex-1"></div>
        <button @click="abrirModalSair"
          class="text-red-500 hover:text-red-700 px-4 py-3 hover:bg-red-50 transition-all flex items-center mx-2 rounded-full mb-4 w-full text-left cursor-pointer">
          <span class="material-symbols-outlined mr-3">logout</span>
          <span class="font-medium">Sair</span>
        </button>
      </nav>
    </aside>
    <header class="flex justify-between items-center w-full lg:pl-72 px-4 lg:pr-8 h-20 fixed top-0 bg-white/80 backdrop-blur-md z-40 border-b border-slate-100">
      <div class="flex items-center gap-3">
        <button class="lg:hidden p-2 rounded-lg hover:bg-slate-100 transition-colors" @click="sidebarAberta = !sidebarAberta">
          <span class="material-symbols-outlined">menu</span>
        </button>
        <h1 class="text-2xl font-bold text-blue-900 tracking-tight">Veículos</h1>
      </div>
      <div class="flex items-center gap-3">
        <RouterLink to="/perfil" class="flex gap-3 text-black hover:text-blue-600 transition-colors">
          <p class="hidden sm:block"><strong>Meu Perfil</strong></p>
          <span class="material-symbols-outlined">account_circle</span>
        </RouterLink>
      </div>
    </header>
    <main class="flex-1 lg:ml-64 p-8 pt-24">
      <div class="flex flex-col items-center justify-center h-[60vh] text-slate-400">
        <span class="material-symbols-outlined text-9xl mb-4">ambulance</span>
        <h2 class="text-2xl font-bold">Em Construção</h2>
        <p>A pagina de Veículos estará disponível em breve.</p>
      </div>
    </main>
  </div>

  <Teleport to="body">
    <div v-if="mostrarModalSair" class="fixed inset-0 bg-black/50 z-[100] flex items-center justify-center p-4">
      <div class="bg-white rounded-2xl shadow-2xl p-8 max-w-sm w-full">
        <div class="flex flex-col items-center text-center">
          <span class="material-symbols-outlined text-red-500 text-6xl mb-4">logout</span>
          <h2 class="text-xl font-bold text-slate-800 mb-2">Sair do sistema?</h2>
          <p class="text-slate-500 mb-6">Tem certeza que deseja sair? Você precisará fazer login novamente.</p>
          <div class="flex gap-3 w-full">
            <button @click="mostrarModalSair = false" class="flex-1 py-3 border-2 border-slate-200 text-slate-600 font-bold rounded-xl hover:bg-slate-50 transition-colors">Cancelar</button>
            <button @click="confirmarSaida" class="flex-1 py-3 bg-red-500 text-white font-bold rounded-xl hover:bg-red-600 transition-colors">Sim, sair</button>
          </div>
        </div>
      </div>
    </div>
  </Teleport>
</template>
