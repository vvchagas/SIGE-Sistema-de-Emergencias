<script setup lang="ts">
import { reactive, ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { chamadosApi, ambulanciasApi, type AmbulanciaGetDTO, type ChamadoRequestDto } from '../api/index'

const usuarioNome = localStorage.getItem('usuario_nome') || 'Usuário'

const form = reactive<ChamadoRequestDto>({
  nomeSolicitante: '',
  telefoneSolicitante: '',
  tipoEmergencia: '',
  descricao: '',
  statusPaciente: -1,
  nivelPrioridade: -1,
  ambulanciaId: '',
  paramedicos: [],
  logradouro: '',
  numero: 0,
  bairro: '',
  cidade: '',
  pontoReferencia: '',
})

const router = useRouter()
const sidebarAberta = ref(false)
const mostrarModalSair = ref(false)
const enviando = ref(false)
const erro = ref<string | null>(null)
const ambulancias = ref<AmbulanciaGetDTO[]>([])
const carregandoAmbulancias = ref(true)

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

function selecionarConsciencia(valor: number) {
  form.statusPaciente = valor
}

function selecionarPrioridade(valor: number) {
  form.nivelPrioridade = valor
}

async function carregarAmbulancias() {
  carregandoAmbulancias.value = true
  try {
    const todas = await ambulanciasApi.listar()
    ambulancias.value = todas
  } catch {
    ambulancias.value = []
  } finally {
    carregandoAmbulancias.value = false
  }
}

async function despacharChamado() {
  erro.value = null

  if (
    !form.nomeSolicitante ||
    !form.telefoneSolicitante ||
    !form.logradouro ||
    !form.bairro ||
    !form.cidade ||
    !form.tipoEmergencia ||
    !form.descricao ||
    !form.ambulanciaId ||
    form.statusPaciente === -1 ||
    form.nivelPrioridade === -1
  ) {
    erro.value =
      'Preencha todos os campos obrigatórios, incluindo estado de consciência, nível de prioridade e ambulância.'
    return
  }

  enviando.value = true
  try {
    await chamadosApi.criar({
      ...form,
      numero: parseInt(String(form.numero)) || 0,
    })
    router.push('/dashboard')
  } catch (e: unknown) {
    erro.value = e instanceof Error ? e.message : 'Erro ao criar chamado.'
  } finally {
    enviando.value = false
  }
}

onMounted(carregarAmbulancias)
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
          <span class="material-symbols-outlined mr-3">dashboard</span>
          <span class="font-medium">Dashboard</span>
        </RouterLink>
        <RouterLink
          to="/chamado"
          @click="sidebarAberta = false"
          class="bg-blue-900 dark:bg-blue-600 text-white rounded-full mx-2 flex items-center px-4 py-3 scale-95 active:scale-90 duration-150 transition-all"
        >
          <span class="material-symbols-outlined mr-3">pending_actions</span>
          <span class="font-medium">Chamados</span>
        </RouterLink>
        <RouterLink
          to="/equipe"
          @click="sidebarAberta = false"
          class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full"
        >
          <span class="material-symbols-outlined mr-3">groups_2</span>
          <span class="font-medium">Equipe</span>
        </RouterLink>
        <RouterLink
          to="/veiculos"
          @click="sidebarAberta = false"
          class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full"
        >
          <span class="material-symbols-outlined mr-3">ambulance</span>
          <span class="font-medium">Veículos</span>
        </RouterLink>
        <RouterLink
          to="/settings"
          @click="sidebarAberta = false"
          class="text-slate-500 dark:text-slate-400 hover:text-blue-900 dark:hover:text-blue-200 px-4 py-3 hover:bg-slate-200/50 dark:hover:bg-slate-800 transition-all flex items-center mx-2 rounded-full"
        >
          <span class="material-symbols-outlined mr-3">settings</span>
          <span class="font-medium">Configurações</span>
        </RouterLink>
        <div class="flex-1"></div>
        <button
          @click="abrirModalSair"
          class="text-red-500 hover:text-red-700 px-4 py-3 hover:bg-red-50 dark:hover:bg-red-900/20 transition-all flex items-center mx-2 rounded-full mb-4 w-full text-left cursor-pointer"
        >
          <span class="material-symbols-outlined mr-3">logout</span>
          <span class="font-medium">Sair</span>
        </button>
      </nav>
    </aside>

    <main class="flex-1 flex flex-col overflow-y-auto lg:ml-64">
      <header class="bg-white border-b flex items-center justify-between px-4 lg:px-6 min-h-[80px]">
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
            Chamados
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
              <span class="material-symbols-outlined" data-icon="account_circle"
                >account_circle</span
              >
            </RouterLink>
          </div>
        </div>
      </header>

      <div class="p-8 max-w-[1400px] w-full mx-auto">
        <div class="mb-6">
          <h2 class="text-2xl font-extrabold text-indigo-950">Novo Chamado</h2>
          <p class="text-sm text-slate-500">
            Preencha os dados com precisão para despacho imediato.
          </p>
          <p class="text-sm text-red-500 font-bold mt-1">(*) Campos obrigatórios</p>
        </div>

        <!-- Erro geral -->
        <div
          v-if="erro"
          class="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm font-medium"
        >
          ⚠️ {{ erro }}
        </div>

        <form
          @submit.prevent="despacharChamado"
          class="grid grid-cols-1 lg:grid-cols-[1fr_340px] gap-6"
        >
          <div class="flex flex-col gap-6">
            <!-- Identificação do Solicitante -->
            <div class="bg-white rounded-xl p-6 shadow-sm">
              <h3 class="font-bold text-indigo-950 mb-5">Identificação do Solicitante</h3>
              <div class="flex gap-4">
                <div class="flex flex-col gap-2 w-1/2">
                  <label class="text-[11px] font-bold text-slate-500 uppercase"
                    >Nome do solicitante <span class="text-red-400">*</span></label
                  >
                  <input
                    type="text"
                    v-model="form.nomeSolicitante"
                    placeholder="Nome completo"
                    class="bg-slate-100 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
                    required
                  />
                </div>
                <div class="flex flex-col gap-2 w-1/2">
                  <label class="text-[11px] font-bold text-slate-500 uppercase"
                    >Telefone <span class="text-red-400">*</span></label
                  >
                  <input
                    type="text"
                    v-model="form.telefoneSolicitante"
                    placeholder="(11) 99999-9999"
                    class="bg-slate-100 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-200 outline-none"
                    required
                  />
                </div>
              </div>
            </div>

            <!-- Localização -->
            <div class="bg-white rounded-xl p-6 shadow-sm">
              <h3 class="font-bold text-indigo-950 mb-5">Localização da Ocorrência</h3>
              <div class="flex flex-col gap-4">
                <div class="flex gap-4">
                  <div class="flex flex-col gap-2 flex-1">
                    <label class="text-[11px] font-bold text-slate-500 uppercase tracking-wide"
                      >Endereço (Rua/Av.) <span class="text-red-400">*</span></label
                    >
                    <input
                      type="text"
                      v-model="form.logradouro"
                      placeholder="Rua, Avenida..."
                      class="bg-slate-50 rounded-lg px-3 py-3 text-sm focus:ring-2 focus:ring-blue-200 outline-none w-full"
                      required
                    />
                  </div>
                  <div class="flex flex-col gap-2 w-24">
                    <label class="text-[11px] font-bold text-slate-500 uppercase tracking-wide"
                      >Número <span class="text-red-400">*</span></label
                    >
                    <input
                      type="text"
                      v-model="form.numero"
                      placeholder="Nº"
                      class="bg-slate-50 rounded-lg px-3 py-3 text-sm focus:ring-2 focus:ring-blue-200 outline-none w-full"
                    />
                  </div>
                </div>
                <div class="flex gap-4">
                  <div class="flex flex-col gap-2 w-1/2">
                    <label class="text-[11px] font-bold text-slate-500 uppercase tracking-wide"
                      >Bairro <span class="text-red-400">*</span></label
                    >
                    <input
                      type="text"
                      v-model="form.bairro"
                      placeholder="Bairro"
                      class="bg-slate-50 rounded-lg px-3 py-3 text-sm focus:ring-2 focus:ring-blue-200 outline-none w-full"
                      required
                    />
                  </div>
                  <div class="flex flex-col gap-2 w-1/2">
                    <label class="text-[11px] font-bold text-slate-500 uppercase tracking-wide"
                      >Cidade <span class="text-red-400">*</span></label
                    >
                    <input
                      type="text"
                      v-model="form.cidade"
                      placeholder="Cidade"
                      class="bg-slate-50 rounded-lg px-3 py-3 text-sm focus:ring-2 focus:ring-blue-200 outline-none w-full"
                      required
                    />
                  </div>
                </div>
                <div class="flex flex-col gap-2">
                  <label class="text-[11px] font-bold text-slate-500 uppercase tracking-wide"
                    >Ponto de Referência (opcional)</label
                  >
                  <input
                    type="text"
                    v-model="form.pontoReferencia"
                    placeholder="Próximo a..."
                    class="bg-slate-50 rounded-lg px-3 py-3 text-sm focus:ring-2 focus:ring-blue-200 outline-none w-full"
                  />
                </div>
              </div>
            </div>

            <!-- Detalhes Clínicos -->
            <div class="bg-white rounded-xl p-6 shadow-sm">
              <h3 class="font-bold text-indigo-950 mb-5">Detalhes Clínicos</h3>
              <div class="flex flex-col gap-6">
                <div class="flex gap-4">
                  <div class="flex flex-col gap-2 w-1/2">
                    <label class="text-[11px] font-bold text-slate-500 uppercase tracking-wide"
                      >Natureza da Emergência <span class="text-red-400">*</span></label
                    >
                    <select
                      v-model="form.tipoEmergencia"
                      required
                      class="appearance-none bg-slate-50 rounded-lg px-4 py-3 text-sm focus:ring-2 focus:ring-blue-200 outline-none w-full text-slate-700 cursor-pointer"
                    >
                      <option value="" disabled selected>Selecione...</option>
                      <option value="trauma">Trauma</option>
                      <option value="clinico">Clínico</option>
                      <option value="psiquiatrico">Psiquiátrico</option>
                      <option value="obstetrico">Obstétrico</option>
                    </select>
                  </div>
                  <div class="flex flex-col gap-2 w-1/2">
                    <label class="text-[11px] font-bold text-slate-500 uppercase tracking-wide"
                      >Estado de Consciência <span class="text-red-400">*</span></label
                    >
                    <div class="flex gap-2 h-11">
                      <button
                        v-for="(label, idx) in ['Consciente', 'Confuso', 'Inconsciente']"
                        :key="idx"
                        type="button"
                        @click="selecionarConsciencia(idx)"
                        :class="[
                          'flex-1 border font-bold text-[12px] rounded-lg transition-all cursor-pointer',
                          form.statusPaciente === idx
                            ? 'bg-blue-900 text-white border-blue-900'
                            : 'bg-white border-slate-200 text-slate-700 hover:border-blue-200 hover:bg-blue-50',
                        ]"
                      >
                        {{ label }}
                      </button>
                    </div>
                  </div>
                </div>
                <div class="flex flex-col gap-2">
                  <label class="text-[11px] font-bold text-slate-500 uppercase tracking-wide"
                    >Descrição do paciente e sintomas <span class="text-red-400">*</span></label
                  >
                  <textarea
                    v-model="form.descricao"
                    rows="3"
                    placeholder="Descreva os sintomas e situação do paciente..."
                    class="bg-slate-50 rounded-lg px-4 py-3 text-[14px] focus:ring-2 focus:ring-blue-200 outline-none w-full resize-none"
                    required
                  ></textarea>
                </div>
              </div>
            </div>
          </div>

          <!-- Painel Direito -->
          <div>
            <div class="bg-white rounded-xl p-6 shadow-sm flex flex-col gap-5">
              <h3 class="font-bold text-indigo-950">Triagem e Prioridade</h3>

              <div class="flex flex-col gap-3">
                <div
                  v-for="(p, idx) in [
                    { label: 'Nível 1 - Emergência', sub: 'Risco imediato de vida', cor: 'red' },
                    { label: 'Nível 2 - Urgência', sub: 'Potencial risco de vida', cor: 'amber' },
                    { label: 'Nível 3 - Pouco Urgente', sub: 'Estável, aguardo tolerável', cor: 'emerald' },
                  ]"
                  :key="idx"
                  @click="selecionarPrioridade(2 - idx)"
                  :class="[
                    'flex items-center gap-4 p-4 rounded-xl cursor-pointer border transition-all',
                    form.nivelPrioridade === 2 - idx
                      ? 'border-blue-500 bg-blue-50'
                      : 'border-transparent bg-slate-50 hover:border-slate-200',
                  ]"
                >
                  <div
                    :class="`w-3.5 h-3.5 rounded-full bg-${p.cor}-500 shrink-0`"
                  ></div>
                  <div class="flex flex-col mt-0.5">
                    <h4 class="font-bold text-sm text-slate-800">
                      {{ p.label }}
                    </h4>
                    <p class="text-[11px] font-medium text-slate-500">
                      {{ p.sub }}
                    </p>
                  </div>
                </div>
              </div>

              <div class="flex flex-col gap-2">
                <label class="text-[11px] font-bold text-slate-500 uppercase tracking-wide"
                  >Ambulância <span class="text-red-400">*</span></label
                >
                <select
                  v-model="form.ambulanciaId"
                  required
                  class="appearance-none bg-slate-50 rounded-lg px-4 py-3 text-sm focus:ring-2 focus:ring-blue-200 outline-none w-full text-slate-700 cursor-pointer border border-slate-200"
                >
                  <option value="" disabled selected>
                    {{ carregandoAmbulancias ? 'Carregando...' : 'Selecione a ambulância' }}
                  </option>
                  <option v-for="amb in ambulancias" :key="amb.id" :value="amb.id">
                    {{ amb.placa }} — {{ amb.marca }} {{ amb.modelo }} ({{ amb.tipo }})
                  </option>
                </select>
                <p
                  v-if="!carregandoAmbulancias && ambulancias.length === 0"
                  class="text-xs text-red-500"
                >
                  Nenhuma ambulância disponível.
                </p>
              </div>

              <button
                type="submit"
                :disabled="enviando"
                class="w-full bg-indigo-950 text-white rounded-full py-4 font-bold flex items-center justify-center gap-3 hover:bg-indigo-900 transition-colors disabled:opacity-60"
              >
                <span
                  v-if="enviando"
                  class="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin"
                ></span>
                <span v-else class="material-symbols-outlined">directions_car</span>
                {{ enviando ? 'Despachando...' : 'Despachar Ambulância' }}
              </button>
              <RouterLink to="/dashboard">
                <button
                  type="button"
                  class="w-full bg-white text-slate-500 border border-slate-200 rounded-full py-4 font-bold flex items-center justify-center gap-3 hover:bg-slate-50 hover:text-slate-700 transition-colors"
                >
                  Cancelar Registro
                </button>
              </RouterLink>
            </div>
          </div>
        </form>
      </div>
    </main>
  </div>

  <!-- Modal de saída -->
  <Teleport to="body">
    <div
      v-if="mostrarModalSair"
      class="fixed inset-0 bg-black/50 z-[100] flex items-center justify-center p-4"
    >
      <div class="bg-white rounded-2xl shadow-2xl p-8 max-w-sm w-full">
        <div class="flex flex-col items-center text-center">
          <span class="material-symbols-outlined text-red-500 text-6xl mb-4">logout</span>
          <h2 class="text-xl font-bold text-slate-800 mb-2">Sair do sistema?</h2>
          <p class="text-slate-500 mb-6">Tem certeza que deseja sair?</p>
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