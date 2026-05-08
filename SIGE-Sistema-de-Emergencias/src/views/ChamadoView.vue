<script setup lang="ts">
import { reactive, ref } from 'vue';
import { useRouter } from 'vue-router';

const form = reactive({
  solicitanteNome: '',
  solicitanteTelefone: '',
  ocorrenciaEndereco: '',
  ocorrenciaBairro: '',
  ocorrenciaCidade: '',
  ocorrenciaReferencia: '',
  naturezaEmergencia: '',
  descricaoPaciente: ''
});

const router = useRouter();
const sidebarAberta = ref(false);
const mostrarModalSair = ref(false);

function abrirModalSair() {
  sidebarAberta.value = false;
  mostrarModalSair.value = true;
}

function confirmarSaida() {
  mostrarModalSair.value = false;
  router.push('/login');
}
</script>


<template>
<div class="flex h-screen overflow-hidden bg-slate-100 text-slate-800">
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
      sidebarAberta ? 'translate-x-0' : '-translate-x-full lg:translate-x-0'
    ]"
  >
    <div class="px-6 mb-10">
      <span class="text-xl font-bold text-blue-900 dark:text-blue-50 tracking-tight">SIGE Emergências</span>
    </div>
    <nav class="flex-1 space-y-1 flex flex-col">
      <RouterLink to="/dashboard" @click="sidebarAberta = false"
        class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full">
        <span class="material-symbols-outlined mr-3" data-icon="dashboard">dashboard</span>
        <span class="font-medium">Dashboard</span>
      </RouterLink>
      <!-- Active: Chamados -->
      <RouterLink to="/chamado" @click="sidebarAberta = false"
        class="bg-blue-900 dark:bg-blue-600 text-white rounded-full mx-2 flex items-center px-4 py-3 scale-95 active:scale-90 duration-150 transition-all">
        <span class="material-symbols-outlined mr-3" data-icon="pending_actions">pending_actions</span>
        <span class="font-medium">Chamados</span>
      </RouterLink>
      <RouterLink to="/equipe" @click="sidebarAberta = false"
        class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full">
        <span class="material-symbols-outlined mr-3" data-icon="groups_2">groups_2</span>
        <span class="font-medium">Equipe</span>
      </RouterLink>
      <RouterLink to="/veiculos" @click="sidebarAberta = false"
        class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full">
        <span class="material-symbols-outlined mr-3" data-icon="ambulance">ambulance</span>
        <span class="font-medium">Veículos</span>
      </RouterLink>
      <RouterLink to="/settings" @click="sidebarAberta = false"
        class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full">
        <span class="material-symbols-outlined mr-3" data-icon="settings">settings</span>
        <span class="font-medium">Configurações</span>
      </RouterLink>
      <div class="flex-1"></div>
      <button
        @click="abrirModalSair"
        class="text-red-500 hover:text-red-700 px-4 py-3 hover:bg-red-50 dark:hover:bg-red-900/20 transition-all flex items-center mx-2 rounded-full mb-4 w-full text-left cursor-pointer">
        <span class="material-symbols-outlined mr-3" data-icon="logout">logout</span>
        <span class="font-medium">Sair</span>
      </button>
    </nav>
  </aside>

      <main class="flex-1 flex flex-col overflow-y-auto lg:ml-64">
        <!-- PARTE DE CIMA -->
        <header
          class="bg-white border-b flex items-center justify-between px-4 lg:px-6 min-h-[80px]">
          <div class="flex items-center gap-3">
            <!-- Botão hambúrguer mobile -->
            <button
              class="lg:hidden p-2 rounded-lg hover:bg-slate-100 transition-colors"
              @click="sidebarAberta = !sidebarAberta"
            >
              <span class="material-symbols-outlined">menu</span>
            </button>
            <h1 class="text-2xl font-bold font-headline text-blue-900 dark:text-blue-400 tracking-tight">Novo Chamado</h1>
          </div>
          <div class="flex items-center gap-6">
            <div class="flex items-center gap-3 cursor-pointer hover:bg-slate-50 p-2 -mr-2 rounded-lg transition-colors">
              <RouterLink to="/perfil" class="flex gap-3 text-black hover:text-blue-600 transition-colors">
                <p class="hidden sm:block"><strong>Meu Perfil</strong></p>
                <span class="material-symbols-outlined" data-icon="account_circle">account_circle</span>
              </RouterLink>
            </div>
          </div>
        </header>


        <div class="p-8 max-w-[1400px] w-full mx-auto">
          <div class="mb-6">
            <h2 class="text-2xl font-extrabold text-indigo-950">
              Novo Chamado
            </h2>

            <p class="text-sm text-slate-500">
              Preencha os dados com precisão para despacho imediato.
            </p>
          </div>

          <form class="grid grid-cols-1 lg:grid-cols-[1fr_340px] gap-6">


            <div class="flex flex-col gap-6">
              <!-- BLOCOS -->

              <div class="bg-white rounded-xl p-6 shadow-sm">
                <div class="flex items-center gap-2 mb-5">
                  <i class="ph ph-user text-indigo-950 text-xl"></i>

                  <h3 class="font-bold text-indigo-950">
                    Identificação do Solicitante
                  </h3>
                </div>

                <div class="flex gap-4">
                  <div class="flex flex-col gap-2 w-1/2">
                    <label
                      class="text-[11px] font-bold text-slate-500 uppercase"
                    >
                      Nome do solicitante
                    </label>

                    <input
                      type="text"
                      v-model="form.solicitanteNome"
                      placeholder="xxxxx"
                      class="bg-slate-100 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
                    />
                  </div>

                  <div class="flex flex-col gap-2 w-1/2">
                    <label
                      class="text-[11px] font-bold text-slate-500 uppercase"
                    >
                      Telefone
                    </label>

                    <input
                      type="text"
                      v-model="form.solicitanteTelefone"
                      placeholder="xxxxx"
                      class="bg-slate-100 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
                    />
                  </div>
                </div>
              </div>

              <!-- BLOCO -->
              <div class="bg-white rounded-xl p-6 shadow-sm">
                <div class="flex items-center gap-2 mb-5">
                  <i class="ph ph-map-pin text-indigo-950 text-xl font-bold"></i>

                  <h3 class="font-bold text-indigo-950">
                    Localização da Ocorrência
                  </h3>
                </div>

                <div class="flex flex-col gap-4">
                  <div class="flex flex-col gap-2">
                    <label class="text-[11px] font-bold text-slate-500 uppercase tracking-wide">
                      Endereço (Rua, Número)
                    </label>

                    <div class="relative">
                      <i class="ph ph-map-pin absolute left-3 top-1/2 -translate-y-1/2 text-slate-400"></i>
                      <input
                        type="text"
                        v-model="form.ocorrenciaEndereco"
                        placeholder="xxxxx"
                        class="bg-slate-50 rounded-lg pl-9 pr-3 py-3 text-sm focus:ring-2 focus:ring-blue-200 outline-none w-full"
                      />
                    </div>
                  </div>

                  <div class="flex gap-4">
                    <div class="flex flex-col gap-2 w-1/2">
                      <label class="text-[11px] font-bold text-slate-500 uppercase tracking-wide">
                        Bairro
                      </label>

                      <input
                        type="text"
                        v-model="form.ocorrenciaBairro"
                        placeholder="xxxxx"
                        class="bg-slate-50 rounded-lg px-3 py-3 text-sm focus:ring-2 focus:ring-blue-200 outline-none w-full"
                      />
                    </div>

                    <div class="flex flex-col gap-2 w-1/2">
                      <label class="text-[11px] font-bold text-slate-500 uppercase tracking-wide">
                        Cidade
                      </label>

                      <input
                        type="text"
                        v-model="form.ocorrenciaCidade"
                        placeholder="xxxxx"
                        class="bg-slate-50 rounded-lg px-3 py-3 text-sm focus:ring-2 focus:ring-blue-200 outline-none w-full"
                      />
                    </div>
                  </div>

                  <div class="flex flex-col gap-2">
                    <label class="text-[11px] font-bold text-slate-500 uppercase tracking-wide">
                      Pontos de Referência
                    </label>

                    <input
                      type="text"
                      v-model="form.ocorrenciaReferencia"
                      placeholder="xxxxx"
                      class="bg-slate-50 rounded-lg px-3 py-3 text-sm focus:ring-2 focus:ring-blue-200 outline-none w-full"
                    />
                  </div>
                </div>
              </div>

              <!-- BlOCOS -->
              <div class="bg-white rounded-xl p-6 shadow-sm">
                <div class="flex items-center gap-2 mb-5">
                  <i class="ph ph-first-aid text-indigo-950 text-xl font-bold"></i>

                  <h3 class="font-bold text-indigo-950">
                    Detalhes Clínicos
                  </h3>
                </div>

                <div class="flex flex-col gap-6">
                  <div class="flex gap-4">
                    <div class="flex flex-col gap-2 w-1/2">
                      <label class="text-[11px] font-bold text-slate-500 uppercase tracking-wide">
                        Natureza da Emergência
                      </label>

                      <div class="relative">
                        <select
                          v-model="form.naturezaEmergencia"
                          class="appearance-none bg-slate-50 rounded-lg pl-4 pr-10 py-3 text-sm focus:ring-2 focus:ring-blue-200 outline-none w-full text-slate-700 cursor-pointer border border-transparent hover:border-slate-200 transition-all"
                        >
                          <option value="" disabled selected>xxxxx</option>
                          <option value="trauma">Trauma</option>
                          <option value="clinico">Clínico</option>
                          <option value="psiquiatrico">Psiquiátrico</option>
                          <option value="obstetrico">Obstétrico</option>
                        </select>
                        <i class="ph ph-caret-down absolute right-4 top-1/2 -translate-y-1/2 text-slate-400 pointer-events-none"></i>
                      </div>
                    </div>

                    <div class="flex flex-col gap-2 w-1/2">
                      <label class="text-[11px] font-bold text-slate-500 uppercase tracking-wide">
                        Estado de Consciência
                      </label>

                      <div class="flex gap-3 h-11">
                        <button
                          type="button"
                          class="flex-1 bg-white border border-slate-100 text-slate-800 font-bold text-[13px] rounded-lg hover:border-red-200 hover:bg-red-100 hover:text-red-700 transition-all cursor-pointer"
                        >
                          Consciente
                        </button>
                        <button
                          type="button"
                          class="flex-1 bg-white border border-slate-100 text-slate-800 font-bold text-[13px] rounded-lg hover:border-red-200 hover:bg-red-100 hover:text-red-700 transition-all cursor-pointer"
                        >
                          Confuso
                        </button>
                        <button
                          type="button"
                          class="flex-1 bg-white border border-slate-100 text-slate-800 font-bold text-[13px] rounded-lg hover:border-red-200 hover:bg-red-100 hover:text-red-700 transition-all cursor-pointer"
                        >
                          Inconsciente
                        </button>
                      </div>
                    </div>
                  </div>
                  <div class="flex flex-col gap-2">
                    <label class="text-[11px] font-bold text-slate-500 uppercase tracking-wide">
                      Descrição do paciente e sintomas
                    </label>

                    <textarea
                      v-model="form.descricaoPaciente"
                      rows="3"
                      placeholder="xxxxx"
                      class="bg-slate-50 rounded-lg px-4 py-3 text-[14px] focus:ring-2 focus:ring-blue-200 outline-none w-full placeholder-slate-400 resize-none border border-transparent hover:border-slate-200 transition-all"
                    ></textarea>
                  </div>
                </div>
              </div>
            </div>

            <!-- Parte da DIreita -->

            <div>
              <div class="bg-white rounded-xl p-6 h-full shadow-sm">
                <h3 class="font-bold text-indigo-950 mb-6">
                  Triagem e Prioridade
                </h3>

                <div class="flex flex-col gap-3">
                  <!-- Nível 1 -->
                  <div class="flex items-center gap-4 p-4 rounded-xl cursor-pointer bg-red-50 border border-red-50 hover:border-red-200 transition-all">
                    <div class="w-3.5 h-3.5 rounded-full bg-red-700 shrink-0"></div>
                    <div class="flex flex-col mt-0.5">
                      <h4 class="font-bold text-sm text-red-700">Nível 1 - Emergência</h4>
                      <p class="text-[11px] font-medium text-red-400">Risco<br>Imediato de vida</p>
                    </div>
                  </div>

                  <!-- Nível 2 -->
                  <div class="flex items-center gap-4 p-4 rounded-xl cursor-pointer bg-amber-50 border border-amber-50 hover:border-amber-200 transition-all">
                    <div class="w-3.5 h-3.5 rounded-full bg-amber-500 shrink-0"></div>
                    <div class="flex flex-col mt-0.5">
                      <h4 class="font-bold text-sm text-amber-700">Nível 2 - Urgência</h4>
                      <p class="text-[11px] font-medium text-amber-500">Potencial<br>Risco de vida</p>
                    </div>
                  </div>

                  <!-- Nível 3 -->
                  <div class="flex items-center gap-4 p-4 rounded-xl cursor-pointer bg-green-50 border border-green-50 hover:border-green-200 transition-all">
                    <div class="w-3.5 h-3.5 rounded-full bg-emerald-500 shrink-0"></div>
                    <div class="flex flex-col mt-0.5">
                      <h4 class="font-bold text-sm text-emerald-700">Nível 3 - Pouco Urgente</h4>
                      <p class="text-[11px] font-medium text-emerald-500">Estável,<br>aguardo tolerável</p>
                    </div>
                  </div>
                </div>

                <button
                  type="submit"
                  class="w-full bg-indigo-950 text-white mt-6 rounded-full py-4 font-bold flex items-center justify-center gap-3 hover:bg-indigo-900 transition-colors"
                >
                  <i class="ph ph-car-profile text-xl"></i>

                  Despachar Ambulância
                </button>
                <RouterLink to="/dashboard">
                <button
                  type="button"
                  class="w-full bg-white text-slate-500 border border-slate-200 mt-3 rounded-full py-4 font-bold flex items-center justify-center gap-3 hover:bg-slate-50 hover:text-slate-700 transition-colors"
                >
                  <i class="ph ph-x text-xl"></i>

                  Cancelar Registro
                </button>
                </RouterLink>
              </div>
            </div>
          </form>
        </div>
      </main>
</div>

<!-- Modal de confirmação de saída -->
<Teleport to="body">
  <div v-if="mostrarModalSair" class="fixed inset-0 bg-black/50 z-[100] flex items-center justify-center p-4">
    <div class="bg-white rounded-2xl shadow-2xl p-8 max-w-sm w-full">
      <div class="flex flex-col items-center text-center">
        <span class="material-symbols-outlined text-red-500 text-6xl mb-4">logout</span>
        <h2 class="text-xl font-bold text-slate-800 mb-2">Sair do sistema?</h2>
        <p class="text-slate-500 mb-6">Tem certeza que deseja sair? Você precisará fazer login novamente para acessar o sistema.</p>
        <div class="flex gap-3 w-full">
          <button
            @click="mostrarModalSair = false"
            class="flex-1 py-3 border-2 border-slate-200 text-slate-600 font-bold rounded-xl hover:bg-slate-50 transition-colors"
          >Cancelar</button>
          <button
            @click="confirmarSaida"
            class="flex-1 py-3 bg-red-500 text-white font-bold rounded-xl hover:bg-red-600 transition-colors"
          >Sim, sair</button>
        </div>
      </div>
    </div>
  </div>
</Teleport>
</template>
