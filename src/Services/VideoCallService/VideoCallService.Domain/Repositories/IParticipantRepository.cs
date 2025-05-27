using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoCallService.Domain.Models;

namespace VideoCallService.Domain.Repositories
{
    public interface IParticipantRepository
    {
        Task<VideoParticipant> GetParticipantAsync(Guid participantId);
        Task<List<VideoParticipant>> GetRoomParticipantsAsync(Guid roomId);
        Task<List<VideoParticipant>> GetUserParticipationsAsync(string userId);
        Task<VideoParticipant> AddParticipantAsync(VideoParticipant participant);
        Task<VideoParticipant> UpdateParticipantAsync(VideoParticipant participant);
        Task RemoveParticipantAsync(Guid participantId);
    }
} 