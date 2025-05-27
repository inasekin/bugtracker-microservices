using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VideoCallService.Domain.Interfaces;
using VideoCallService.Domain.Models;
using VideoCallService.Api.Dtos;

namespace VideoCallService.Api.Controllers
{
    [ApiController]
    [Route("api/videocall/rooms")]
    [Authorize]
    public class VideoRoomController : ControllerBase
    {
        private readonly IVideoRoomService _roomService;
        private readonly ILogger<VideoRoomController> _logger;

        public VideoRoomController(
            IVideoRoomService roomService,
            ILogger<VideoRoomController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }
        
        [HttpPost]
        public async Task<ActionResult<VideoRoomDto>> CreateRoom([FromBody] CreateRoomRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            string userIdString = GetUserIdFromRequest();
            
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Не удалось получить ID пользователя");
            }
            
            try
            {
                var room = await _roomService.CreateRoomAsync(
                    request.Name, 
                    userId, 
                    request.MaxParticipants ?? 10,
                    request.IsPrivate ?? true);
                
                return Ok(MapToDto(room));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании комнаты пользователем {UserId}", userId);
                return StatusCode(500, "Произошла ошибка при создании комнаты");
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<VideoRoomDto>> GetRoom(string id)
        {
            try
            {
                var room = await _roomService.GetRoomAsync(id);
                if (room == null)
                {
                    return NotFound("Комната не найдена");
                }
                
                return Ok(MapToDto(room));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении комнаты {RoomId}", id);
                return StatusCode(500, "Произошла ошибка при получении комнаты");
            }
        }
        
        [HttpGet("code/{accessCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoomByCode(string accessCode)
        {
            try
            {
                var room = await _roomService.GetRoomByAccessCodeAsync(accessCode);
                if (room == null)
                {
                    return NotFound(new { message = "Комната не найдена" });
                }

                return Ok(MapToDto(room));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении комнаты по коду доступа");
                return StatusCode(500, new { message = "Произошла ошибка при получении комнаты" });
            }
        }
        
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<VideoRoomDto>>> GetUserRooms()
        {
            string userIdString = GetUserIdFromRequest();
            
            if (string.IsNullOrEmpty(userIdString))
            {
                _logger.LogWarning("ID пользователя не найден в запросе");
                return Ok(new List<VideoRoomDto>()); // Возвращаем пустой список вместо создания случайного ID
            }
            
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                _logger.LogWarning("ID пользователя имеет неверный формат: {UserId}", userIdString);
                return Ok(new List<VideoRoomDto>()); // Возвращаем пустой список вместо создания случайного ID
            }
            
            try
            {
                var rooms = await _roomService.GetUserRoomsAsync(userId);
                _logger.LogInformation("Получено {Count} комнат для пользователя {UserId}", rooms.Count, userId);
                return Ok(MapToDto(rooms));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении комнат пользователя {UserId}", userId);
                return StatusCode(500, "Произошла ошибка при получении комнат пользователя");
            }
        }
        
        [HttpPost("join")]
        [AllowAnonymous]
        public async Task<IActionResult> JoinRoom([FromBody] JoinRoomRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.RoomId))
                {
                    return BadRequest(new { message = "Не указан ID комнаты" });
                }

                if (!Guid.TryParse(request.RoomId, out Guid roomId))
                {
                    return BadRequest(new { message = "Неверный формат ID комнаты" });
                }

                // Получаем комнату
                var room = await _roomService.GetRoomAsync(roomId);
                if (room == null)
                {
                    return NotFound(new { message = "Комната не найдена" });
                }

                // Проверяем активность комнаты
                if (!room.IsActive)
                {
                    return BadRequest(new { message = "Комната закрыта" });
                }

                string userId;
                string username = request.Username;

                // Если пользователь авторизован, берем его ID из токена
                if (User.Identity?.IsAuthenticated == true)
                {
                    userId = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userId))
                    {
                        _logger.LogWarning("Не удалось получить ID пользователя из токена");
                        userId = Guid.NewGuid().ToString(); // Создаем временный ID
                        _logger.LogInformation("Создан временный ID: {UserId}", userId);
                    }
                }
                else
                {
                    // Для гостя создаем временный ID
                    userId = Guid.NewGuid().ToString();
                    _logger.LogInformation("Гость присоединяется к комнате: {Username}, ID: {UserId}", username, userId);
                }

                // Для приватных комнат неавторизованные пользователи не могут присоединиться
                if (room.IsPrivate && User.Identity?.IsAuthenticated != true)
                {
                    _logger.LogWarning("Неавторизованный пользователь пытается присоединиться к приватной комнате {RoomId}", roomId);
                    return BadRequest(new { message = "Для присоединения к приватной комнате необходима авторизация" });
                }

                // Добавляем участника в комнату
                try
                {
                    await _roomService.AddParticipantAsync(roomId, userId, username);
                    _logger.LogInformation("Пользователь {Username} ({UserId}) присоединился к комнате {RoomId}", username, userId, roomId);
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is KeyNotFoundException)
                {
                    _logger.LogWarning(ex, "Ошибка при добавлении участника: {Message}", ex.Message);
                    return BadRequest(new { message = ex.Message });
                }

                return Ok(new 
                { 
                    message = "Добро пожаловать в комнату", 
                    room = MapToDto(room),
                    userId = userId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при присоединении к комнате");
                return StatusCode(500, new { message = "Произошла ошибка при присоединении к комнате" });
            }
        }
        
        [HttpPost("join-by-code")]
        [AllowAnonymous]
        public async Task<IActionResult> JoinRoomByCode([FromBody] JoinByCodeRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Code))
                {
                    return BadRequest(new { message = "Не указан код доступа" });
                }

                // Получаем комнату по коду доступа
                var room = await _roomService.GetRoomByAccessCodeAsync(request.Code);
                if (room == null)
                {
                    return NotFound(new { message = "Комната не найдена" });
                }

                // Проверяем активность комнаты
                if (!room.IsActive)
                {
                    return BadRequest(new { message = "Комната закрыта" });
                }

                string userId;
                string username = request.Username;

                // Если пользователь авторизован, берем его ID из токена
                if (User.Identity?.IsAuthenticated == true)
                {
                    userId = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userId))
                    {
                        _logger.LogWarning("Не удалось получить ID пользователя из токена");
                        userId = Guid.NewGuid().ToString(); // Создаем временный ID
                        _logger.LogInformation("Создан временный ID: {UserId}", userId);
                    }
                }
                else
                {
                    // Для гостя создаем временный ID
                    userId = Guid.NewGuid().ToString();
                    
                    // Если не указано имя гостя, используем дефолтное
                    if (string.IsNullOrEmpty(username))
                    {
                        username = "Гость";
                    }
                    
                    _logger.LogInformation("Гость {Username} присоединяется к комнате по коду: {Code}, ID: {UserId}", 
                        username, request.Code, userId);
                }

                // Проверяем, приватная ли комната
                // Для приватных комнат мы всё равно разрешаем доступ по коду, так как код и есть "ключ" от комнаты
                if (room.IsPrivate && User.Identity?.IsAuthenticated != true)
                {
                    _logger.LogInformation("Неавторизованный пользователь присоединяется к приватной комнате по коду: {Code}", request.Code);
                }

                // Добавляем участника в комнату
                try
                {
                    await _roomService.AddParticipantAsync(room.Id, userId, username);
                    _logger.LogInformation("Пользователь {Username} ({UserId}) присоединился к комнате {RoomId} по коду", 
                        username, userId, room.Id);
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is KeyNotFoundException)
                {
                    _logger.LogWarning(ex, "Ошибка при добавлении участника: {Message}", ex.Message);
                    return BadRequest(new { message = ex.Message });
                }

                return Ok(new 
                { 
                    message = "Добро пожаловать в комнату", 
                    room = MapToDto(room),
                    userId = userId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при присоединении к комнате по коду: {Message}", ex.Message);
                return StatusCode(500, new { message = "Произошла ошибка при присоединении к комнате" });
            }
        }
        
        [HttpPost("{id}/close")]
        public async Task<ActionResult<VideoRoomDto>> CloseRoom(string id)
        {
            string userIdString = GetUserIdFromRequest();
            
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Не удалось получить ID пользователя");
            }
            
            try
            {
                if (!Guid.TryParse(id, out Guid roomId))
                {
                    return BadRequest("Неверный формат ID комнаты");
                }
                
                // Получаем комнату, чтобы проверить существует ли она
                var room = await _roomService.GetRoomAsync(roomId);
                if (room == null)
                {
                    return NotFound("Комната не найдена");
                }
                
                // Проверяем права доступа - только владелец может закрыть комнату
                if (room.OwnerId != userId)
                {
                    _logger.LogWarning("Пользователь {UserId} попытался закрыть чужую комнату {RoomId}", userId, roomId);
                    return Forbid();
                }
                
                // Если комната уже закрыта, просто возвращаем ее
                if (!room.IsActive)
                {
                    _logger.LogInformation("Комната {RoomId} уже была закрыта", roomId);
                    return Ok(MapToDto(room));
                }
                
                // Закрываем комнату
                var closedRoom = await _roomService.CloseRoomAsync(roomId, userId);
                _logger.LogInformation("Комната {RoomId} успешно закрыта пользователем {UserId}", roomId, userId);
                
                return Ok(MapToDto(closedRoom));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Комната не найдена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при закрытии комнаты {RoomId}", id);
                return StatusCode(500, "Произошла ошибка при закрытии комнаты");
            }
        }
        
        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicRooms()
        {
            try
            {
                var rooms = await _roomService.GetPublicRoomsAsync();
                return Ok(MapToDto(rooms));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка публичных комнат");
                return StatusCode(500, "Произошла ошибка при получении списка публичных комнат");
            }
        }
        
        /// <summary>
        /// Endpoint для тестирования соединения с микросервисом
        /// </summary>
        [HttpGet("test-connection")]
        [AllowAnonymous]
        public IActionResult TestConnection()
        {
            try
            {
                return Ok(new
                {
                    status = "success",
                    message = "VideoCallService API connection is working",
                    timestamp = DateTime.UtcNow,
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    serverInfo = new
                    {
                        osVersion = Environment.OSVersion.ToString(),
                        machineName = Environment.MachineName,
                        processorCount = Environment.ProcessorCount,
                        dotNetVersion = Environment.Version.ToString()
                    },
                    iceServers = new object[]
                    {
                        new { 
                            urls = new[] { "stun:stun.l.google.com:19302", "stun:stun1.l.google.com:19302" } 
                        },
                        new { 
                            urls = new[] { "turn:openrelay.metered.ca:80" },
                            username = "openrelayproject",
                            credential = "openrelayproject"
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TestConnection endpoint");
                return StatusCode(500, new { error = "Internal server error during connection test" });
            }
        }
        
        private string GetUserIdFromRequest()
        {
            // Пробуем получить ID пользователя из разных клеймов
            var userId = User.FindFirst("sub")?.Value ?? 
                   User.FindFirst("id")?.Value ??
                   User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                   User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier", StringComparison.OrdinalIgnoreCase))?.Value ??
                   Request.Headers["X-UserId"].ToString();
            
            if (!string.IsNullOrEmpty(userId))
            {
                Console.WriteLine($"Найден ID пользователя: {userId}");
            }
            else
            {
                Console.WriteLine("ID пользователя не найден. Доступные клеймы:");
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"- {claim.Type}: {claim.Value}");
                }
            }
            
            return userId;
        }
        
        // Ручное маппирование вместо AutoMapper
        private VideoRoomDto MapToDto(VideoRoom room)
        {
            if (room == null) return null;
            
            return new VideoRoomDto
            {
                Id = room.Id,
                Name = room.Name,
                OwnerId = room.OwnerId,
                AccessCode = room.AccessCode,
                CreatedAt = room.CreatedAt,
                ClosedAt = room.ClosedAt,
                IsActive = room.IsActive,
                IsPrivate = room.IsPrivate,
                MaxParticipants = room.MaxParticipants,
                Participants = room.Participants?.Select(p => new VideoParticipantDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Username = p.Username,
                    RoomId = p.RoomId,
                    JoinedAt = p.JoinedAt,
                    LeftAt = p.LeftAt,
                    IsActive = p.IsActive
                }).ToList() ?? new List<VideoParticipantDto>()
            };
        }
        
        private List<VideoRoomDto> MapToDto(List<VideoRoom> rooms)
        {
            return rooms?.Select(MapToDto).ToList() ?? new List<VideoRoomDto>();
        }
    }
    
    public class CreateRoomRequest
    {
        public required string Name { get; set; }
        public int? MaxParticipants { get; set; }
        public bool? IsPrivate { get; set; }
    }
    
    public class JoinRoomRequest
    {
        public required string RoomId { get; set; }
        public required string Username { get; set; }
    }
    
    public class JoinByCodeRequest
    {
        public required string Code { get; set; }
        public string? Username { get; set; }
    }
} 