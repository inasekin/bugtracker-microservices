using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using UserService.Domain.Models;

namespace UserService.Api.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManagementService _userManagementService;

        public AuthService(IConfiguration configuration, UserManagementService userManagementService)
        {
            _configuration = configuration;
            _userManagementService = userManagementService;
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            var hashedInputPassword = HashPassword(password);
            return hashedInputPassword == passwordHash;
        }

        public async Task<UserResponse?> GetUserByEmailAsync(string email)
        {
            return await _userManagementService.GetUserByEmailAsync(email);
        }

        public async Task<UserResponse?> GetUserByIdAsync(Guid userId)
        {
            return await _userManagementService.GetUserByIdAsync(userId);
        }

        public async Task RegisterUserAsync(UserResponse userResponse)
        {
            await _userManagementService.CreateUserAsync(userResponse);
        }

        public string GenerateJwtToken(UserResponse userResponse)
        {
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT ключ не сконфигурирован.");
            }

            var key = Encoding.ASCII.GetBytes(jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("id", userResponse.Id.ToString()),
                    new System.Security.Claims.Claim("name", userResponse.Name)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public JwtSecurityToken ValidateJwtToken(string token)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return (JwtSecurityToken)validatedToken;
        }
    }
}
