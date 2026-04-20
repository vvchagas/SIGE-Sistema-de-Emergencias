<script setup lang="ts">
import { ref, reactive } from 'vue';
import { useRouter } from 'vue-router';

// Controle de estado da tela
const isLogin = ref(true);
const router = useRouter();

// Estrutura de dados única para o formulário
const form = reactive({
  nome: "",
  email: "",
  cpf: "",
  nascimento: "",
  telefone: "",
  senha: "",
  confirmarSenha: "",
});

const handleSubmit = () => {
  // Validação básica de senha no cadastro
  if (!isLogin.value && form.senha !== form.confirmarSenha) {
    alert("As senhas não conferem!");
    return;
  }

  // Ao invés do alert, redirecionamos para o dashboard
  console.log("Dados enviados:", { ...form });
  router.push('/dashboard');
};

// Alternar entre Login e Cadastro limpando campos sensíveis
const toggleMode = () => {
  isLogin.value = !isLogin.value;
  form.senha = "";
  form.confirmarSenha = "";
};
</script>

<template>
  <div class="login-wrapper min-h-screen flex items-center justify-center p-4">
    <div class="glass-effect w-full max-w-[420px] p-8 md:p-10 rounded-[28px] shadow-2xl transition-all duration-500 lg:ml-[60%]">

      <h1 class="text-4xl font-bold text-[#0f1480] text-center mb-8">
        {{ isLogin ? 'Login' : 'Cadastro' }}
      </h1>

      <form @submit.prevent="handleSubmit" class="space-y-5">

        <div>
          <input
            type="text"
            v-model="form.nome"
            placeholder="Nome Completo"
            class="w-full px-6 py-4 border-2 border-gray-100 rounded-2xl text-base outline-none focus:border-[#2027a8] focus:ring-4 focus:ring-blue-50 transition-all placeholder:text-gray-400;"
            required
          />
        </div>

        <transition name="fade">
          <div v-if="!isLogin" class="space-y-4">
            <input
              type="email"
              v-model="form.email"
              placeholder="E-mail"
              class="w-full px-6 py-4 border-2 border-gray-100 rounded-2xl text-base outline-none focus:border-[#2027a8] focus:ring-4 focus:ring-blue-50 transition-all placeholder:text-gray-400;"
              required
            />

            <input
              type="text"
              v-model="form.cpf"
              placeholder="CPF (somente números)"
              class="w-full px-6 py-4 border-2 border-gray-100 rounded-2xl text-base outline-none focus:border-[#2027a8] focus:ring-4 focus:ring-blue-50 transition-all placeholder:text-gray-400;"
              required
            />

            <div class="px-1">
              <label class="text-xs text-gray-500 block mb-1 ml-2">Data de Nascimento</label>
              <input
                type="date"
                v-model="form.nascimento"
                class="w-full px-6 py-4 border-2 border-gray-100 rounded-2xl text-base outline-none focus:border-[#2027a8] focus:ring-4 focus:ring-blue-50 transition-all placeholder:text-gray-400;"
                required
              />
            </div>

            <input
              type="tel"
              v-model="form.telefone"
              placeholder="Telefone (DDD + Número)"
              class="w-full px-6 py-4 border-2 border-gray-100 rounded-2xl text-base outline-none focus:border-[#2027a8] focus:ring-4 focus:ring-blue-50 transition-all placeholder:text-gray-400;"
              required
            />
          </div>
        </transition>

        <div>
          <input
            type="password"
            v-model="form.senha"
            placeholder="Senha"
            class="w-full px-6 py-4 border-2 border-gray-100 rounded-2xl text-base outline-none focus:border-[#2027a8] focus:ring-4 focus:ring-blue-50 transition-all placeholder:text-gray-400;"
            required
          />
        </div>

        <div v-if="!isLogin">
          <input
            type="password"
            v-model="form.confirmarSenha"
            placeholder="Confirmar Senha"
            class="w-full px-6 py-4 border-2 border-gray-100 rounded-2xl text-base outline-none focus:border-[#2027a8] focus:ring-4 focus:ring-blue-50 transition-all placeholder:text-gray-400;"
            required
          />
        </div>

        <button
          type="submit"
          class="w-full py-4 bg-[#2027a8] hover:bg-[#0f1480] text-white rounded-2xl text-lg font-semibold transition-all transform hover:-translate-y-1 active:scale-95 shadow-lg mt-2"
        >
          {{ isLogin ? 'Entrar' : 'Criar conta' }}
        </button>

        <div class="text-center mt-6 text-gray-600">
          {{ isLogin ? 'Não tem cadastro?' : 'Já possui conta?' }}
          <a
            href="#"
            @click.prevent="toggleMode"
            class="text-[#2027a8] font-bold hover:underline"
          >
            {{ isLogin ? 'Cadastrar-se' : 'Fazer login' }}
          </a>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
/* Background conforme sua imagem de luzes */
.login-wrapper {
  background-image: url("@/assets/img/image.png");
  background-size: 818px;
  background-position: left;
  background-repeat: no-repeat;
  background-attachment: fixed;
}

/* Efeito de Vidro */
.glass-effect {
  background: rgba(255, 255, 255, 0.92);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.3);
}


/* Animação suave ao trocar de tela */
.fade-enter-active, .fade-leave-active {
  transition: opacity 0.3s ease;
}
.fade-enter, .fade-leave-to {
  opacity: 0;
}
</style>
