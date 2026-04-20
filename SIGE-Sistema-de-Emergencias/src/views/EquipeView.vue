<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { paramedicosApi, type Paramedico } from '../api/index';

const paramedicos = ref<Paramedico[]>([]);
const carregando = ref(true);
const erro = ref<string | null>(null);

async function carregarEquipe() {
  carregando.value = true;
  try {
    paramedicos.value = await paramedicosApi.listar();
  } catch (e: unknown) {
    erro.value = e instanceof Error ? e.message : 'Erro ao carregar equipe';
  } finally {
    carregando.value = false;
  }
}

onMounted(carregarEquipe);
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
        <RouterLink to="/equipe" class="bg-blue-900 text-white rounded-full mx-2 flex items-center px-4 py-3 scale-95 duration-150 transition-all">
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
        <div>
          <h1 class="text-2xl font-bold text-blue-900">Equipe de Paramédicos</h1>
          <p class="text-slate-500 text-sm">Visualização de todos os profissionais cadastrados</p>
        </div>
        <RouterLink to="/perfil" class="flex items-center gap-2 text-black hover:text-blue-600 transition-colors font-bold">
          <span>Meu Perfil</span>
          <span class="material-symbols-outlined">account_circle</span>
        </RouterLink>
      </header>

      <div v-if="carregando" class="flex justify-center p-20">
        <span class="animate-spin material-symbols-outlined text-4xl text-blue-900">progress_activity</span>
      </div>
      <div v-else-if="erro" class="p-4 bg-red-50 border border-red-200 text-red-600 rounded-xl flex items-center gap-3">
         <span class="material-symbols-outlined">error</span>
         {{ erro }}
      </div>
      <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div v-for="p in paramedicos" :key="p.id" class="bg-white p-6 rounded-2xl shadow-sm border hover:shadow-md transition-shadow">
          <div class="flex justify-between items-start mb-4">
            <div class="w-12 h-12 rounded-full bg-slate-100 flex items-center justify-center text-slate-600">
              <span class="material-symbols-outlined">person</span>
            </div>
            <span :class="p.ocupado ? 'bg-amber-100 text-amber-700' : 'bg-emerald-100 text-emerald-700'" class="text-[10px] font-bold px-2 py-0.5 rounded-full uppercase">
              {{ p.ocupado ? 'Em Atendimento' : 'Disponível' }}
            </span>
          </div>
          <h3 class="font-bold text-lg leading-tight">{{ p.name }}</h3>
          <p class="text-xs font-bold text-blue-900 uppercase mt-1">{{ p.cargo }}</p>
          <div class="mt-4 space-y-1 text-sm text-slate-500">
            <p class="flex items-center gap-2 px-1">
              <span class="material-symbols-outlined text-sm">badge</span> {{ p.identificadorProfissional }}
            </p>
            <p class="flex items-center gap-2 px-1">
              <span class="material-symbols-outlined text-sm">call</span> {{ p.telefone }}
            </p>
          </div>
        </div>
        
        <div v-if="paramedicos.length === 0" class="col-span-full py-20 text-center opacity-40">
           <span class="material-symbols-outlined text-6xl">person_off</span>
           <p class="text-xl font-bold mt-2">Nenhum paramédico encontrado</p>
        </div>
      </div>
    </main>
  </div>
</template>
