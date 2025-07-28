namespace ChatAPI.DTO
{
    public class OnlineUserDto
    {
        public string? ID { get; set; }
        public string? ConnectionID { get; set; }
        public string? Username { get; set; }

        public string? FullName { get; set; }

        public string? Profilepicture { get; set; }

        public bool IsOnline { get; set; }

        public int UnreadCount { get; set; }


    }
}
