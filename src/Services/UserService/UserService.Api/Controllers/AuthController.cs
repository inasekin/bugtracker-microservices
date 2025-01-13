using Microsoft.AspNetCore.Mvc;
using UserService.Api.Services;
using UserService.Domain.Models;

namespace UserService.Api.Controllers
{
    [Route("api/user/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Регистрация нового пользователя.
        /// </summary>
        /// <param name="request">Данные для регистрации пользователя.</param>
        /// <returns>Результат операции регистрации.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!_authService.IsValidEmail(request.Email))
            {
                return BadRequest("Неверный формат email.");
            }

            var existingUser = await _authService.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest("Пользователь с таким email уже зарегистрирован.");
            }

            var passwordHash = _authService.HashPassword(request.Password);
            var user = new UserResponse
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHash
            };

            await _authService.RegisterUserAsync(user);

            // Генерация JWT токена
            var token = _authService.GenerateJwtToken(user);

            // Установка куки
            Response.Cookies.Append("AUTH_COOKIE", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new { message = "Вы успешно зарегистрировались." });
        }

        /// <summary>
        /// Авторизация пользователя.
        /// </summary>
        /// <param name="request">Данные для входа (email и пароль).</param>
        /// <returns>Токен авторизации или ошибка.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!_authService.IsValidEmail(request.Email))
            {
                return BadRequest("Неверный формат email.");
            }

            var user = await _authService.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized("Пользователь не найден.");
            }

            if (!_authService.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized("Неверное имя или пароль.");
            }

            var token = _authService.GenerateJwtToken(user);

            // Установка куки
            Response.Cookies.Append("AUTH_COOKIE", token, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new AuthResponse { Token = token, Name = user.Name });
        }

        /// <summary>
        /// Выход из системы.
        /// </summary>
        /// <returns>Результат операции выхода.</returns>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Удаление куки с JWT токеном
            Response.Cookies.Delete("AUTH_COOKIE");

            return Ok(new { message = "Вы успешно вышли из системы." });
        }

        /// <summary>
        /// Получение текущего пользователя по токену.
        /// </summary>
        /// <returns>Информация о текущем пользователе.</returns>
        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            var token = Request.Cookies["AUTH_COOKIE"];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Не авторизован" });
            }
            // Проверка токена и возврат данных пользователя
            var user = _authService.ValidateJwtToken(token);
            if (user == null)
            {
                return Unauthorized(new { message = "Неверный токен" });
            }

            return Ok(new { data = user });
        }
    }
}
