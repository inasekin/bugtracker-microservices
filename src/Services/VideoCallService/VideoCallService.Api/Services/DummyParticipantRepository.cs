using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoCallService.Domain.Models;
using VideoCallService.Domain.Repositories;

namespace VideoCallService.Api.Services
{
    // Временная заглушка для интерфейса IParticipantRepository
    public class DummyParticipantRepository : IParticipantRepository
    {
        private readonly ILogger<DummyParticipantRepository> _logger;
        
        public DummyParticipantRepository(ILogger<DummyParticipantRepository> logger)
        {
            _logger = logger;
        }
        
        public Task<VideoParticipant> GetParticipantAsync(Guid participantId)
        {
            _logger.LogInformation("Вызов GetParticipantAsync с ID: {ParticipantId}", participantId);
            return Task.FromResult(new VideoParticipant 
            { 
                Id = participantId,
                UserId = Guid.NewGuid(),
                Username = "Тестовый участник",
                RoomId = Guid.NewGuid(),
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            });
        }
        
        public Task<List<VideoParticipant>> GetRoomParticipantsAsync(Guid roomId)
        {
            _logger.LogInformation("Вызов GetRoomParticipantsAsync с ID комнаты: {RoomId}", roomId);
            return Task.FromResult(new List<VideoParticipant>());
        }
        
        public Task<List<VideoParticipant>> GetUserParticipationsAsync(string userId)
        {
            _logger.LogInformation("Вызов GetUserParticipationsAsync с ID пользователя: {UserId}", userId);
            return Task.FromResult(new List<VideoParticipant>());
        }
        
        public Task<VideoParticipant> AddParticipantAsync(VideoParticipant participant)
        {
            _logger.LogInformation("Вызов AddParticipantAsync для пользователя: {UserId}", participant.UserId);
            participant.Id = Guid.NewGuid();
            return Task.FromResult(participant);
        }
        
        public Task<VideoParticipant> UpdateParticipantAsync(VideoParticipant participant)
        {
            _logger.LogInformation("Вызов UpdateParticipantAsync для участника: {ParticipantId}", participant.Id);
            return Task.FromResult(participant);
        }
        
        public Task RemoveParticipantAsync(Guid participantId)
        {
            _logger.LogInformation("Вызов RemoveParticipantAsync с ID: {ParticipantId}", participantId);
            return Task.CompletedTask;
        }
    }
} 