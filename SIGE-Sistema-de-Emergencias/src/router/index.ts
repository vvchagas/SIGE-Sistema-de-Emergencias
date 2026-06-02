import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import DashboardView from '../views/DashboardView.vue'
import ChamadoView from '../views/ChamadoView.vue'
import PerfilView from '../views/PerfilView.vue'
import EquipeView from '../views/EquipeView.vue'
import SettingsView from '../views/SettingsView.vue'
import VeiculosView from '../views/VeiculosView.vue'


const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/login',
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: { requiresAuth: false },
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: DashboardView,
      meta: { requiresAuth: true },
    },
    {
      path: '/chamado',
      name: 'chamado',
      component: ChamadoView,
      meta: { requiresAuth: true },
    },
    {
      path: '/perfil',
      name: 'perfil',
      component: PerfilView,
      meta: { requiresAuth: true },
    },
    {
      path: '/equipe',
      name: 'equipe',
      component: EquipeView,
      meta: { requiresAuth: true },
    },
    {
      path: '/veiculos',
      name: 'veiculos',
      component: VeiculosView,
      meta: { requiresAuth: true },
    },
    {
      path: '/settings',
      name: 'settings',
      component: SettingsView,
      meta: { requiresAuth: true },
    },
  ],
})

// Guarda de navegação - verifica autenticação
router.beforeEach((to, _from, next) => {
  const token = localStorage.getItem('token')
  const requiresAuth = to.meta.requiresAuth

  if (requiresAuth && !token) {
    // Redireciona para login se não autenticado
    next('/login')
  } else if (to.path === '/login' && token) {
    // Redireciona para dashboard se já autenticado
    next('/dashboard')
  } else {
    next()
  }
})

export default router