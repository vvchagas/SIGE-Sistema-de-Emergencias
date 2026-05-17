using SIGEApi.Data;
using SIGEApi.Enums;
using SIGEApi.Models;
using Microsoft.EntityFrameworkCore;

namespace SIGEApi.Repositories
{
    public class ChamadoRepository
    {
        private readonly AppDbContext _context;
        public ChamadoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Chamado>> ListarChamados(int pular, int pegar)
        {
            return await _context.Chamados
                  .OrderBy(c => !(c.StatusChamado == StatusChamado.Aguardando))
                  .ThenBy(c => !(c.StatusChamado == StatusChamado.EmAndamento))
                  .ThenBy(c => !(c.NivelPrioridade == PrioridadeChamado.Nivel3_Emergencia))
                  .ThenBy(c => !(c.NivelPrioridade == PrioridadeChamado.Nivel2_Ugente))
                  .ThenBy(c => !(c.NivelPrioridade == PrioridadeChamado.Nivel1_PoucoUrgente))
                  .Skip(pular)
                  .Take(pegar)
                  .ToListAsync();
        }
        public async Task<Chamado> BuscarChamadoById(Guid id)
        {
            return await _context.Chamados.FindAsync(id);
        }
        public async Task<Chamado> SalvarChamado(Chamado chamado)
        {
            foreach (var paramedico in chamado.Paramedicos)
            {
                if (_context.Entry(paramedico).State == EntityState.Detached)
                    _context.Paramedicos.Attach(paramedico);
            }

            _context.Chamados.Add(chamado);
            await _context.SaveChangesAsync();
            return chamado;
        }
        public async Task<Chamado> AtualizarChamado(Chamado chamado)
        {
            _context.Chamados.Update(chamado);
            await _context.SaveChangesAsync();
            return chamado;
        }
    }
}
