using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoCallService.Domain.Interfaces;
using VideoCallService.Domain.Models;

namespace VideoCallService.Infrastructure.Services
{
    public class VideoRoomService : IVideoRoomService
    {
        private readonly Domain.Interfaces.IVideoRoomRepository _repository;
        private readonly ILogger<VideoRoomService> _logger;
        
        public VideoRoomService(Domain.Interfaces.IVideoRoomRepository repository, ILogger<VideoRoomService> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        
        public async Task<VideoRoom> CreateRoomAsync(string name, Guid ownerId, int maxParticipants = 10, bool isPrivate = true)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Имя комнаты не может быть пустым", nameof(name));

            if (ownerId == Guid.Empty)
                throw new ArgumentException("ID владельца не может быть пустым", nameof(ownerId));

            if (maxParticipants <= 0)
                maxParticipants = 10;

            // Создаем новую комнату
            var room = new VideoRoom
            {
                Id = Guid.NewGuid(),
                Name = name,
                OwnerId = ownerId,
                AccessCode = GenerateAccessCode(),
                CreatedAt = DateTime.UtcNow,
                MaxParticipants = maxParticipants,
                IsActive = true,
                IsPrivate = isPrivate,
                Participants = new List<VideoParticipant>
                {
                    new VideoParticipant
                    {
                        Id = Guid.NewGuid(),
                        UserId = ownerId,
                        Username = "Owner",
                        RoomId = Guid.Empty, // Будет установлено в репозитории
                        JoinedAt = DateTime.UtcNow,
                        IsActive = true
                    }
                }
            };

            // Создаем комнату вместе с владельцем как первым участником
            return await _repository.CreateAsync(room);
        }
        
        public async Task<VideoRoom> GetRoomAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }
        
        public async Task<VideoRoom> GetRoomAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("ID комнаты не может быть пустым", nameof(id));

            if (!Guid.TryParse(id, out Guid roomId))
                throw new ArgumentException("Неверный формат ID комнаты", nameof(id));

            return await GetRoomAsync(roomId);
        }
        
        public async Task<VideoRoom> GetRoomByAccessCodeAsync(string accessCode)
        {
            if (string.IsNullOrEmpty(accessCode))
                throw new ArgumentException("Код доступа не может быть пустым", nameof(accessCode));

            return await _repository.GetByAccessCodeAsync(accessCode);
        }
        
        public async Task<List<VideoRoom>> GetAllRoomsAsync()
        {
            return await _repository.GetAllAsync();
        }
        
        public async Task<List<VideoRoom>> GetPublicRoomsAsync()
        {
            var rooms = await _repository.GetPublicRoomsAsync();
            // Фильтруем только активные комнаты
            return rooms.Where(r => r.IsActive).ToList();
        }
        
        public async Task<List<VideoRoom>> GetUserRoomsAsync(Guid userId)
        {
            var ownedRooms = await _repository.GetByOwnerIdAsync(userId);
            var participatedRooms = await _repository.GetByParticipantIdAsync(userId);
            
            // Объединяем комнаты и удаляем дубликаты
            var allRooms = new List<VideoRoom>(ownedRooms);
            foreach (var room in participatedRooms)
            {
                if (!allRooms.Any(r => r.Id == room.Id))
                {
                    allRooms.Add(room);
                }
            }
            
            // Фильтруем только активные комнаты
            return allRooms.Where(r => r.IsActive).ToList();
        }
        
        public async Task<VideoRoom> JoinRoomAsync(Guid roomId, string accessCode, Guid userId, string username)
        {
            if (roomId == Guid.Empty)
                throw new ArgumentException("ID комнаты не может быть пустым", nameof(roomId));
            
            if (userId == Guid.Empty)
                throw new ArgumentException("ID пользователя не может быть пустым", nameof(userId));
            
            var room = await _repository.GetByIdAsync(roomId);
            if (room == null)
                throw new KeyNotFoundException($"Комната с ID {roomId} не найдена");
            
            if (!room.IsActive)
                throw new InvalidOperationException("Комната закрыта");
            
            // Проверяем код доступа только для приватных комнат
            if (room.IsPrivate)
            {
                if (string.IsNullOrEmpty(accessCode))
                    throw new ArgumentException("Код доступа не может быть пустым для приватной комнаты", nameof(accessCode));
                
                if (room.AccessCode != accessCode)
                    throw new InvalidOperationException("Неверный код доступа");
            }
            
            return await AddParticipantAsync(roomId, userId.ToString(), username);
        }
        
        public async Task<VideoRoom> AddParticipantAsync(Guid roomId, string userId, string username)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("ID пользователя не может быть пустым", nameof(userId));

            if (string.IsNullOrEmpty(username))
                username = "Гость";

            // Конвертируем строковый ID в Guid
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                userGuid = Guid.NewGuid(); // Создаем новый ID для гостей
            }

            var room = await _repository.GetByIdAsync(roomId);
            if (room == null)
                throw new KeyNotFoundException($"Комната с ID {roomId} не найдена");

            // Проверяем, не превышено ли максимальное число участников
            var activeParticipants = room.Participants.Count(p => p.IsActive);
            if (activeParticipants >= room.MaxParticipants)
                throw new InvalidOperationException("Превышено максимальное количество участников");

            // Проверяем, не является ли пользователь уже участником
            var existingParticipant = room.Participants.FirstOrDefault(p => p.UserId == userGuid && p.IsActive);
            if (existingParticipant != null)
                return room; // Пользователь уже является участником комнаты

            // Создаем нового участника
            var participant = new VideoParticipant
            {
                Id = Guid.NewGuid(),
                UserId = userGuid,
                Username = username,
                RoomId = roomId,
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Добавляем участника в комнату
            room.Participants.Add(participant);
            await _repository.UpdateAsync(room);

            return room;
        }
        
        public async Task<VideoRoom> CloseRoomAsync(Guid roomId, Guid userId)
        {
            var room = await _repository.GetByIdAsync(roomId);
            if (room == null)
                throw new KeyNotFoundException($"Комната с ID {roomId} не найдена");
            
            if (room.OwnerId != userId)
                throw new UnauthorizedAccessException("Только владелец может закрыть комнату");
            
            room.ClosedAt = DateTime.UtcNow;
            room.IsActive = false;
            
            // Отмечаем всех участников как неактивных
            if (room.Participants != null)
            {
                foreach (var participant in room.Participants)
                {
                    if (participant.IsActive)
                    {
                        participant.IsActive = false;
                        participant.LeftAt = DateTime.UtcNow;
                    }
                }
            }
            
            await _repository.UpdateAsync(room);
            
            _logger.LogInformation("Комната {RoomId} закрыта. Участников отмечено как неактивные: {ParticipantCount}", 
                roomId, room.Participants?.Count(p => p.LeftAt.HasValue) ?? 0);
            
            return room;
        }
        
        public async Task<VideoRoom> UpdateParticipantStatusAsync(Guid roomId, Guid userId, bool isActive)
        {
            var room = await _repository.GetByIdAsync(roomId);
            if (room == null)
                throw new KeyNotFoundException($"Комната с ID {roomId} не найдена");
            
            var participant = room.Participants?.FirstOrDefault(p => p.UserId == userId);
            if (participant == null)
                throw new KeyNotFoundException($"Участник с ID {userId} не найден в комнате {roomId}");
            
            participant.IsActive = isActive;
            if (!isActive)
            {
                participant.LeftAt = DateTime.UtcNow;
            }
            
            await _repository.UpdateAsync(room);
            return room;
        }
        
        public async Task<int> GetActiveParticipantsCountAsync(Guid roomId)
        {
            var room = await _repository.GetByIdAsync(roomId);
            if (room == null)
                throw new KeyNotFoundException($"Комната с ID {roomId} не найдена");
            
            return room.Participants.Count(p => p.IsActive);
        }
        
        private string GenerateAccessCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var code = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            
            return code;
        }
    }
} 