using SIGEApi.Enums;
using SIGEApi.Models;
using System.Diagnostics.CodeAnalysis;

namespace SIGEApi.DTOs.ChamadoDtos
{
    public class ChamadoRequestDto
    {
        [NotNull]
        public string NomeSolicitante { get; set; }
        [NotNull]
        public string TelefoneSolicitante { get; set; }
        [NotNull]
        public string TipoEmergencia { get; set; }
        [NotNull]
        public string Descricao { get; set; }
        [NotNull]
        public ConscienciaPaciente StatusPaciente { get; set; }
        [NotNull]
        public PrioridadeChamado NivelPrioridade { get; set; }
        [NotNull]
        public Guid AmbulanciaId { get; set; }
        public List<Guid> Paramedicos { get; set; }
        [NotNull]
        public string Logradouro { get; set; }
        [NotNull]
        public int Numero { get; set; }
        [NotNull]
        public string Cidade { get; set; }
        [NotNull]
        public string Bairro { get; set; }
        public string? PontoReferencia { get; set; }
    }
}
