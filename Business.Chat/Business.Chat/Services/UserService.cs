using Data.Chat;
using Data.Chat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Chat.Services
{
    public class UserService
    {
        /// <summary>
        /// Chat db context
        /// </summary>
        private ChatDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public UserService(ChatDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Load user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User Load(int id)
        {
            try
            {
                return _context.Users.SingleOrDefault(x => x.Id == id);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Returns all existing user ids from list of user ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<User> LoadByIds(List<int> ids)
        {
            return _context.Users.Where(x => ids.Contains(x.Id)).ToList();
        }
    }
}
