using System.Security.Claims;

namespace ChatAPI.Extensions
{
    public static class ClaimsPrinciples
    {
        public static   string GetUserName(this ClaimsPrincipal user)
        {
          return user.FindFirstValue(ClaimTypes.Name) ?? throw new Exception("Cannot Get Username ");
           
        }

        public static Guid GetUserID(this ClaimsPrincipal user)
        {
            return Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Cannot find user Id"));
        }


    }
}
