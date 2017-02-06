namespace Domain.Entities
{
    public class UserMessage
    {
        public int messageId { get; set; }
        public int userId { get; set; }
        public MessageStatus messageStatus { get; set; }
        public virtual Message Message { get; set; }
        public bool IsDeleted { get; set; }
    }
}
