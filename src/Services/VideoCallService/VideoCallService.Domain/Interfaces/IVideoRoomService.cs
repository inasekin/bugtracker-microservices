using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoCallService.Domain.Models;

namespace VideoCallService.Domain.Interfaces
{
    public interface IVideoRoomService
    {
        Task<VideoRoom> GetRoomAsync(Guid id);
        Task<VideoRoom> GetRoomAsync(string id);
        Task<VideoRoom> GetRoomByAccessCodeAsync(string accessCode);
        Task<List<VideoRoom>> GetAllRoomsAsync();
        Task<List<VideoRoom>> GetUserRoomsAsync(Guid userId);
        Task<List<VideoRoom>> GetPublicRoomsAsync();
        Task<VideoRoom> CreateRoomAsync(string name, Guid ownerId, int maxParticipants = 10, bool isPrivate = true);
        Task<VideoRoom> JoinRoomAsync(Guid roomId, string accessCode, Guid userId, string username);
        Task<VideoRoom> AddParticipantAsync(Guid roomId, string userId, string username);
        Task<VideoRoom> CloseRoomAsync(Guid roomId, Guid userId);
        Task<VideoRoom> UpdateParticipantStatusAsync(Guid roomId, Guid userId, bool isActive);
        Task<int> GetActiveParticipantsCountAsync(Guid roomId);
    }
} 