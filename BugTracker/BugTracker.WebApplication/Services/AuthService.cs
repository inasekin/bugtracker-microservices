using BugTracker.WebApplication.Models;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;

namespace BugTracker.WebApplication.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;

        public AuthService(IConfiguration configuration, UserService userService)
        {
            _configuration = configuration;
            _userService = userService;
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
            return await _userService.GetUserByEmailAsync(email);
        }

        public async Task<UserResponse?> GetUserByIdAsync(Guid userId)
        {
            return await _userService.GetUserByIdAsync(userId);
        }

        public async Task RegisterUserAsync(UserResponse userResponse)
        {
            await _userService.CreateUserAsync(userResponse);
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
