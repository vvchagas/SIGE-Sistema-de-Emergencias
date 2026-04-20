<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ambulanciasApi, type Ambulancia } from '../api/index';

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
  <div class="flex h-screen bg-slate-100 text-slate-800">
    <!-- SIDEBAR -->
    <aside class="fixed left-0 top-0 h-full flex flex-col py-6 bg-slate-50 dark:bg-slate-900 w-64 border-r z-50">
      <div class="px-6 mb-10">
        <span class="text-xl font-bold text-blue-900 dark:text-blue-50 tracking-tight">SIGE Emergências</span>
      </div>
      <nav class="flex-1 space-y-1 flex flex-col">
        <RouterLink to="/dashboard" class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full">
          <span class="material-symbols-outlined mr-3">dashboard</span>
          <span class="font-medium">Dashboard</span>
        </RouterLink>
        <RouterLink to="/chamado" class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full">
          <span class="material-symbols-outlined mr-3">pending_actions</span>
          <span class="font-medium">Chamados</span>
        </RouterLink>
        <RouterLink to="/equipe" class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full">
          <span class="material-symbols-outlined mr-3">groups_2</span>
          <span class="font-medium">Equipe</span>
        </RouterLink>
        <RouterLink to="/veiculos" class="bg-blue-900 text-white rounded-full mx-2 flex items-center px-4 py-3 scale-95 duration-150 transition-all">
          <span class="material-symbols-outlined mr-3">ambulance</span>
          <span class="font-medium">Veículos</span>
        </RouterLink>
        <RouterLink to="/settings" class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full">
          <span class="material-symbols-outlined mr-3">settings</span>
          <span class="font-medium">Configurações</span>
        </RouterLink>
        <div class="flex-1"></div>
        <RouterLink to="/login" class="text-red-500 hover:text-red-700 px-4 py-3 hover:bg-red-50 dark:hover:bg-red-900/20 transition-all flex items-center mx-2 rounded-full mb-4">
          <span class="material-symbols-outlined mr-3">logout</span>
          <span class="font-medium">Sair</span>
        </RouterLink>
      </nav>
    </aside>

    <!-- CONTENT -->
    <main class="flex-1 ml-64 p-8">
      <header class="flex justify-between items-center mb-8">
        <div>
          <h1 class="text-2xl font-bold text-blue-900">Frota de Veículos</h1>
          <p class="text-slate-500 text-sm">Módulo em desenvolvimento</p>
        </div>
        <RouterLink to="/perfil" class="flex items-center gap-2 text-black hover:text-blue-600 transition-colors font-bold">
          <span>Meu Perfil</span>
          <span class="material-symbols-outlined">account_circle</span>
        </RouterLink>
      </header>

      <div class="flex flex-col items-center justify-center h-[60vh] text-slate-400">
        <span class="material-symbols-outlined text-9xl mb-4">construction</span>
        <h2 class="text-2xl font-bold">Em Construção</h2>
        <p>Esta funcionalidade estará disponível em breve.</p>
      </div>
    </main>
  </div>
</template>
