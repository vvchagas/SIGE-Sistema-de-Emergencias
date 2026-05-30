<script>
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