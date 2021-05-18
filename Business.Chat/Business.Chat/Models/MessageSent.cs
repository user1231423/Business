using Data.Chat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Chat.Models
{
    public class MessageSent
    {
        /// <summary>
        /// User ids
        /// </summary>
        public List<string> UserIds { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public Message Message { get; set; }
    }
}
