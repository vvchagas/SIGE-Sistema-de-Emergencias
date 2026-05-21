<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { paramedicosApi, type ParamedicoGetDTO } from '../api/index'

const usuarioNome = localStorage.getItem('usuario_nome') || 'Usuário'
const paramedicos = ref<ParamedicoGetDTO[]>([])
const carregando = ref(true)
const erro = ref<string | null>(null)
const sidebarAberta = ref(false)
const mostrarModalSair = ref(false)
const mostrandoFormulario = ref(false)

const router = useRouter()

// Formulário
const novoParamedico = ref({
  nome: '',
  cpf: '',
  telefone: '',
  email: '',
  identificadorProfissional: '',
  cargo: '',
})
const salvando = ref(false)
const erroSalvar = ref<string | null>(null)

const totalParamedicos = computed(() => paramedicos.value.length)
const disponiveis = computed(() => paramedicos.value.filter(p => !p.ocupado).length)
const ocupados = computed(() => paramedicos.value.filter(p => p.ocupado).length)

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

async function carregarEquipe() {
  carregando.value = true
  erro.value = null
  try {
    paramedicos.value = await paramedicosApi.listar()
  } catch (e: unknown) {
    erro.value = e instanceof Error ? e.message : 'Erro ao carregar equipe'
  } finally {
    carregando.value = false
  }
}

async function salvarParamedico() {
  salvando.value = true
  erroSalvar.value = null
  try {
    await paramedicosApi.criar({
      name: novoParamedico.value.nome,
      cpf: novoParamedico.value.cpf,
      telefone: novoParamedico.value.telefone,
      email: novoParamedico.value.email,
      identificadorProfissional: novoParamedico.value.identificadorProfissional,
      cargo: novoParamedico.value.cargo,
      ocupado: false,
    })
    await carregarEquipe()
    mostrandoFormulario.value = false
    novoParamedico.value = {
      nome: '',
      cpf: '',
      telefone: '',
      email: '',
      identificadorProfissional: '',
      cargo: '',
    }
  } catch (e: unknown) {
    erroSalvar.value = e instanceof Error ? e.message : 'Erro ao salvar paramédico'
  } finally {
    salvando.value = false
  }
}

async function deletarParamedico(id: string) {
  if (!confirm('Tem certeza que deseja excluir este paramédico?')) return
  try {
    await paramedicosApi.deletar(id)
    await carregarEquipe()
  } catch (e: unknown) {
    alert(e instanceof Error ? e.message : 'Erro ao deletar paramédico')
  }
}

onMounted(carregarEquipe)
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
          class="bg-blue-900 dark:bg-blue-600 text-white rounded-full mx-2 flex items-center px-4 py-3 scale-95 active:scale-90 duration-150 transition-all"
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
          class="text-2xl font-bold text-blue-900 dark:text-blue-400 tracking-tight"
        >
          Equipe
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

    <!-- CONTENT -->
    <main class="flex-1 lg:ml-64 p-8 overflow-y-auto pt-24">
      <!-- Stats -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div class="bg-white p-6 rounded-xl shadow-sm border-l-4 border-l-blue-500">
          <p class="text-sm font-medium text-slate-500 uppercase tracking-wider">Total</p>
          <p class="text-3xl font-extrabold text-slate-800">{{ totalParamedicos }}</p>
        </div>
        <div class="bg-white p-6 rounded-xl shadow-sm border-l-4 border-l-emerald-500">
          <p class="text-sm font-medium text-slate-500 uppercase tracking-wider">Disponíveis</p>
          <p class="text-3xl font-extrabold text-emerald-600">{{ disponiveis }}</p>
        </div>
        <div class="bg-white p-6 rounded-xl shadow-sm border-l-4 border-l-amber-500">
          <p class="text-sm font-medium text-slate-500 uppercase tracking-wider">Em Atendimento</p>
          <p class="text-3xl font-extrabold text-amber-600">{{ ocupados }}</p>
        </div>
      </div>

      <!-- Actions -->
      <div class="flex items-center justify-between mb-6">
        <h2 class="text-xl font-bold text-slate-800">Lista de Paramédicos</h2>
        <button
          @click="mostrandoFormulario = !mostrandoFormulario"
          class="px-4 py-2 bg-blue-900 text-white rounded-full text-sm font-bold hover:bg-blue-800 transition-colors flex items-center gap-2"
        >
          <span class="material-symbols-outlined text-sm">{{ mostrandoFormulario ? 'close' : 'add' }}</span>
          {{ mostrandoFormulario ? 'Fechar' : 'Novo Paramédico' }}
        </button>
      </div>

      <!-- Formulário -->
      <div v-if="mostrandoFormulario" class="bg-white rounded-xl p-6 shadow-sm mb-6">
        <h3 class="font-bold text-indigo-950 mb-4">Cadastrar Novo Paramédico</h3>

        <div v-if="erroSalvar" class="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm">
          ⚠️ {{ erroSalvar }}
        </div>

        <form @submit.prevent="salvarParamedico" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <div class="flex flex-col gap-1">
            <label class="text-xs font-bold text-slate-500 uppercase">Nome *</label>
            <input
              type="text"
              v-model="novoParamedico.nome"
              required
              class="bg-slate-50 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
              placeholder="Nome completo"
            />
          </div>
          <div class="flex flex-col gap-1">
            <label class="text-xs font-bold text-slate-500 uppercase">CPF *</label>
            <input
              type="text"
              v-model="novoParamedico.cpf"
              required
              class="bg-slate-50 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
              placeholder="Somente números"
            />
          </div>
          <div class="flex flex-col gap-1">
            <label class="text-xs font-bold text-slate-500 uppercase">Telefone *</label>
            <input
              type="text"
              v-model="novoParamedico.telefone"
              required
              class="bg-slate-50 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
              placeholder="(11) 99999-9999"
            />
          </div>
          <div class="flex flex-col gap-1">
            <label class="text-xs font-bold text-slate-500 uppercase">Email *</label>
            <input
              type="email"
              v-model="novoParamedico.email"
              required
              class="bg-slate-50 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
              placeholder="email@exemplo.com"
            />
          </div>
          <div class="flex flex-col gap-1">
            <label class="text-xs font-bold text-slate-500 uppercase">Identificador Profissional *</label>
            <input
              type="text"
              v-model="novoParamedico.identificadorProfissional"
              required
              class="bg-slate-50 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
              placeholder="CRM/COREN etc"
            />
          </div>
          <div class="flex flex-col gap-1">
            <label class="text-xs font-bold text-slate-500 uppercase">Cargo *</label>
            <input
              type="text"
              v-model="novoParamedico.cargo"
              required
              class="bg-slate-50 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
              placeholder="Ex: Técnico de Enfermagem"
            />
          </div>
          <div class="md:col-span-2 lg:col-span-3 flex gap-3 mt-2">
            <button
              type="submit"
              :disabled="salvando"
              class="px-6 py-2 bg-blue-900 text-white rounded-lg text-sm font-bold hover:bg-blue-800 transition-colors disabled:opacity-60 flex items-center gap-2"
            >
              <span v-if="salvando" class="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></span>
              {{ salvando ? 'Salvando...' : 'Salvar Paramédico' }}
            </button>
          </div>
        </form>
      </div>

      <!-- Erro -->
      <div
        v-if="erro"
        class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm mb-6"
      >
        ⚠️ {{ erro }}
      </div>

      <!-- Loading -->
      <div v-if="carregando" class="space-y-3">
        <div v-for="i in 3" :key="i" class="bg-white p-5 rounded-xl h-20 animate-pulse"></div>
      </div>

      <!-- Empty -->
      <div
        v-else-if="!carregando && paramedicos.length === 0 && !erro"
        class="flex flex-col items-center justify-center py-16 text-slate-400"
      >
        <span class="material-symbols-outlined text-6xl mb-3">groups_2</span>
        <p class="font-medium">Nenhum paramédico cadastrado</p>
      </div>

      <!-- Lista -->
      <div v-else class="space-y-3">
        <div
          v-for="paramedico in paramedicos"
          :key="paramedico.id"
          class="bg-white p-5 rounded-xl shadow-sm flex items-center gap-4 hover:shadow-md transition-shadow"
        >
          <div
            :class="[
              'w-12 h-12 rounded-full flex items-center justify-center shrink-0',
              paramedico.ocupado ? 'bg-amber-100' : 'bg-emerald-100'
            ]"
          >
            <span
              :class="[
                'material-symbols-outlined',
                paramedico.ocupado ? 'text-amber-600' : 'text-emerald-600'
              ]"
            >person</span>
          </div>
          <div class="flex-1 min-w-0">
            <p class="font-bold text-slate-800">{{ paramedico.nome }}</p>
            <p class="text-sm text-slate-500">{{ paramedico.cargo }} - {{ paramedico.identificadorProfissional }}</p>
            <p class="text-xs text-slate-400">{{ paramedico.email }}</p>
          </div>
          <div class="text-right">
            <span
              :class="[
                'px-3 py-1 rounded-full text-xs font-bold',
                paramedico.ocupado ? 'bg-amber-100 text-amber-700' : 'bg-emerald-100 text-emerald-700'
              ]"
            >
              {{ paramedico.ocupado ? 'Em Atendimento' : 'Disponível' }}
            </span>
            <button
              @click="deletarParamedico(paramedico.id)"
              class="block mt-2 text-red-500 hover:text-red-700 text-xs font-medium"
            >
              Excluir
            </button>
          </div>
        </div>
      </div>
    </main>
  </div>

  <!-- Modal de confirmação de saída -->
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