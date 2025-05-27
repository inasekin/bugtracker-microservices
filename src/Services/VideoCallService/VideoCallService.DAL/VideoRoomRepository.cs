using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VideoCallService.Domain.Models;

namespace VideoCallService.DAL
{
    public class VideoRoomRepository : IVideoRoomRepository
    {
        private readonly VideoCallDbContext _dbContext;
        private readonly ILogger<VideoRoomRepository> _logger;
        
        public VideoRoomRepository(VideoCallDbContext dbContext, ILogger<VideoRoomRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        
        public async Task<VideoRoom> GetRoomAsync(Guid roomId)
        {
            try
            {
                return await _dbContext.VideoRooms
                    .Include(r => r.Participants)
                    .FirstOrDefaultAsync(r => r.Id == roomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении комнаты {RoomId}", roomId);
                throw;
            }
        }
        
        public async Task<IEnumerable<VideoRoom>> GetRoomsByUserIdAsync(Guid userId)
        {
            try
            {
                // Получаем комнаты, где пользователь является владельцем или участником
                var rooms = await _dbContext.VideoRooms
                    .Include(r => r.Participants)
                    .Where(r => r.OwnerId == userId || r.Participants.Any(p => p.UserId == userId))
                    .ToListAsync();
                
                return rooms;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении комнат пользователя {UserId}", userId);
                throw;
            }
        }
        
        public async Task<IEnumerable<VideoRoom>> GetActiveRoomsAsync()
        {
            try
            {
                return await _dbContext.VideoRooms
                    .Include(r => r.Participants)
                    .Where(r => r.ClosedAt == null)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении активных комнат");
                throw;
            }
        }
        
        public async Task AddRoomAsync(VideoRoom room)
        {
            try
            {
                // Устанавливаем корректный RoomId для участников
                foreach (var participant in room.Participants)
                {
                    participant.RoomId = room.Id;
                }
                
                await _dbContext.VideoRooms.AddAsync(room);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении комнаты {RoomId}", room.Id);
                throw;
            }
        }
        
        public async Task UpdateRoomAsync(VideoRoom room)
        {
            try
            {
                // Устанавливаем корректный RoomId для участников
                foreach (var participant in room.Participants)
                {
                    participant.RoomId = room.Id;
                }
                
                _dbContext.Entry(room).State = EntityState.Modified;
                
                // Обновляем состояние участников
                foreach (var participant in room.Participants)
                {
                    var entry = _dbContext.Entry(participant);
                    if (entry.State == EntityState.Detached)
                    {
                        // Проверяем, есть ли участник в базе данных
                        var existingParticipant = await _dbContext.VideoParticipants
                            .FirstOrDefaultAsync(p => p.Id == participant.Id);
                            
                        if (existingParticipant == null)
                        {
                            // Если участника нет, добавляем его
                            await _dbContext.VideoParticipants.AddAsync(participant);
                        }
                        else
                        {
                            // Если участник уже есть, то прикрепляем его к контексту
                            _dbContext.VideoParticipants.Attach(participant);
                            entry.State = EntityState.Modified;
                        }
                    }
                }
                
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении комнаты {RoomId}", room.Id);
                throw;
            }
        }
        
        public async Task<VideoRoom> GetByAccessCodeAsync(string accessCode)
        {
            if (string.IsNullOrEmpty(accessCode))
                throw new ArgumentException("Код доступа не может быть пустым", nameof(accessCode));

            return await _dbContext.VideoRooms
                .Include(r => r.Participants)
                .FirstOrDefaultAsync(r => r.AccessCode == accessCode && r.IsActive);
        }
        
        public async Task<List<VideoRoom>> GetPublicRoomsAsync()
        {
            try
            {
                return await _dbContext.VideoRooms
                    .Include(r => r.Participants)
                    .Where(r => !r.IsPrivate && r.IsActive)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка публичных комнат");
                throw;
            }
        }
    }
} 