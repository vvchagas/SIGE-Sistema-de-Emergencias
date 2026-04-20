# 🚑 SIGE - Sistema de Gestão de Emergências (SAMU 192)

## 📖 Sobre o Projeto
O **SIGE** é uma plataforma digital desenvolvida para gerenciar o fluxo completo de atendimento de emergências pré-hospitalares (SAMU 192). Construído por uma equipe de 6 desenvolvedores, o sistema atua desde o recebimento da chamada telefônica até a finalização do atendimento pela equipe médica, garantindo agilidade no despacho de ambulâncias e organização das informações.

---

## ⚠️ O Problema
A gestão de chamados de emergência exige precisão e velocidade. Fluxos manuais ou sistemas desintegrados causam:
- **Lentidão no Despacho:** Dificuldade em cruzar rapidamente a gravidade da ocorrência com a disponibilidade da frota.
- **Falha de Comunicação:** Perda de informações críticas repassadas via rádio entre o operador e a ambulância.
- **Falta de Rastreabilidade:** Dificuldade em saber o status exato de cada viatura (se está em atendimento, a caminho do hospital ou disponível).

## 💡 A Solução
O SIGE centraliza a operação do SAMU 192 em um *Dashboard* unificado, focado na figura do **Operador**. O sistema oferece:
1. **Triagem por Prioridade:** Classificação visual imediata dos chamados (Crítica, Alta, Média e Baixa).
2. **Gestão de Frota e Paramédicos:** Controle em tempo real de quais profissionais e viaturas estão disponíveis para despacho.
3. **Controle de Status (Máquina de Estados):** Transições claras do chamado (`Aberto` ➔ `Aguardando Despacho` ➔ `Em Atendimento` ➔ `Encerrado`).

---

## 🔄 Fluxo de Operação (Core Business)

1. **Recebimento:** Operador atende o 192 e registra os dados do solicitante no SIGE (gerando um ID único).
2. **Classificação:** Definição da prioridade clínica da ocorrência.
3. **Seleção e Despacho:** O sistema filtra as ambulâncias com status `Disponível`. O operador associa a viatura ao chamado.
4. **Comunicação e Atualização:** A equipe de rua se comunica via rádio/telefone com o operador, que atualiza manualmente o SIGE caso haja necessidade de transporte para um hospital.
5. **Finalização:** Ao concluir a ocorrência, a ambulância retorna ao status `Disponível` e o chamado é `Encerrado`.

---

## 🛠️ Tecnologias Utilizadas

O projeto adota uma arquitetura separada, garantindo alta performance e uma interface moderna e responsiva.

### Frontend
- **Framework:** Vue.js
- **Estilização:** Tailwind CSS (UI limpa, baseada em componentes)
- **Ferramentas:** Visual Studio Code

### Backend
- **Linguagem/Framework:** C# / .NET
- **Ferramentas:** Visual Studio Community

### Versionamento e Colaboração
- **Controle de Versão:** Git / GitHub
- **Metodologia:** Branch-based workflow com `.gitignore` rigoroso para arquivos de IDE.

---

## ⚙️ Como executar o projeto

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/vvchagas/SIGE-Sistema-de-Atendimento-Urgentes.git