using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEApi.DTOs.AmbulanciaDtos;
using SIGEApi.DTOs.ChamadoDtos;
using SIGEApi.Enums;
using SIGEApi.Models;
using SIGEApi.Services;

namespace SIGEApi.Controllers
{
    [Authorize]
    [Route("chamados")]
    [ApiController]
    public class ChamadoController(ChamadoService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Chamado>>> GetChamados([FromQuery] int pagina, [FromQuery] int quantidade)
        {
            try
            {
                var chamados = await service.Listar(pagina, quantidade);
                return Ok(chamados);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Chamado>> GetChamadoById(Guid id)
        {
            try
            {
                var chamados = await service.ListarChamadoById(id);
                return Ok(chamados);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<ActionResult<Chamado>> PostCreateChamado(ChamadoRequestDto chamadoRequest)
        {
            try
            {
                var novoChamado = await service.CreateChamado(chamadoRequest);
                return CreatedAtAction(nameof(PostCreateChamado), new { id = novoChamado.Id }, novoChamado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Chamado>> PutChamado(Guid id, ChamadoAtualizarDto chamado)
        {
            try
            {
                var chamadoAtualizado = await service.AtualizarChamado(id, chamado);
                return Ok(chamadoAtualizado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("{id}/despachar")]
        public async Task<ActionResult<Chamado>> PostDespacharChamado(Guid idAmbulancia, Guid id)
        {
            try
            {
                var chamadoDespachado = await service.DespacharChamado(idAmbulancia, id);
                return Ok(chamadoDespachado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("{id}/encerrar")]
        public async Task<ActionResult<Chamado>> PostEncerrarChamado(Guid id)
        {
            try
            {
                var chamadoDespachado = await service.EncerrarChamado(id);
                return Ok(chamadoDespachado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPatch("{id}/prioridade")]
        public async Task<ActionResult<Chamado>> PatchPrioridadeChamado(Guid id, PrioridadeChamado status)
        {
            try
            {
                var chamadoAtualizado = await service.AtualizarPrioridadeChamado(id, status);
                return Ok(chamadoAtualizado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("{id}/paramedicos")]
        public async Task<ActionResult<List<Paramedico>>> GetParamedicoChamado(Guid id)
        {
            try
            {
                var paramedicos = await service.ListarParamedicoChamado(id);
                return Ok(paramedicos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("{id}/paramedicos")]
        public async Task<ActionResult<Chamado>> PostDesignarParamedico(Guid id, Guid idParamedico)
        {
            try
            {
                var chamado = await service.DesignarParamedico(id, idParamedico);
                return Ok(chamado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpDelete("{id}/paramedicos/{paramedicoId}")]
        public async Task<ActionResult<Chamado>> DeleteParamedicoChamado(Guid id, Guid paramedicoId)
        {
            try
            {
                var chamado = await service.RemoverParamedicoChamado(id, paramedicoId);
                return Ok(chamado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
