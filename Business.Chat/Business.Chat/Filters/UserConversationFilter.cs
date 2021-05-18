namespace Business.Chat.Filters
{
    public class UserConversationFilter
    {
        /// <summary>
        /// User id
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Conversation id
        /// </summary>
        public int? ConversationId { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public short? Status { get; set; }

        /// <summary>
        /// Include user
        /// </summary>
        public bool IncludeUser { get; set; }

        /// <summary>
        /// Include conversation
        /// </summary>
        public bool IncludeConversation { get; set; }
    }
}
