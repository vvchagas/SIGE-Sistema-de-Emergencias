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
      <nav class="flex-1 space-y-1 flex flex-col px-2">
        <RouterLink to="/dashboard" class="flex items-center gap-3 px-4 py-3 text-slate-500 hover:bg-slate-100 rounded-full transition-all">
          <span class="material-symbols-outlined">dashboard</span>
          <span class="font-medium">Dashboard</span>
        </RouterLink>
        <RouterLink to="/chamado" class="flex items-center gap-3 px-4 py-3 text-slate-500 hover:bg-slate-100 rounded-full transition-all">
          <span class="material-symbols-outlined">pending_actions</span>
          <span class="font-medium">Chamados</span>
        </RouterLink>
        <RouterLink to="/equipe" class="flex items-center gap-3 px-4 py-3 text-slate-500 hover:bg-slate-100 rounded-full transition-all">
          <span class="material-symbols-outlined">groups_2</span>
          <span class="font-medium">Equipe</span>
        </RouterLink>
        <RouterLink to="/veiculos" class="flex items-center gap-3 px-4 py-3 text-slate-500 hover:bg-slate-100 rounded-full transition-all">
          <span class="material-symbols-outlined">ambulance</span>
          <span class="font-medium">Veículos</span>
        </RouterLink>
        <RouterLink to="/settings" class="flex items-center gap-3 px-4 py-3 text-slate-500 hover:bg-slate-100 rounded-full transition-all">
          <span class="material-symbols-outlined">settings</span>
          <span class="font-medium">Configurações</span>
        </RouterLink>
        <div class="flex-1"></div>
        <RouterLink to="/login" class="flex items-center gap-3 px-4 py-3 text-red-500 hover:bg-red-50 rounded-full transition-all">
          <span class="material-symbols-outlined">logout</span>
          <span class="font-medium">Sair</span>
        </RouterLink>
      </nav>
    </aside>

    <!-- CONTENT -->
    <main class="flex-1 ml-64 p-8 overflow-y-auto">
      <header class="flex justify-between items-center mb-8">
        <h1 class="text-2xl font-bold text-blue-900">Configurações do Sistema</h1>
        <RouterLink to="/perfil" class="flex items-center gap-2 text-black hover:text-blue-600 transition-colors font-bold">
          <span>Meu Perfil</span>
          <span class="material-symbols-outlined">account_circle</span>
        </RouterLink>
      </header>

      <div class="flex flex-col items-center justify-center h-[60vh] text-slate-400">
        <span class="material-symbols-outlined text-9xl mb-4">account_circle</span>
        <h2 class="text-2xl font-bold">Perfil do Usuário</h2>
        <p>A pagina de perfil do usuário estará disponível em breve.</p>
      </div>
    </main>
  </div>
</template>

