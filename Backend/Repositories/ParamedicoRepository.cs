using Microsoft.EntityFrameworkCore;
using SIGEApi.Data;
using SIGEApi.Models;

namespace SIGEApi.Repositories
{
    public class ParamedicoRepository
    {
        private readonly AppDbContext _context;

        public ParamedicoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Paramedico> CriarParamedico(Paramedico paramedico)
        {
            _context.Paramedicos.Add(paramedico);
            await _context.SaveChangesAsync();
            return paramedico;
        }

        public async Task<bool> VerificarParamedico(string cpf)
        {
            return await _context.Paramedicos.AnyAsync(p => p.Cpf == cpf);
        }

        public async Task<List<Paramedico>> Listar(int pular, int pegar, bool? ocupado)
        {
            var query = _context.Paramedicos.AsQueryable();

            if (ocupado.HasValue)
                query = query.Where(p => p.Ocupado == ocupado.Value);

            return await query
                .OrderBy(p => p.Ocupado) // disponíveis primeiro
                .ThenBy(p => p.Name)
                .Skip(pular)
                .Take(pegar)
                .ToListAsync();
        }

        public async Task<Paramedico> GetParamedicoById(Guid id)
        {
            return await _context.Paramedicos.FindAsync(id);
        }

        public async Task<Paramedico> DeletarParamedico(Paramedico paramedico)
        {
            _context.Paramedicos.Remove(paramedico);
            await _context.SaveChangesAsync();
            return paramedico;
        }

        public async Task<Paramedico> AtualizarParamedico(Paramedico paramedico)
        {
            _context.Paramedicos.Update(paramedico);
            await _context.SaveChangesAsync();
            return paramedico;
        }
    }
}
