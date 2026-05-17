using Microsoft.AspNetCore.SignalR;
using SIGEApi.DTOs.UserDtos;
using SIGEApi.Enums;
using System.Diagnostics.CodeAnalysis;

namespace SIGEApi.Models
{
    public class Chamado
    {
        public Guid Id { get; set; }
        [NotNull]
        public string NomeSolicitante{ get; set; }
        [NotNull]
        public string TelefoneSolicitante { get; set; }
        [NotNull]
        public string TipoEmergencia { get; set; }
        [NotNull]
        public string Descricao { get; set; }
        [NotNull]
        public ConscienciaPaciente StatusPaciente { get; set; }
        [NotNull]
        public StatusChamado StatusChamado { get; set; }
        [NotNull]
        public PrioridadeChamado NivelPrioridade { get; set; }
        [NotNull]
        public DateTime DataAbertura { get; set; }
        public DateTime DataEncerramento { get; set; }
        public Usuario User { get; set; } 
        [NotNull]
        public string UserId { get; set; }
        public Ambulancia? Ambulancia { get; set; }
        public Guid AmbulanciaId { get; set; }
        [NotNull]
        public List<Paramedico> Paramedicos { get; set; }
        [NotNull]
        public string Logradouro { get; set; }
        [NotNull]
        public int Numero { get; set; }
        [NotNull]
        public string Cidade { get; set; }
        [NotNull]
        public string Bairro { get; set; }
        public string PontoReferencia { get; set; }
    }
    
}
