using yourOrder.Core.Entity.Identity;

namespace yourOrder.APIs.DTOs.Account
{
    public class UserDto
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public RefreshToken? RefreshToken { get; set; }
    }
    
}
