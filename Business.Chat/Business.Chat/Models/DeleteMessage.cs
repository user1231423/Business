using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Chat.Models
{
    public class DeleteMessage
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        public int UserId { get; set; }
    }
}
