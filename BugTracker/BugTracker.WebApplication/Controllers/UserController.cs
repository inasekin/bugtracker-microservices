using Microsoft.AspNetCore.Mvc;
using BugTracker.WebApplication.Models;
using BugTracker.WebApplication.Services;

namespace BugTracker.WebApplication.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Получение списка всех пользователей.
        /// </summary>
        /// <returns>Список пользователей.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
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
            var user = await _userService.GetUserByIdAsync(id);
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

            await _userService.CreateUserAsync(userResponse);
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

            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound("Пользователь не найден.");
            }

            await _userService.UpdateUserAsync(userResponse);
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
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
