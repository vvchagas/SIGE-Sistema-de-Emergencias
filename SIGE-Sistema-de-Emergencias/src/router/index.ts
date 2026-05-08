import { createRouter, createWebHistory } from 'vue-router'
import DashboardView from '../views/DashboardView.vue'
import LoginView from '../views/LoginView.vue'
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
      redirect: '/login'
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView,
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: DashboardView,
    },
    {
      path: '/chamado',
      name: 'chamado',
      component: ChamadoView,
    },
    {
      path: '/perfil',
      name: 'perfil',
      component: PerfilView,
    },
    {
      path: '/equipe',
      name: 'equipe',
      component: EquipeView,
    },
    {
      path: '/veiculos',
      name: 'veiculos',
      component: VeiculosView,
    },
    {
      path: '/settings',
      name: 'settings',
      component: SettingsView,
    },
  ],
})

export default router
