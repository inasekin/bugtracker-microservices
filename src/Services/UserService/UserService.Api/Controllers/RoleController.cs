using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Services;
using UserService.Domain.Models;

namespace UserService.Api.Controllers
{
    [Route("api/user/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;
        private readonly AuthService _authService;
        private readonly ILogger<RoleController> _logger;

        public RoleController(
            RoleService roleService,
            AuthService authService,
            ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Повышает роль пользователя с Guest до Manager
        /// </summary>
        /// <returns>Результат операции повышения роли</returns>
        [HttpPost("upgrade")]
        [Authorize]
        public async Task<IActionResult> UpgradeToManager()
        {
            try
            {
                // Получаем текущего пользователя из токена
                var token = Request.Cookies["AUTH_COOKIE"];
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Попытка повышения роли без токена авторизации");
                    return Unauthorized(new { message = "Не авторизован" });
                }

                var tokenData = _authService.ValidateJwtToken(token);
                if (tokenData == null)
                {
                    _logger.LogWarning("Попытка повышения роли с недействительным токеном");
                    return Unauthorized(new { message = "Недействительный токен" });
                }

                // Получаем ID пользователя из утверждений (claims)
                var userIdClaim = tokenData.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    _logger.LogWarning("Попытка повышения роли с токеном без ID пользователя");
                    return BadRequest(new { message = "Недопустимый идентификатор пользователя" });
                }

                // Вызываем сервис ролей для повышения пользователя
                var result = await _roleService.UpgradeUserToManagerAsync(userId);
                if (!result.Success)
                {
                    _logger.LogWarning("Не удалось повысить роль пользователя {UserId}: {Reason}", userId, result.Message);
                    return BadRequest(new { message = result.Message });
                }

                _logger.LogInformation("Пользователь {UserId} повышен до роли Менеджера", userId);
                return Ok(new { message = "Роль успешно повышена до Менеджера" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при повышении роли");
                return StatusCode(500, new { message = "Произошла ошибка при повышении роли" });
            }
        }

        /// <summary>
        /// Административная конечная точка для прямой установки роли пользователя
        /// </summary>
        [HttpPut("{userId:guid}")]
        [Authorize]
        public async Task<IActionResult> SetUserRole(Guid userId, [FromBody] SetRoleRequest request)
        {
            try
            {
                // Получаем текущего пользователя из токена, чтобы проверить, является ли он администратором
                var token = Request.Cookies["AUTH_COOKIE"];
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Попытка установки роли без токена авторизации");
                    return Unauthorized(new { message = "Не авторизован" });
                }

                var tokenData = _authService.ValidateJwtToken(token);
                if (tokenData == null)
                {
                    _logger.LogWarning("Попытка установки роли с недействительным токеном");
                    return Unauthorized(new { message = "Недействительный токен" });
                }

                // Получаем ID пользователя из утверждений
                var userIdClaim = tokenData.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var currentUserId))
                {
                    _logger.LogWarning("Попытка установки роли с токеном без ID пользователя");
                    return BadRequest(new { message = "Недопустимый идентификатор пользователя" });
                }

                // Проверяем, является ли текущий пользователь администратором
                var result = await _roleService.SetUserRoleAsync(currentUserId, userId, request.Role);
                if (!result.Success)
                {
                    _logger.LogWarning("Не удалось установить роль: {Reason}", result.Message);
                    return BadRequest(new { message = result.Message });
                }

                _logger.LogInformation("Роль пользователя {UserId} установлена на {Role} администратором {AdminId}",
                    userId, request.Role, currentUserId);
                return Ok(new { message = $"Роль пользователя успешно обновлена до {request.Role}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при установке роли пользователя");
                return StatusCode(500, new { message = "Произошла ошибка при установке роли пользователя" });
            }
        }
    }

    public class SetRoleRequest
    {
        public UserRole Role { get; set; }
    }
}
