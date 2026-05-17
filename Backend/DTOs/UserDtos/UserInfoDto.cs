namespace SIGEApi.DTOs.UserDtos
{
    public class UserInfoDto
    {
        public Guid? Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Roles { get; set; }
        public string Cpf {  get; set; }
        public string Cargo { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
