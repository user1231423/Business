using Business.Chat.Extensions;
using Common.ExceptionHandler.Exceptions;
using Common.Pagination.Models;
using Data.Chat;
using Data.Chat.Globalization.Errors;
using Data.Chat.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Chat.Services
{
    public class ConversationService
    {
        /// <summary>
        /// Chat db context
        /// </summary>
        private ChatDbContext _context;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public ConversationService(ChatDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Save conversation
        /// </summary>
        /// <param name="conversation"></param>
        /// <returns></returns>
        public async Task<int> Save(Conversation conversation)
        {
            try
            {
                _context.Conversations.Add(conversation);

                await _context.SaveChangesAsync();

                return conversation.Id;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Load conversation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Conversation Load(int id)
        {
            try
            {
                return _context.Conversations.SingleOrDefault(x => x.Id == id);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Load conversation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="messages">Include messages</param>
        /// <param name="users">Include users</param>
        /// <param name="kickedUsers">Include kicked users</param>
        /// <param name="invitedUsers">Include invited users</param>
        /// <returns></returns>
        public Conversation Load(int id, bool messages, bool users, bool kickedUsers, bool invitedUsers)
        {
            try
            {
                var query = _context.Conversations.AsQueryable();

                if (messages)
                    query = query.Include(x => x.Messages);

                if (users)
                    query = query.Include(x => x.Users);   
                
                if (kickedUsers)
                    query = query.Include(x => x.KickedUsers);   
                
                if (invitedUsers)
                    query = query.Include(x => x.InvitedUsers);    

                return query.SingleOrDefault(x => x.Id == id);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update conversation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="conversation"></param>
        /// <returns></returns>
        public async Task<int> Update(int id, Conversation conversation)
        {
            try
            {
                var oldConversation = Load(id);

                if (oldConversation == null)
                    throw new NotFoundException(Errors.ConversationNotFound);

                //Update old conversation fields
                oldConversation.UpdateModifiedFields(conversation, ref _context);

                conversation.UpdateDate = DateTime.Now;

                _context.Update(oldConversation);

                await _context.SaveChangesAsync();

                return oldConversation.Id;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Delete read Message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> Delete(int id)
        {
            try
            {
                var conversation = Load(id);

                if (conversation == null)
                    throw new NotFoundException(Errors.ConversationNotFound);

                _context.Remove(conversation);

                await _context.SaveChangesAsync();

                return conversation.Id;
            }
            catch
            {
                throw;
            }
        }
    }
}
