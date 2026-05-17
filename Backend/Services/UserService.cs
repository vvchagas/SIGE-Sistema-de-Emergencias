using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SIGEApi.DTOs.UserDtos;
using SIGEApi.Models;
using System.Security.Claims;

namespace SeuProjeto.Services.Context
{
    public class UserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<Usuario> _userManager;

        public UserService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<Usuario> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        
        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public bool IsAuthenticated()
        {
            return User?.Identity?.IsAuthenticated ?? false;
        }

        public Guid? GetUserId()
        {
            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return null;

            return Guid.TryParse(userIdClaim.Value, out var id) ? id : null;
        }
        public async Task<Usuario?> GetUserAsync()
        {
            var userId = GetUserId();

            if (userId == null)
                return null;

            return await _userManager.FindByIdAsync(userId.ToString());
        }

        public string? GetEmail()
        {
            return User?.FindFirst(ClaimTypes.Email)?.Value;
        }

        public string? GetUserName()
        {
            return User?.FindFirst(ClaimTypes.Name)?.Value;
        }

        public string? GetRole()
        {
            return User?.FindFirst(ClaimTypes.Role)?.Value;
        }
        public async Task<string?> GetCpfAsync()
        {
            var user = await GetUserAsync();

            return user?.Cpf;
        }
        public async Task<string?> GetCargoAsync()
        {
            var user = await GetUserAsync();

            return user?.Cargo;
        }
        public async Task<DateTime?> GetCriacaoAsync()
        {
            var user = await GetUserAsync();

            return user?.DataCriacao;
        }
        public async Task<UserInfoDto> GetAllInfoAsync()
        {
            var user = await GetUserAsync();

            UserInfoDto userInfo = new UserInfoDto()
            {
                Id = GetUserId(),
                Nome = GetUserName(),
                Email = GetEmail(),
                IsAuthenticated = IsAuthenticated(),
                Roles = GetRole(),
                Cpf = user?.Cpf,
                Cargo = user?.Cargo,
                DataCriacao = user.DataCriacao
            };

            return userInfo;
        }
    }
}