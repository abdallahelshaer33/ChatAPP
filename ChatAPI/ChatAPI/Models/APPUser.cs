using Microsoft.AspNetCore.Identity;

namespace ChatAPI.Models
{
    public class APPUser :IdentityUser
    {
        public string? Fullname { get; set; }
        public string? ProfileImage { get; set; }
    }
}
