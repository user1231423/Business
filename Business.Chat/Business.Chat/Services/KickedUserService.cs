using Common.ExceptionHandler.Exceptions;
using Common.Pagination;
using Common.Pagination.Models;
using Data.Chat;
using Data.Chat.Globalization.Errors;
using Data.Chat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Chat.Services
{
    public class KickedUserService
    {
        /// <summary>
        /// Chat db context
        /// </summary>
        private ChatDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public KickedUserService(ChatDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// List conversation kicked users
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public PagedList<KickedUser> List(int conversationId, PaginationParams pagination)
        {
            try
            {
                return _context.KickedUsers.Where(x => x.ConversationId == conversationId).ToPagedList(pagination);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Save kicked user
        /// </summary>
        /// <param name="kickedUser"></param>
        /// <returns></returns>
        public async Task<int> Save(KickedUser kickedUser)
        {
            try
            {
                _context.KickedUsers.Add(kickedUser);

                await _context.SaveChangesAsync();

                return kickedUser.Id;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Delete kicked user
        /// </summary>
        /// <param name="conversationId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> Delete(int conversationId, int userId)
        {
            try
            {
                var kickedUser = _context.KickedUsers.SingleOrDefault(x => x.ConversationId == conversationId && x.UserId == userId);

                if (kickedUser == null)
                    throw new NotFoundException(Errors.KickedUserNotFound);

                _context.Remove(kickedUser);

                await _context.SaveChangesAsync();

                return kickedUser.Id;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Check if user has been kicked from conversation
        /// </summary>
        /// <param name="conversationId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool HasBeenKicked(int conversationId, int userId)
        {
            try
            {
                return _context.KickedUsers.Any(x => x.ConversationId == conversationId && x.UserId == userId);
            }
            catch
            {
                throw;
            }
        }
    }
}
