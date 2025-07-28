using ChatAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatAPI.Data
{
    public class Appdbcontext :IdentityDbContext<APPUser>
    {
        public Appdbcontext(DbContextOptions<Appdbcontext> options):base(options)
        {
            
        }
        public DbSet<Message> messages { get; set; }
        public DbSet<User> users { get; set; }
    }
}
