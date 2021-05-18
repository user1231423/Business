using Business.Chat.Extensions;
using Business.Chat.Filters;
using Common.ExceptionHandler.Exceptions;
using Common.Pagination;
using Common.Pagination.Models;
using Data.Chat;
using Data.Chat.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Chat.Services
{
    public class UserConversationService
    {
        /// <summary>
        /// Chat db context
        /// </summary>
        private ChatDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public UserConversationService(ChatDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Filter 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public  PagedList<UserConversation> Filter(UserConversationFilter filter, PaginationParams pagination)
        {
            try
            {
                var query = _context.UserConversations.AsQueryable();

                if (filter.IncludeConversation)
                    query = query.Include(x => x.Conversation);

                if (filter.IncludeUser)
                    query = query.Include(x => x.User);

                if (filter.UserId != null)
                    query = query.Where(x => x.UserId == filter.UserId);

                if (filter.ConversationId != null)
                    query = query.Where(x => x.ConversationId == filter.ConversationId);

                if (filter.Status != null)
                    query = query.Where(x => x.Status == filter.Status);

                return query.ToPagedList(pagination);
            }catch
            {
                throw;
            }
        }

        /// <summary>
        /// Save user conversation
        /// </summary>
        /// <param name="userConversation"></param>
        /// <returns></returns>
        public async Task<int> Save(UserConversation userConversation)
        {
            try
            {
                _context.UserConversations.Add(userConversation);

                await _context.SaveChangesAsync();

                return userConversation.Id;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Load user conversations
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserConversation> Load(int userId)
        {
            try
            {
                return _context.UserConversations.Where(x => x.UserId == userId).ToList();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Load user conversations
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserConversation Load(int userId, int conversationId)
        {
            try
            {
                return _context.UserConversations.Where(x => x.UserId == userId && x.ConversationId == conversationId).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update user conversation id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userConversation"></param>
        /// <returns></returns>
        public async Task<int> Update(int id, UserConversation userConversation)
        {
            try
            {
                var oldUserConversation = _context.UserConversations.SingleOrDefault(x => x.Id == id);

                if (oldUserConversation == null)
                    throw new NotFoundException("Message not found");

                //Update old message fields
                oldUserConversation.UpdateModifiedFields(oldUserConversation, ref _context);

                userConversation.UpdateDate = DateTime.Now;

                _context.Update(oldUserConversation);

                await _context.SaveChangesAsync();

                return oldUserConversation.Id;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Delete Message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> Delete(int id)
        {
            try
            {
                var userConversation = _context.UserConversations.SingleOrDefault(x => x.Id == id);

                if (userConversation == null)
                    throw new NotFoundException("User conversation not found");

                _context.Remove(userConversation);

                await _context.SaveChangesAsync();

                return userConversation.Id;
            }
            catch
            {
                throw;
            }
        }
    }
}
