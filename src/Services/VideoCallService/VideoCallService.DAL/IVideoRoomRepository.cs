using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoCallService.Domain.Models;

namespace VideoCallService.DAL
{
    public interface IVideoRoomRepository
    {
        Task<VideoRoom> GetRoomAsync(Guid roomId);
        Task<IEnumerable<VideoRoom>> GetRoomsByUserIdAsync(Guid userId);
        Task<IEnumerable<VideoRoom>> GetActiveRoomsAsync();
        Task AddRoomAsync(VideoRoom room);
        Task UpdateRoomAsync(VideoRoom room);
        Task<List<VideoRoom>> GetPublicRoomsAsync();
        Task<VideoRoom> GetByAccessCodeAsync(string accessCode);
    }
} 