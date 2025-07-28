using System.ComponentModel.DataAnnotations;

namespace ChatAPI.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }
    }
}
