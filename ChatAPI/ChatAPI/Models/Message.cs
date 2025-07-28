namespace ChatAPI.Models
{
    public class Message
    {
        public int ID { get; set; }

        public string? SenderId { get; set; }
        public string? RecieverID { get; set; }

        public string? Content { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsRead { get; set; }

        public APPUser? Sender { get; set; }
        public APPUser? Reciever { get; set; }
    }
}
