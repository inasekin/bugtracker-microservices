namespace Bugtracker.WebHost.Contracts
{
    public class RoleResponse
    {
        public string RoleName { get; set; }

        public string RoleDescription { get; set; }
        
        // TODO:
        public string Permissions { get; set; }
    }
}