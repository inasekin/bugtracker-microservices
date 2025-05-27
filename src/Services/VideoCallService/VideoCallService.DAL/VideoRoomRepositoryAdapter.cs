using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoCallService.Domain.Models;
using VideoCallService.Domain.Interfaces;

namespace VideoCallService.DAL
{
    public class VideoRoomRepositoryAdapter : Domain.Interfaces.IVideoRoomRepository
    {
        private readonly IVideoRoomRepository _repository;
        private readonly ILogger<VideoRoomRepositoryAdapter> _logger;

        public VideoRoomRepositoryAdapter(IVideoRoomRepository repository, ILogger<VideoRoomRepositoryAdapter> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<VideoRoom> GetByIdAsync(Guid id)
        {
            return await _repository.GetRoomAsync(id);
        }

        public async Task<VideoRoom> GetByAccessCodeAsync(string accessCode)
        {
            try
            {
                var rooms = await _repository.GetActiveRoomsAsync();
                return rooms.FirstOrDefault(r => r.AccessCode == accessCode && r.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении комнаты по коду доступа {AccessCode}", accessCode);
                throw;
            }
        }

        public async Task<List<VideoRoom>> GetAllAsync()
        {
            var rooms = await _repository.GetActiveRoomsAsync();
            return new List<VideoRoom>(rooms);
        }
        
        public async Task<List<VideoRoom>> GetActiveRoomsAsync()
        {
            var rooms = await _repository.GetActiveRoomsAsync();
            return new List<VideoRoom>(rooms);
        }

        public async Task<List<VideoRoom>> GetByOwnerIdAsync(Guid ownerId)
        {
            var allRooms = await _repository.GetRoomsByUserIdAsync(ownerId);
            return new List<VideoRoom>(allRooms.Where(r => r.OwnerId == ownerId));
        }

        public async Task<List<VideoRoom>> GetByParticipantIdAsync(Guid participantId)
        {
            var allRooms = await _repository.GetRoomsByUserIdAsync(participantId);
            return new List<VideoRoom>(allRooms.Where(r => r.Participants.Any(p => p.UserId == participantId)));
        }

        public async Task<VideoRoom> CreateAsync(VideoRoom room)
        {
            await _repository.AddRoomAsync(room);
            return room;
        }

        public async Task<VideoRoom> UpdateAsync(VideoRoom room)
        {
            await _repository.UpdateRoomAsync(room);
            return room;
        }
        
        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var room = await GetByIdAsync(id);
                if (room == null)
                {
                    return false;
                }
                
                room.IsActive = false;
                room.ClosedAt = DateTime.UtcNow;
                await _repository.UpdateRoomAsync(room);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении комнаты {RoomId}", id);
                return false;
            }
        }

        public async Task<List<VideoRoom>> GetPublicRoomsAsync()
        {
            try
            {
                return await _repository.GetPublicRoomsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении публичных комнат");
                throw;
            }
        }
    }
} 