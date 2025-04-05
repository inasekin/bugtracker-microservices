using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using UserService.Domain.Models;

namespace UserService.Api.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManagementService _userManagementService;
        private readonly ICacheService _cache;
        private readonly ILogger<AuthService> _logger;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

        public AuthService(
            IConfiguration configuration,
            UserManagementService userManagementService,
            ICacheService cache,
            ILogger<AuthService> logger)
        {
            _configuration = configuration;
            _userManagementService = userManagementService;
            _cache = cache;
            _logger = logger;
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
            string cacheKey = $"user_email_{email}";
            return await _cache.GetOrCreateAsync(cacheKey, async () =>
            {
                var user = await _userManagementService.GetUserByEmailAsync(email);
                if (user != null)
                {
                    _logger.LogDebug("Пользователь с email {Email} получен из базы данных и кэширован", email);
                }
                return user;
            }, _cacheDuration);
        }

        public async Task<UserResponse?> GetUserByIdAsync(Guid userId)
        {
            // Используем Redis кэш
            string cacheKey = $"user_{userId}";
            return await _cache.GetOrCreateAsync(cacheKey, async () =>
            {
                var user = await _userManagementService.GetUserByIdAsync(userId);
                if (user != null)
                {
                    _logger.LogDebug("Пользователь {UserId} получен из базы данных и кэширован", userId);
                }
                return user;
            }, _cacheDuration);
        }

        public async Task RegisterUserAsync(UserResponse userResponse)
        {
            userResponse.Role = UserRole.Guest;
            userResponse.CreatedAt = DateTime.UtcNow;
            userResponse.UpdatedAt = DateTime.UtcNow;

            await _userManagementService.CreateUserAsync(userResponse);
            _logger.LogInformation("Зарегистрирован новый пользователь с ID {UserId}", userResponse.Id);
        }

        public string GenerateJwtToken(UserResponse userResponse)
        {
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                _logger.LogError("JWT ключ не настроен в конфигурации");
                throw new InvalidOperationException("JWT ключ не настроен.");
            }

            var key = Encoding.ASCII.GetBytes(jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", userResponse.Id.ToString()),
                    new Claim("name", userResponse.Name),
                    new Claim("email", userResponse.Email),
                    new Claim("role", userResponse.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            _logger.LogInformation("JWT токен сгенерирован для пользователя {UserId}", userResponse.Id);
            return tokenHandler.WriteToken(token);
        }

        public JwtSecurityToken ValidateJwtToken(string token)
        {
            try
            {
                var jwtKey = _configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(jwtKey))
                {
                    _logger.LogError("JWT ключ не настроен в конфигурации");
                    throw new InvalidOperationException("JWT ключ не настроен.");
                }

                var key = Encoding.ASCII.GetBytes(jwtKey);
                var tokenHandler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                _logger.LogDebug("JWT токен успешно валидирован");
                return (JwtSecurityToken)validatedToken;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка валидации JWT токена");
                return null;
            }
        }
    }
}
