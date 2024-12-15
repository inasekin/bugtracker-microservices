using System.Collections.Generic;

namespace Bugtracker.WebHost.Contracts
{
    public class UserRoleResponse
    {
        public UserResponse User { get; set; }

        public IEnumerable<RoleResponse> Roles { get; set; }
    }
}