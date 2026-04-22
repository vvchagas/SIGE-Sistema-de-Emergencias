using SIGEApi.DTOs.ParamedicoDtos;
using SIGEApi.Models;
using SIGEApi.Repositories;

namespace SIGEApi.Services
{
    public class ParamedicoService
    {
        private readonly ParamedicoRepository _repository;

        public ParamedicoService(ParamedicoRepository repository)
        {
            _repository = repository;
        }

        public async Task<Paramedico> CriarParamedico(ParamedicoCreateDTO request)
        {
            if (await _repository.VerificarParamedico(request.Cpf))
                throw new ArgumentException("Já existe um paramédico cadastrado com esse CPF.");

            var paramedico = new Paramedico
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Cpf = request.Cpf,
                Telefone = request.Telefone,
                Email = request.Email,
                IdentificadorProfissional = request.IdentificadorProfissional,
                Cargo = request.Cargo,
                Ocupado = false // sempre começa disponível
            };

            return await _repository.CriarParamedico(paramedico);
        }

        public async Task<List<Paramedico>> Listar(int pagina, int quantidade, bool? ocupado)
        {
            int pular = (pagina - 1) * quantidade;
            return await _repository.Listar(pular, quantidade, ocupado);
        }

        public async Task<Paramedico> GetParamedicoById(Guid id)
        {
            var paramedico = await _repository.GetParamedicoById(id);
            if (paramedico == null)
                throw new ArgumentException("Paramédico não encontrado.");
            return paramedico;
        }

        public async Task<Paramedico> DeletarParamedico(Guid id)
        {
            var paramedico = await _repository.GetParamedicoById(id);
            if (paramedico == null)
                throw new ArgumentException("Paramédico não encontrado.");
            return await _repository.DeletarParamedico(paramedico);
        }

        public async Task<Paramedico> AtualizarDisponibilidade(Guid id, bool ocupado)
        {
            var paramedico = await _repository.GetParamedicoById(id);
            if (paramedico == null)
                throw new ArgumentException("Paramédico não encontrado.");

            paramedico.Ocupado = ocupado;
            return await _repository.AtualizarParamedico(paramedico);
        }
    }
}
