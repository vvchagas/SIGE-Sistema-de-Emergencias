# SIGE - Sistema de Gestão de Emergências (SAMU 192)

Projeto acadêmico em grupo (UNIMAR). Frontend Vue 3 + TypeScript + Tailwind + Vite.
Backend separado em C#/.NET no diretório irmão `../Backend/` (não mexer salvo se pedido explicitamente).

## Stack do frontend
- Vue 3 com `<script setup lang="ts">` (Composition API) — manter esse padrão SEMPRE
- TypeScript estrito
- Tailwind CSS + Material Symbols Outlined (ícones)
- Vue Router (history mode)
- Fetch nativo via wrapper em `src/api/index.ts` (NÃO usar axios direto, mesmo que esteja em package.json)

## Como rodar
- `npm run dev` → sobe Vite em http://localhost:5173/
- `npm run build` → build de produção
- `npm run type-check` → checagem de tipos
- `npm run lint` → roda oxlint + eslint
- `npm run format` → roda prettier no src/

## Padrões visuais (seguir à risca)
- Cor primária: `blue-900` (títulos, marca)
- Fundo: `bg-slate-100`, texto `text-slate-800`
- Sidebar fixa de 256px (w-64), retrátil no mobile com `lg:translate-x-0`
- Header com botão hamburguer (mobile) + nome do usuário linkado ao /perfil
- Ícones: `<span class="material-symbols-outlined">nome_do_icone</span>`
- Cards/botões: `rounded-xl` ou `rounded-2xl`
- Estados: usar `carregando: Ref<boolean>` e `erro: Ref<string | null>` como nas views existentes

## Autenticação
- Token JWT salvo em `localStorage`
- Chaves usadas: `token`, `usuario_nome`, `usuario_cargo`, `usuario_id`
- API base: `https://localhost:7057`
- Endpoint `/auth/me` retorna: { nome, email, cpf, telefone, cargo }
- Header `Authorization: Bearer {token}` é adicionado automaticamente pelo wrapper

## Endpoints do backend já existentes
- `POST /auth/login`, `POST /auth/registrar`, `GET /auth/me`
- `GET/POST/PATCH /chamados`, `GET /chamados/{id}`
- `GET/POST/PATCH/DELETE /paramedicos`
- `GET/POST/PATCH/DELETE /ambulancias`

## Estado atual do front
Views prontas e funcionais (usar como referência de estilo):
- LoginView, DashboardView, ChamadoView, EquipeView, VeiculosView

Views como placeholder "Em manutenção" (PRECISAM SER CONSTRUÍDAS):
- **`src/views/PerfilView.vue`** (rota `/perfil`)
- **`src/views/SettingsView.vue`** (rota `/settings`)

## Convenções obrigatórias
- Sempre Composition API com `<script setup lang="ts">` — NUNCA Options API
- Reutilizar o padrão de ModalSair e layout de sidebar das views existentes
- Comentários e textos de UI em português
- Tratamento de erros: try/catch + estado `erro` exibido pro usuário
- Loading: estado `carregando` com skeleton ou spinner durante chamadas de API

## O que NÃO fazer
- Não trocar Tailwind por Bootstrap (mesmo que `bootstrap` apareça em deps antigas)
- Não usar axios direto (usar o wrapper em `src/api/index.ts`)
- Não criar pasta `components/` nova sem confirmar antes
- Não rodar `npm audit fix` (pode quebrar deps)