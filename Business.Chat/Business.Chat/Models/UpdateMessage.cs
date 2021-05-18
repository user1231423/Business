using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Chat.Models
{
    public class UpdateMessage
    {
        /// <summary>
        /// Message Body
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "Body length must be at least 1")]
        public string Body { get; set; }
    }
}
