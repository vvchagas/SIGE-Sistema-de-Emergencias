using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEApi.DTOs.ParamedicoDtos;
using SIGEApi.Models;
using SIGEApi.Services;

namespace SIGEApi.Controllers
{
    [Authorize]
    [Route("paramedicos")]
    [ApiController]
    public class ParamedicoController(ParamedicoService service) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Paramedico>> CriarParamedico(ParamedicoCreateDTO request)
        {
            try
            {
                var paramedico = await service.CriarParamedico(request);
                return CreatedAtAction(nameof(GetParamedicoById), new { id = paramedico.Id }, paramedico);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Paramedico>>> ListarParamedicos([FromQuery] int pagina, [FromQuery]  int quantidade, [FromQuery] bool? ocupado)
        {
            try
            {
                var paramedicos = await service.Listar(pagina, quantidade, ocupado);
                return Ok(paramedicos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Paramedico>> GetParamedicoById(Guid id)
        {
            try
            {
                var paramedico = await service.GetParamedicoById(id);
                return Ok(paramedico);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletarParamedico(Guid id)
        {
            try
            {
                await service.DeletarParamedico(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/disponibilidade")]
        public async Task<ActionResult<Paramedico>> AtualizarDisponibilidade(Guid id, [FromBody] bool ocupado)
        {
            try
            {
                var paramedico = await service.AtualizarDisponibilidade(id, ocupado);
                return Ok(paramedico);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
