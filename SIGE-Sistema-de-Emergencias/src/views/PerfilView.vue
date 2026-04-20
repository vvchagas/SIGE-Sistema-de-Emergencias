<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { authApi, type UserReadDto } from '../api/index';

const usuario = ref<UserReadDto | null>(null);
const carregando = ref(true);

onMounted(async () => {
  try {
    usuario.value = await authApi.me();
  } catch (e: unknown) {
    console.error('Erro ao buscar perfil:', e);
  } finally {
    carregando.value = false;
  }
});
</script>

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
        <RouterLink to="/veiculos" class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full">
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
        <h1 class="text-2xl font-bold text-blue-900">Meu Perfil</h1>
        <RouterLink to="/perfil" class="flex items-center gap-2 text-black hover:text-blue-600 transition-colors font-bold">
          <span>Meu Perfil</span>
          <span class="material-symbols-outlined">account_circle</span>
        </RouterLink>
      </header>

      <div class="bg-white rounded-2xl p-8 shadow-sm max-w-2xl border">
        <div v-if="carregando" class="flex justify-center p-12">
          <span class="animate-spin material-symbols-outlined text-4xl text-blue-900">progress_activity</span>
        </div>
        <div v-else-if="usuario" class="space-y-6">
          <div class="flex items-center gap-6 pb-6 border-b">
            <div class="w-24 h-24 rounded-full bg-blue-100 flex items-center justify-center text-blue-900">
              <span class="material-symbols-outlined text-6xl">account_circle</span>
            </div>
            <div>
              <h2 class="text-2xl font-bold">{{ usuario.name }}</h2>
              <p class="text-slate-500">{{ usuario.email }}</p>
            </div>
          </div>
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="text-xs font-bold text-slate-400 uppercase">ID de Usuário</label>
              <p class="font-mono text-sm bg-slate-50 p-2 rounded">{{ usuario.id }}</p>
            </div>
            <div>
              <label class="text-xs font-bold text-slate-400 uppercase">Status</label>
              <p class="text-emerald-600 font-bold">Ativo / Central</p>
            </div>
          </div>
        </div>
        <div v-else class="text-center text-red-500">
          Não foi possível carregar as informações do usuário.
        </div>
      </div>
    </main>
  </div>
</template>
