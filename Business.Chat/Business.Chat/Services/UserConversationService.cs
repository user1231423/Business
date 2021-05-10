using Business.Chat.Extensions;
using Common.ExceptionHandler.Exceptions;
using Data.Chat;
using Data.Chat.Models;
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
