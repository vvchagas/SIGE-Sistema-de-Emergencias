<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import {
  paramedicosApi, ambulanciasApi,
  type Paramedico, type Ambulancia,
  type ParamedicoCreateDTO, type AmbulanciaCreateDTO
} from '../api/index';

const abaAtiva = ref<'paramedicos' | 'ambulancias'>('paramedicos');

// --- Paramedicos ---
const paramedicos = ref<Paramedico[]>([]);
const formP = reactive<ParamedicoCreateDTO>({
  name: '', cpf: '', telefone: '', email: '',
  identificadorProfissional: '', cargo: '', ocupado: false
});
const salvandoP = ref(false);

async function carregarP() {
  paramedicos.value = await paramedicosApi.listar();
}

async function salvarParamedico() {
  salvandoP.value = true;
  try {
    await paramedicosApi.criar({ ...formP });
    await carregarP();
    Object.assign(formP, { name: '', cpf: '', telefone: '', email: '', identificadorProfissional: '', cargo: '', ocupado: false });
    alert('Paramédico cadastrado!');
  } catch (e: unknown) {
    alert(e instanceof Error ? e.message : 'Erro ao cadastrar paramédico');
  } finally {
    salvandoP.value = false;
  }
}

// --- Ambulancias ---
const ambulancias = ref<Ambulancia[]>([]);
const formA = reactive<AmbulanciaCreateDTO>({
  placa: '', marca: '', modelo: '', tipo: ''
});
const salvandoA = ref(false);

async function carregarA() {
  ambulancias.value = await ambulanciasApi.listar();
}

async function salvarAmbulancia() {
  salvandoA.value = true;
  try {
    await ambulanciasApi.criar({ ...formA });
    await carregarA();
    Object.assign(formA, { placa: '', marca: '', modelo: '', tipo: '' });
    alert('Ambulância cadastrada!');
  } catch (e: unknown) {
    alert(e instanceof Error ? e.message : 'Erro ao cadastrar ambulância');
  } finally {
    salvandoA.value = false;
  }
}

onMounted(() => {
  carregarP();
  carregarA();
});
</script>

<template>
  <div>
    <aside
      class="fixed left-0 top-0 h-full flex flex-col py-6 bg-slate-50 dark:bg-slate-900 h-screen w-64 border-r-0 z-50">
      <div class="px-6 mb-10">
        <span class="text-xl font-bold text-blue-900 dark:text-blue-50 tracking-tight">SIGE Emergências</span>
      </div>
      <nav class="flex-1 space-y-1 flex flex-col">
        <RouterLink to="/dashboard"
          class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full">
          <span class="material-symbols-outlined mr-3" data-icon="dashboard">dashboard</span>
          <span class="font-medium">Dashboard</span>
        </RouterLink>
        <RouterLink to="/chamado"
          class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full">
          <span class="material-symbols-outlined mr-3" data-icon="pending_actions">pending_actions</span>
          <span class="font-medium">Chamados</span>
        </RouterLink>
        <RouterLink to="/equipe"
          class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full">
          <span class="material-symbols-outlined mr-3" data-icon="groups_2">groups_2</span>
          <span class="font-medium">Equipe</span>
        </RouterLink>
        <RouterLink to="/veiculos"
          class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full">
          <span class="material-symbols-outlined mr-3" data-icon="ambulance">ambulance</span>
          <span class="font-medium">Veículos</span>
        </RouterLink>
        <RouterLink to="/settings"
          class="bg-blue-900 dark:bg-blue-600 text-white rounded-full mx-2 flex items-center px-4 py-3 scale-95 active:scale-90 duration-150 transition-all">
          <span class="material-symbols-outlined mr-3" data-icon="settings">settings</span>
          <span class="font-medium">Configurações</span>
        </RouterLink>
        <div class="flex-1"></div>
        <RouterLink to="/login"
          class="text-red-500 hover:text-red-700 px-4 py-3 hover:bg-red-50 dark:hover:bg-red-900/20 transition-all flex items-center mx-2 rounded-full mb-4">
          <span class="material-symbols-outlined mr-3" data-icon="logout">logout</span>
          <span class="font-medium">Sair</span>
        </RouterLink>
      </nav>

    </aside>
    <header
      class="flex justify-between items-center w-full pl-72 pr-8 h-20 fixed top-0 bg-white/80 dark:bg-slate-950/80 backdrop-blur-md z-40 border-b border-slate-100 dark:border-slate-800">
      <h1 class="text-2xl font-bold font-headline text-blue-900 dark:text-blue-400 tracking-tight">
        Equipe
      </h1>
      <div class="flex items-center space-x-6">
        <div class="relative hidden lg:block">


        </div>
         <div class="flex items-center gap-3 cursor-pointer hover:bg-slate-50 p-2 -mr-2 rounded-lg transition-colors">

              <RouterLink to="/perfil" class="flex gap-3 text-black hover:text-blue-600 transition-colors">
                <p><strong>Meu Perfil</strong></p>
                <span
                  class="material-symbols-outlined"
                  data-icon="account_circle"
                  >account_circle</span>
              </RouterLink>
            </div>
      </div>
    </header>
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
        <span class="material-symbols-outlined text-9xl mb-4">settings_suggest</span>
        <h2 class="text-2xl font-bold">Em Construção</h2>
        <p>A pagina de Configurações estará disponível em breve.</p>
      </div>
    </main>
  </div>
</template>
