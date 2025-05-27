using System;
using System.Collections.Generic;

namespace VideoCallService.Api.Dtos
{
    public class VideoRoomDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public Guid OwnerId { get; set; }
        public required string AccessCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrivate { get; set; }
        public int MaxParticipants { get; set; }
        public List<VideoParticipantDto> Participants { get; set; } = new List<VideoParticipantDto>();
    }

    public class VideoParticipantDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Username { get; set; }
        public Guid RoomId { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
        public bool IsActive { get; set; }
    }
} 