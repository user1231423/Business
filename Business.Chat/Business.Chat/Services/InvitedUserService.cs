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
    public class InvitedUserService
    {
        /// <summary>
        /// Chat db context
        /// </summary>
        private ChatDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public InvitedUserService(ChatDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// List conversation invited users
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
        /// Save invited user
        /// </summary>
        /// <param name="invitedUser"></param>
        /// <returns></returns>
        public async Task<int> Save(InvitedUser invitedUser)
        {
            try
            {
                _context.InvitedUsers.Add(invitedUser);

                await _context.SaveChangesAsync();

                return invitedUser.Id;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Delete invited user
        /// </summary>
        /// <param name="conversationId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> Delete(int conversationId, int userId)
        {
            try
            {
                var invitedUser = _context.InvitedUsers.SingleOrDefault(x => x.ConversationId == conversationId && x.UserId == userId);

                if (invitedUser == null)
                    throw new NotFoundException(Errors.InvitedUserNotFound);

                _context.Remove(invitedUser);

                await _context.SaveChangesAsync();

                return invitedUser.Id;
            }
            catch
            {
                throw;
            }
        }
    }
}
