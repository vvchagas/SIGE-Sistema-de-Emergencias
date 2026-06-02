<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { authApi } from '@/api/index'

const isLogin = ref(true)
const router = useRouter()
const carregando = ref(false)
const erroLogin = ref<string | null>(null)
const mostrarSenha = ref(false)
const mostrarConfirmarSenha = ref(false)

// Aplica máscara DD/MM/AAAA enquanto o usuário digita
function formatarData(e: Event) {
  const alvo = e.target as HTMLInputElement
  let v = alvo.value.replace(/\D/g, '').slice(0, 8)
  if (v.length >= 5) v = `${v.slice(0, 2)}/${v.slice(2, 4)}/${v.slice(4)}`
  else if (v.length >= 3) v = `${v.slice(0, 2)}/${v.slice(2)}`
  form.nascimento = v
}

// Converte "DD/MM/AAAA" para "AAAA-MM-DD" (formato esperado pelo backend)
function converterDataParaISO(data: string): string {
  const partes = data.split('/')
  if (partes.length !== 3) return data
  const [dia, mes, ano] = partes
  return `${ano}-${mes}-${dia}`
}

const form = reactive({
  nome: '',
  email: '',
  cpf: '',
  nascimento: '',
  telefone: '',
  cargo: '',
  senha: '',
  confirmarSenha: '',
})

const handleSubmit = async () => {
  erroLogin.value = null

  if (!isLogin.value && form.senha !== form.confirmarSenha) {
    erroLogin.value = 'As senhas não conferem!'
    return
  }

  carregando.value = true

  try {
    if (isLogin.value) {
      const resposta = await authApi.login({
        email: form.email,
        senha: form.senha,
      })

      localStorage.setItem('token', resposta.token)
      localStorage.setItem('usuario_nome', resposta.nome)
      localStorage.setItem('usuario_cargo', resposta.cargo ?? '')
      router.push('/dashboard')
    } else {
      await authApi.registrar({
        nome: form.nome,
        email: form.email,
        cpf: form.cpf,
        nascimento: converterDataParaISO(form.nascimento),
        telefone: form.telefone,
        cargo: form.cargo,
        senha: form.senha,
      })

      // Após registrar, faz login automaticamente
      const resposta = await authApi.login({
        email: form.email,
        senha: form.senha,
      })

      localStorage.setItem('token', resposta.token)
      localStorage.setItem('usuario_nome', resposta.nome)
      localStorage.setItem('usuario_cargo', resposta.cargo ?? '')
      router.push('/dashboard')
    }
  } catch (e: unknown) {
    erroLogin.value = e instanceof Error ? e.message : 'Erro ao conectar com o servidor.'
  } finally {
    carregando.value = false
  }
}

const toggleMode = () => {
  isLogin.value = !isLogin.value
  form.senha = ''
  form.confirmarSenha = ''
  erroLogin.value = null
}
</script>

<template>
  <div class="login-wrapper min-h-screen flex items-center justify-center p-4">
    <div
      class="glass-effect w-full max-w-[420px] p-8 md:p-10 rounded-[28px] shadow-2xl transition-all duration-500 lg:ml-[60%]"
    >
      <h1 class="text-4xl font-bold text-[#0f1480] text-center mb-8">
        {{ isLogin ? 'Login' : 'Cadastro' }}
      </h1>

      <div
        v-if="erroLogin"
        class="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm font-medium"
      >
        ⚠️ {{ erroLogin }}
      </div>

      <form @submit.prevent="handleSubmit" class="space-y-5">
        <transition name="fade">
          <div v-if="!isLogin" class="space-y-5">
            <div>
              <input
                type="text"
                v-model="form.nome"
                placeholder="Nome Completo"
                class="w-full px-6 py-4 border-2 border-gray-100 rounded-2xl text-base outline-none focus:border-[#2027a8] focus:ring-4 focus:ring-blue-50 transition-all placeholder:text-gray-400"
                required
              />
            </div>
            <input
              type="text"
              v-model="form.cpf"
              placeholder="CPF (somente números)"
              class="w-full px-6 py-4 border-2 border-gray-100 rounded-2xl text-base outline-none focus:border-[#2027a8] focus:ring-4 focus:ring-blue-50 transition-all placeholder:text-gray-400"
            />
            <div class="px-1">
              <label class="text-xs text-gray-500 block mb-1 ml-2">Data de Nascimento</label>
              <input
                type="text"
                inputmode="numeric"
                :value="form.nascimento"
                @input="formatarData"
                placeholder="DD/MM/AAAA"
                maxlength="10"
                class="w-full px-6 py-4 border-2 border-gray-100 rounded-2xl text-base outline-none focus:border-[#2027a8] focus:ring-4 focus:ring-blue-50 transition-all placeholder:text-gray-400"
              />
             
              <select name="cargo" id="cargo" v-model="form.cargo" required class="w-full px-6 py-4 border-2 mt-3 border-gray-100 rounded-2xl text-base outline-none focus:border-[#2027a8] focus:ring-4 focus:ring-blue-50 transition-all duration-300">
                <option value="" disabled>Selecione seu cargo</option>
                <option value="paramedico">Paramédico</option>
                <option value="medico">Médico</option>
                <option value="enfermeiro">Enfermeiro</option>
                <option value="recepcao">Recepção</option>
                <option value="admin">Admin</option>
              </select>
            </div>
            <input
              type="tel"
              v-model="form.telefone"
              placeholder="Telefone (DDD + Número)"
              class="w-full px-6 py-4 border-2 border-gray-100 rounded-2xl text-base outline-none focus:border-[#2027a8] focus:ring-4 focus:ring-blue-50 transition-all placeholder:text-gray-400"
            />
          </div>
        </transition>

        <div>
          <input
            type="email"
            v-model="form.email"
            placeholder="E-mail"
            class="w-full px-6 py-4 border-2 border-gray-100 rounded-2xl text-base outline-none focus:border-[#2027a8] focus:ring-4 focus:ring-blue-50 transition-all placeholder:text-gray-400"
            required
          />
        </div>

        <div class="relative">
          <input
            :type="mostrarSenha ? 'text' : 'password'"
            v-model="form.senha"
            placeholder="Senha"
            class="w-full px-6 py-4 pr-14 border-2 border-gray-100 rounded-2xl text-base outline-none focus:border-[#2027a8] focus:ring-4 focus:ring-blue-50 transition-all placeholder:text-gray-400"
            required
          />
          <button
            type="button"
            @click="mostrarSenha = !mostrarSenha"
            class="absolute right-5 top-1/2 -translate-y-1/2 text-gray-400 hover:text-[#2027a8] transition-colors"
            :aria-label="mostrarSenha ? 'Ocultar senha' : 'Mostrar senha'"
          >
            <span class="material-symbols-outlined">{{ mostrarSenha ? 'visibility_off' : 'visibility' }}</span>
          </button>
        </div>

        <div v-if="!isLogin" class="relative">
          <input
            :type="mostrarConfirmarSenha ? 'text' : 'password'"
            v-model="form.confirmarSenha"
            placeholder="Confirmar Senha"
            class="w-full px-6 py-4 pr-14 border-2 border-gray-100 rounded-2xl text-base outline-none focus:border-[#2027a8] focus:ring-4 focus:ring-blue-50 transition-all placeholder:text-gray-400"
            required
          />
          <button
            type="button"
            @click="mostrarConfirmarSenha = !mostrarConfirmarSenha"
            class="absolute right-5 top-1/2 -translate-y-1/2 text-gray-400 hover:text-[#2027a8] transition-colors"
            :aria-label="mostrarConfirmarSenha ? 'Ocultar senha' : 'Mostrar senha'"
          >
            <span class="material-symbols-outlined">{{ mostrarConfirmarSenha ? 'visibility_off' : 'visibility' }}</span>
          </button>
        </div>

        <button
          type="submit"
          :disabled="carregando"
          class="w-full py-4 bg-[#2027a8] hover:bg-[#0f1480] text-white rounded-2xl text-lg font-semibold transition-all transform hover:-translate-y-1 active:scale-95 shadow-lg mt-2 disabled:opacity-60 disabled:cursor-not-allowed flex items-center justify-center gap-2"
        >
          <span
            v-if="carregando"
            class="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin"
          ></span>
          {{ carregando ? 'Aguarde...' : isLogin ? 'Entrar' : 'Criar conta' }}
        </button>

        <div class="text-center mt-6 text-gray-600">
          {{ isLogin ? 'Não tem cadastro?' : 'Já possui conta?' }}
          <a href="#" @click.prevent="toggleMode" class="text-[#2027a8] font-bold hover:underline">
            {{ isLogin ? 'Cadastrar-se' : 'Fazer login' }}
          </a>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
.login-wrapper {
  background-image: url('@/assets/img/image.png');
  background-size: cover;
  background-position: left;
  background-repeat: no-repeat;
  background-attachment: fixed;
  width: 100%;
  height: 100vh;
}
/* tela pequena celular tablet */
@media (min-width: 640px) {
  .login-wrapper {
    background-size: 750px;
  }
}
/* tela media responsivo tablets notebooks  */
@media (min-width: 768px) {
  .login-wrapper {
    background-size: 850px;
  }
}
/* tela grande responsivo */
@media (min-width: 1024px) {
  .login-wrapper {
    background-size: 950px;
  }
}

.glass-effect {
  background: rgba(255, 255, 255, 0.92);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.3);
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}
.fade-enter,
.fade-leave-to {
  opacity: 0;
}
</style>