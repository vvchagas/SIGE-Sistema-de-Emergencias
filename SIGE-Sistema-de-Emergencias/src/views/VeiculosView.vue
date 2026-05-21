<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { ambulanciasApi, type AmbulanciaGetDTO } from '../api/index'

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

const ambulancias = ref<AmbulanciaGetDTO[]>([])
const carregando = ref(true)
const erro = ref<string | null>(null)
const mostrandoFormulario = ref(false)

// Formulário
const novaAmbulancia = ref({
  placa: '',
  marca: '',
  modelo: '',
  tipo: '',
})
const salvando = ref(false)
const erroSalvar = ref<string | null>(null)

const statusLabel: Record<number, string> = {
  0: 'Disponível',
  1: 'Manutenção',
  2: 'Em Uso',
}

const statusClass: Record<number, string> = {
  0: 'bg-emerald-100 text-emerald-700 border-l-emerald-500',
  1: 'bg-amber-100 text-amber-700 border-l-amber-500',
  2: 'bg-blue-100 text-blue-700 border-l-blue-500',
}

const totalAmbulancias = computed(() => ambulancias.value.length)
const disponiveis = computed(() => ambulancias.value.filter(a => a.status === 0).length)
const emUso = computed(() => ambulancias.value.filter(a => a.status === 2).length)

async function carregarVeiculos() {
  carregando.value = true
  erro.value = null
  try {
    ambulancias.value = await ambulanciasApi.listar()
  } catch (e: unknown) {
    erro.value = e instanceof Error ? e.message : 'Erro ao carregar veículos'
  } finally {
    carregando.value = false
  }
}

async function salvarAmbulancia() {
  salvando.value = true
  erroSalvar.value = null
  try {
    await ambulanciasApi.criar({
      placa: novaAmbulancia.value.placa,
      marca: novaAmbulancia.value.marca,
      modelo: novaAmbulancia.value.modelo,
      tipo: novaAmbulancia.value.tipo,
    })
    await carregarVeiculos()
    mostrandoFormulario.value = false
    novaAmbulancia.value = { placa: '', marca: '', modelo: '', tipo: '' }
  } catch (e: unknown) {
    erroSalvar.value = e instanceof Error ? e.message : 'Erro ao salvar ambulância'
  } finally {
    salvando.value = false
  }
}

async function deletarAmbulancia(id: string) {
  if (!confirm('Tem certeza que deseja excluir esta ambulância?')) return
  try {
    await ambulanciasApi.deletar(id)
    await carregarVeiculos()
  } catch (e: unknown) {
    alert(e instanceof Error ? e.message : 'Erro ao deletar ambulância')
  }
}

onMounted(carregarVeiculos)
</script>

<template>
  <div>
    <div
      v-if="sidebarAberta"
      class="fixed inset-0 bg-black/40 z-40 lg:hidden"
      @click="sidebarAberta = false"
    ></div>
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
          class="text-slate-500 hover:text-blue-900 px-4 py-3 hover:bg-slate-200/50 transition-all flex items-center mx-2 rounded-full"
        >
          <span class="material-symbols-outlined mr-3">dashboard</span>
          <span class="font-medium">Dashboard</span>
        </RouterLink>
        <RouterLink
          to="/chamado"
          @click="sidebarAberta = false"
          class="text-slate-500 hover:text-blue-900 px-4 py-3 hover:bg-slate-200/50 transition-all flex items-center mx-2 rounded-full"
        >
          <span class="material-symbols-outlined mr-3">pending_actions</span>
          <span class="font-medium">Chamados</span>
        </RouterLink>
        <RouterLink
          to="/equipe"
          @click="sidebarAberta = false"
          class="text-slate-500 hover:text-blue-900 px-4 py-3 hover:bg-slate-200/50 transition-all flex items-center mx-2 rounded-full"
        >
          <span class="material-symbols-outlined mr-3">groups_2</span>
          <span class="font-medium">Equipe</span>
        </RouterLink>
        <RouterLink
          to="/veiculos"
          @click="sidebarAberta = false"
          class="bg-blue-900 text-white rounded-full mx-2 flex items-center px-4 py-3 scale-95 transition-all"
        >
          <span class="material-symbols-outlined mr-3">ambulance</span>
          <span class="font-medium">Veículos</span>
        </RouterLink>
        <RouterLink
          to="/settings"
          @click="sidebarAberta = false"
          class="text-slate-500 hover:text-blue-900 px-4 py-3 hover:bg-slate-200/50 transition-all flex items-center mx-2 rounded-full"
        >
          <span class="material-symbols-outlined mr-3">settings</span>
          <span class="font-medium">Configurações</span>
        </RouterLink>
        <div class="flex-1"></div>
        <button
          @click="abrirModalSair"
          class="text-red-500 hover:text-red-700 px-4 py-3 hover:bg-red-50 transition-all flex items-center mx-2 rounded-full mb-4 w-full text-left cursor-pointer"
        >
          <span class="material-symbols-outlined mr-3">logout</span>
          <span class="font-medium">Sair</span>
        </button>
      </nav>
    </aside>

    <header
      class="flex justify-between items-center w-full lg:pl-72 px-4 lg:pr-8 h-20 fixed top-0 bg-white/80 backdrop-blur-md z-40 border-b border-slate-100"
    >
      <div class="flex items-center gap-3">
        <button
          class="lg:hidden p-2 rounded-lg hover:bg-slate-100 transition-colors"
          @click="sidebarAberta = !sidebarAberta"
        >
          <span class="material-symbols-outlined">menu</span>
        </button>
        <h1 class="text-2xl font-bold text-blue-900 tracking-tight">Veículos</h1>
      </div>
      <div class="flex items-center gap-3">
        <RouterLink
          to="/perfil"
          class="flex gap-3 text-black hover:text-blue-600 transition-colors rounded-full"
        >
          <p class="hidden sm:block">
            <strong>{{ usuarioNome }}</strong>
          </p>
          <span class="material-symbols-outlined">account_circle</span>
        </RouterLink>
      </div>
    </header>

    <main class="flex-1 lg:ml-64 p-8 pt-24">
      <!-- Stats -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div class="bg-white p-6 rounded-xl shadow-sm border-l-4 border-l-blue-500">
          <p class="text-sm font-medium text-slate-500 uppercase tracking-wider">Total</p>
          <p class="text-3xl font-extrabold text-slate-800">{{ totalAmbulancias }}</p>
        </div>
        <div class="bg-white p-6 rounded-xl shadow-sm border-l-4 border-l-emerald-500">
          <p class="text-sm font-medium text-slate-500 uppercase tracking-wider">Disponíveis</p>
          <p class="text-3xl font-extrabold text-emerald-600">{{ disponiveis }}</p>
        </div>
        <div class="bg-white p-6 rounded-xl shadow-sm border-l-4 border-l-blue-500">
          <p class="text-sm font-medium text-slate-500 uppercase tracking-wider">Em Uso</p>
          <p class="text-3xl font-extrabold text-blue-600">{{ emUso }}</p>
        </div>
      </div>

      <!-- Actions -->
      <div class="flex items-center justify-between mb-6">
        <h2 class="text-xl font-bold text-slate-800">Lista de Ambulâncias</h2>
        <button
          @click="mostrandoFormulario = !mostrandoFormulario"
          class="px-4 py-2 bg-blue-900 text-white rounded-full text-sm font-bold hover:bg-blue-800 transition-colors flex items-center gap-2"
        >
          <span class="material-symbols-outlined text-sm">{{ mostrandoFormulario ? 'close' : 'add' }}</span>
          {{ mostrandoFormulario ? 'Fechar' : 'Nova Ambulância' }}
        </button>
      </div>

      <!-- Formulário -->
      <div v-if="mostrandoFormulario" class="bg-white rounded-xl p-6 shadow-sm mb-6">
        <h3 class="font-bold text-indigo-950 mb-4">Cadastrar Nova Ambulância</h3>

        <div v-if="erroSalvar" class="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm">
          ⚠️ {{ erroSalvar }}
        </div>

        <form @submit.prevent="salvarAmbulancia" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <div class="flex flex-col gap-1">
            <label class="text-xs font-bold text-slate-500 uppercase">Placa *</label>
            <input
              type="text"
              v-model="novaAmbulancia.placa"
              required
              class="bg-slate-50 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-200 outline-none uppercase"
              placeholder="ABC-1234"
            />
          </div>
          <div class="flex flex-col gap-1">
            <label class="text-xs font-bold text-slate-500 uppercase">Marca *</label>
            <input
              type="text"
              v-model="novaAmbulancia.marca"
              required
              class="bg-slate-50 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
              placeholder="Ex: Volkswagen"
            />
          </div>
          <div class="flex flex-col gap-1">
            <label class="text-xs font-bold text-slate-500 uppercase">Modelo *</label>
            <input
              type="text"
              v-model="novaAmbulancia.modelo"
              required
              class="bg-slate-50 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
              placeholder="Ex: Saveiro"
            />
          </div>
          <div class="flex flex-col gap-1">
            <label class="text-xs font-bold text-slate-500 uppercase">Tipo *</label>
            <select
              v-model="novaAmbulancia.tipo"
              required
              class="bg-slate-50 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-200 outline-none cursor-pointer"
            >
              <option value="" disabled>Selecione...</option>
              <option value="suporte_basico">Suporte Básico</option>
              <option value="suporte_avancado">Suporte Avançado</option>
              <option value="uti_movel">UTI Móvel</option>
            </select>
          </div>
          <div class="md:col-span-2 lg:col-span-4 flex gap-3 mt-2">
            <button
              type="submit"
              :disabled="salvando"
              class="px-6 py-2 bg-blue-900 text-white rounded-lg text-sm font-bold hover:bg-blue-800 transition-colors disabled:opacity-60 flex items-center gap-2"
            >
              <span v-if="salvando" class="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></span>
              {{ salvando ? 'Salvando...' : 'Salvar Ambulância' }}
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
        <div v-for="i in 3" :key="i" class="bg-white p-5 rounded-xl h-24 animate-pulse"></div>
      </div>

      <!-- Empty -->
      <div
        v-else-if="!carregando && ambulancias.length === 0 && !erro"
        class="flex flex-col items-center justify-center py-16 text-slate-400"
      >
        <span class="material-symbols-outlined text-6xl mb-3">ambulance</span>
        <p class="font-medium">Nenhuma ambulância cadastrada</p>
      </div>

      <!-- Lista -->
      <div v-else class="space-y-3">
        <div
          v-for="ambulancia in ambulancias"
          :key="ambulancia.id"
          :class="['bg-white p-5 rounded-xl shadow-sm flex items-center gap-4 hover:shadow-md transition-shadow border-l-4', statusClass[ambulancia.status] as string]"
        >
          <div class="w-14 h-14 rounded-lg bg-slate-100 flex items-center justify-center shrink-0">
            <span class="material-symbols-outlined text-slate-600 text-2xl">ambulance</span>
          </div>
          <div class="flex-1 min-w-0">
            <p class="font-bold text-slate-800 text-lg">{{ ambulancia.placa }}</p>
            <p class="text-sm text-slate-500">{{ ambulancia.marca }} {{ ambulancia.modelo }}</p>
            <p class="text-xs text-slate-400 capitalize">{{ ambulancia.tipo.replace('_', ' ') }}</p>
          </div>
          <div class="text-right">
            <span
              :class="[
                'px-3 py-1 rounded-full text-xs font-bold',
                statusClass[ambulancia.status].split(' ')[0]
              ]"
            >
              {{ statusLabel[ambulancia.status] }}
            </span>
            <button
              @click="deletarAmbulancia(ambulancia.id)"
              class="block mt-2 text-red-500 hover:text-red-700 text-xs font-medium"
            >
              Excluir
            </button>
          </div>
        </div>
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