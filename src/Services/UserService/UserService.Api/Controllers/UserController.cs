using Microsoft.AspNetCore.Mvc;
using UserService.Domain.Models;

namespace UserService.Api.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Services.UserManagementService _userManagementService;

        public UserController(Services.UserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        /// <summary>
        /// Получение списка всех пользователей.
        /// </summary>
        /// <returns>Список пользователей.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManagementService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Получение пользователя по ID.
        /// </summary>
        /// <param name="id">ID пользователя.</param>
        /// <returns>Информация о пользователе.</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userManagementService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }
            return Ok(user);
        }

        /// <summary>
        /// Создание нового пользователя.
        /// </summary>
        /// <param name="userResponse">Данные нового пользователя.</param>
        /// <returns>Созданный пользователь.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserResponse userResponse)
        {
            if (userResponse == null || string.IsNullOrWhiteSpace(userResponse.Email))
            {
                return BadRequest("Некорректные данные пользователя.");
            }

            await _userManagementService.CreateUserAsync(userResponse);
            return CreatedAtAction(nameof(GetUserById), new { id = userResponse.Id }, userResponse);
        }

        /// <summary>
        /// Обновление данных пользователя.
        /// </summary>
        /// <param name="id">ID пользователя.</param>
        /// <param name="userResponse">Обновленные данные пользователя.</param>
        /// <returns>Результат операции.</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserResponse userResponse)
        {
            if (id != userResponse.Id)
            {
                return BadRequest("ID пользователя не совпадает.");
            }

            var existingUser = await _userManagementService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound("Пользователь не найден.");
            }

            await _userManagementService.UpdateUserAsync(userResponse);
            return NoContent();
        }

        /// <summary>
        /// Удаление пользователя.
        /// </summary>
        /// <param name="id">ID пользователя.</param>
        /// <returns>Результат операции.</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userManagementService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            await _userManagementService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
