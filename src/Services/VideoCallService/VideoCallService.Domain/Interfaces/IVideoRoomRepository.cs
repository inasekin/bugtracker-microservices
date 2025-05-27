using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoCallService.Domain.Models;

namespace VideoCallService.Domain.Interfaces
{
    public interface IVideoRoomRepository
    {
        Task<VideoRoom> GetByIdAsync(Guid id);
        Task<VideoRoom> GetByAccessCodeAsync(string accessCode);
        Task<List<VideoRoom>> GetAllAsync();
        Task<List<VideoRoom>> GetActiveRoomsAsync();
        Task<List<VideoRoom>> GetPublicRoomsAsync();
        Task<List<VideoRoom>> GetByOwnerIdAsync(Guid ownerId);
        Task<List<VideoRoom>> GetByParticipantIdAsync(Guid userId);
        Task<VideoRoom> CreateAsync(VideoRoom room);
        Task<VideoRoom> UpdateAsync(VideoRoom room);
        Task<bool> DeleteAsync(Guid id);
    }
} 