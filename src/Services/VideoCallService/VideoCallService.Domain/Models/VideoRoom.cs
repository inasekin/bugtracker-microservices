using System;
using System.Collections.Generic;

namespace VideoCallService.Domain.Models
{
    public class VideoRoom
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
        public string AccessCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        
        private bool _isActive = true;
        public bool IsActive 
        { 
            get => _isActive && !ClosedAt.HasValue;
            set => _isActive = value;
        }
        
        public bool IsPrivate { get; set; } = true;
        
        public List<VideoParticipant> Participants { get; set; } = new();
        public int MaxParticipants { get; set; } = 10;
    }

    public class VideoParticipant
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public Guid RoomId { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
        
        private bool _isActive = true;
        public bool IsActive 
        { 
            get => _isActive && !LeftAt.HasValue;
            set => _isActive = value;
        }
    }
} 