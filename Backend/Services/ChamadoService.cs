using SeuProjeto.Services.Context;
using SIGEApi.DTOs.ChamadoDtos;
using SIGEApi.Enums;
using SIGEApi.Models;
using SIGEApi.Repositories;

namespace SIGEApi.Services
{
    public class ChamadoService
    {
        private readonly ChamadoRepository _repositoryChamado;
        private readonly AmbulanciaRepository _repositoryAmbulancia;
        private readonly ParamedicoRepository _repositoryParamedico;
        private readonly UserService _userService;
        private readonly ParamedicoService _paramedicoService;
        private readonly AmbulanciaService _ambulanciaService;
        public ChamadoService(ChamadoRepository repositoryChamado, AmbulanciaRepository repositoryAmbulancia, ParamedicoRepository repositoryParamedico, UserService userService, ParamedicoService paramedicoService, AmbulanciaService ambulanciaService)
        {
            _repositoryChamado = repositoryChamado;
            _repositoryAmbulancia = repositoryAmbulancia;
            _repositoryParamedico = repositoryParamedico;
            _userService = userService;
            _paramedicoService = paramedicoService;
            _ambulanciaService = ambulanciaService;
        }
        public async Task<List<Chamado>> Listar(int pagina, int quantidade)
        {
            int pular = (pagina - 1) * quantidade;
            var chamado = await _repositoryChamado.ListarChamados(pular, quantidade);
            return chamado;
        }
        public async Task<Chamado> ListarChamadoById(Guid id)
        {
            var chamado = await _repositoryChamado.BuscarChamadoById(id);
            if (chamado == null)
            {
                throw new ArgumentException("Ambulância não encontrada!");
            }
            return chamado;
        }
        public async Task<Chamado> CreateChamado(ChamadoRequestDto chamadoRequest)
        {
            if (chamadoRequest.Numero <= 0)
                throw new ArgumentException("Número de casa inválido");

            Ambulancia? ambulanciaChamado = await _repositoryAmbulancia.GetAmbulanciaById(chamadoRequest.AmbulanciaId);

            if (ambulanciaChamado == null)
                throw new ArgumentException("Ambulância não encontrada");

            List<Paramedico> paramedicos = new List<Paramedico>();
            foreach (Guid id in chamadoRequest.Paramedicos)
            {
                Paramedico? paramedico = await _repositoryParamedico.GetParamedicoById(id);
                if (paramedico == null)
                    throw new ArgumentException($"Paramédico com ID {id} não encontrado");

                var rastreado = await _paramedicoService.AtualizarDisponibilidade(id, true);
                paramedicos.Add(rastreado);
            }

            var user = await _userService.GetUserAsync();

            Chamado novoChamado = new Chamado()
            {
                NomeSolicitante = chamadoRequest.NomeSolicitante,
                TelefoneSolicitante = chamadoRequest.TelefoneSolicitante,
                TipoEmergencia = chamadoRequest.TipoEmergencia,
                Descricao = chamadoRequest.Descricao,
                StatusPaciente = chamadoRequest.StatusPaciente,
                StatusChamado = StatusChamado.Aguardando,
                NivelPrioridade = chamadoRequest.NivelPrioridade,
                DataAbertura = DateTime.UtcNow,
                UserId = user.Id,
                Ambulancia = null,
                AmbulanciaId = ambulanciaChamado.Id,
                Paramedicos = paramedicos,
                Logradouro = chamadoRequest.Logradouro,
                Numero = chamadoRequest.Numero,
                Cidade = chamadoRequest.Cidade,
                Bairro = chamadoRequest.Bairro,
                PontoReferencia = chamadoRequest.PontoReferencia
            };

            return await _repositoryChamado.SalvarChamado(novoChamado);
        }
        public async Task<Chamado> AtualizarChamado(Guid id, ChamadoAtualizarDto chamadoRequest)
        {
            Chamado chamado = await _repositoryChamado.BuscarChamadoById(id);

            if (chamado == null) { throw new ArgumentException("Chamado não encontrado!"); }

            chamado.NomeSolicitante = chamadoRequest.NomeSolicitante;
            chamado.TelefoneSolicitante = chamadoRequest.TelefoneSolicitante;
            chamado.TipoEmergencia = chamadoRequest.TipoEmergencia;
            chamado.Descricao = chamadoRequest.Descricao;
            chamado.StatusPaciente = chamadoRequest.StatusPaciente;
            chamado.NivelPrioridade = chamadoRequest.NivelPrioridade;
            chamado.Logradouro = chamadoRequest.Logradouro;
            chamado.Numero = chamadoRequest.Numero;
            chamado.Cidade = chamadoRequest.Cidade;
            chamado.Bairro = chamadoRequest.Bairro;
            chamado.PontoReferencia = chamadoRequest.PontoReferencia;

            return await _repositoryChamado.AtualizarChamado(chamado);
        }
        public async Task<Chamado> DespacharChamado(Guid idAmbulancia, Guid idChamado)
        {
            Ambulancia ambulancia = await _repositoryAmbulancia.GetAmbulanciaById(idAmbulancia);
            if (ambulancia  == null) { throw new ArgumentException("Ambulância não encontrada!"); }
            if (ambulancia.Status != StatusAmbulancia.Disponivel) { throw new ArgumentException("Ambulância não disponível no momento!"); }

            Chamado chamado = await _repositoryChamado.BuscarChamadoById(idChamado);
            if (chamado == null) { throw new ArgumentException("Chamado não encontrado!"); }
            if (chamado.StatusChamado != StatusChamado.Aguardando) { throw new ArgumentException("Chamado não está pendente!"); }

            chamado.AmbulanciaId = idAmbulancia;
            chamado.StatusChamado = StatusChamado.EmAndamento;
            await _ambulanciaService.AtualizarStatusAmbulancia(idAmbulancia, StatusAmbulancia.EmUso);
            Chamado chamadoAtualizado = await _repositoryChamado.AtualizarChamado(chamado);

            return chamadoAtualizado;
        }
        public async Task<Chamado> EncerrarChamado(Guid idChamado)
        {
            Chamado chamado = await _repositoryChamado.BuscarChamadoById(idChamado);
            if (chamado == null) { throw new ArgumentException("Chamado não encontrado!"); }
            if (chamado.StatusChamado != StatusChamado.EmAndamento) { throw new ArgumentException("Chamado não está em andamento!"); }

            chamado.StatusChamado = StatusChamado.Finalizado;
            chamado.DataEncerramento = DateTime.Now;

            await _ambulanciaService.AtualizarStatusAmbulancia(chamado.AmbulanciaId, StatusAmbulancia.Disponivel);

            foreach (Paramedico paramedico in chamado.Paramedicos)
            {
                paramedico.Ocupado = false;
            }
            Chamado chamadoAtualizado = await _repositoryChamado.AtualizarChamado(chamado);
            return chamadoAtualizado;
        }
        public async Task<Chamado> AtualizarPrioridadeChamado(Guid idChamado, PrioridadeChamado prioridade)
        {
            Chamado chamado = await _repositoryChamado.BuscarChamadoById(idChamado);
            if (chamado == null) { throw new ArgumentException("Chamado não encontrado!"); }

            chamado.NivelPrioridade = prioridade;
            Chamado chamadoAtualizado = await _repositoryChamado.AtualizarChamado(chamado);
            return chamado;
        }
        public async Task<List<Paramedico>> ListarParamedicoChamado(Guid idChamado)
        {
            Chamado chamado = await _repositoryChamado.BuscarChamadoById(idChamado);
            if (chamado == null) { throw new ArgumentException("Chamado não encontrado!"); }

            return chamado.Paramedicos;
        }
        public async Task<Chamado> DesignarParamedico(Guid idChamado, Guid idParamedico)
        {
            Chamado chamado = await _repositoryChamado.BuscarChamadoById(idChamado);
            if (chamado == null) { throw new ArgumentException("Chamado não encontrado!"); }
            if (chamado.StatusChamado != StatusChamado.Aguardando) { throw new ArgumentException("Chamado não está pendente!"); }

            Paramedico paramedico = await _repositoryParamedico.GetParamedicoById(idParamedico);
            if (paramedico == null) { throw new ArgumentException("Paramédico não encontrado!"); }
            if (paramedico.Ocupado) { throw new ArgumentException("Paramédico ocupado no momento!"); }

            chamado.Paramedicos.Add(paramedico);
            await _paramedicoService.AtualizarDisponibilidade(idParamedico, true);

            return chamado;
        }
        public async Task<Chamado> RemoverParamedicoChamado(Guid idChamado, Guid idParamedico)
        {
            Chamado chamado = await _repositoryChamado.BuscarChamadoById(idChamado);
            if (chamado == null) { throw new ArgumentException("Chamado não encontrado!"); }
            if (chamado.StatusChamado != StatusChamado.Aguardando) { throw new ArgumentException("Chamado não está pendente!"); }

            Paramedico paramedico = await _repositoryParamedico.GetParamedicoById(idParamedico);
            if (!chamado.Paramedicos.Contains(paramedico)) { throw new ArgumentException("Paramédico não está associado com esse chamado!"); }

            chamado.Paramedicos.Remove(paramedico);
            await _repositoryChamado.AtualizarChamado(chamado);

            return chamado;
        }
    }
}