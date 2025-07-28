namespace ChatAPI.DTO
{
    public class MessageRequestDTO
    {
        public int id { get; set; }
        public string? SenderId { get; set; }

        public string? RecieverId { get; set; }
        public string? Content { get; set; }

        public bool IRead { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
