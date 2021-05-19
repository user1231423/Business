using Common.Encoding.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Chat.Filters
{
    public class MessageFilter
    {
        /// <summary>
        /// Id
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// User Id
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Conversation id
        /// </summary>
        public int? ConversationId { get; set; }

        /// <summary>
        /// Create Date equal
        /// </summary>
        public DateTime? CreateDateEq { get; set; }

        /// <summary>
        /// Create date lower than equal
        /// </summary>
        public DateTime? CreateDateLte { get; set; }

        /// <summary>
        /// Create date greather than equal
        /// </summary>
        public DateTime? CreateDateGte { get; set; }
    }
}
