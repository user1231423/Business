using Business.Chat.Extensions;
using Business.Chat.Filters;
using Common.ExceptionHandler.Exceptions;
using Common.Pagination;
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
    public class MessageService
    {
        /// <summary>
        /// Chat db context
        /// </summary>
        private ChatDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public MessageService(ChatDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Filter 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public PagedList<Message> Filter(MessageFilter filter, PaginationParams pagination)
        {
            try
            {
                var query = _context.Messages.AsQueryable();

                if (filter.Id != null)
                    query = query.Where(x => x.Id == filter.Id);

                if (filter.UserId != null)
                    query = query.Where(x => x.UserId == filter.UserId);

                if (filter.ConversationId != null)
                    query = query.Where(x => x.ConversationId == filter.ConversationId);

                if (filter.CreateDateEq != null)
                    query = query.Where(x => x.CreateDate == filter.CreateDateEq);

                if (filter.CreateDateGte != null)
                    query = query.Where(x => x.CreateDate >= filter.CreateDateGte);

                if (filter.CreateDateLte != null)
                    query = query.Where(x => x.CreateDate <= filter.CreateDateLte);

                return query.ToPagedList(pagination);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Save message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<int> Save(Message message)
        {
            try
            {
                _context.Messages.Add(message);

                await _context.SaveChangesAsync();

                return message.Id;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Load message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Message Load(int id)
        {
            try
            {
                return _context.Messages.SingleOrDefault(x => x.Id == id);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Load message
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user">Include user</param>
        /// <param name="conversation">Include conversation</param>
        /// <param name="readMessages">Include read messages</param>
        /// <returns></returns>
        public Message Load(int id, bool user, bool conversation, bool readMessages)
        {
            try
            {
                var query = _context.Messages.AsQueryable();

                if (user)
                    query = query.Include(x => x.User);

                if (conversation)
                    query = query.Include(x => x.Conversation);

                if (readMessages)
                    query = query.Include(x => x.ReadMessages);

                return query.SingleOrDefault(x => x.Id == id);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update message
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<int> Update(int id, Message message)
        {
            try
            {
                var oldMessage = Load(id);

                if (oldMessage == null)
                    throw new NotFoundException(Errors.MessageNotFound);

                //Update old message fields
                oldMessage.UpdateModifiedFields(message, ref _context);

                message.UpdateDate = DateTime.Now;

                _context.Update(oldMessage);

                await _context.SaveChangesAsync();

                return oldMessage.Id;
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
                var message = Load(id);

                if (message == null)
                    throw new NotFoundException(Errors.MessageNotFound);

                _context.Remove(message);

                await _context.SaveChangesAsync();

                return message.Id;
            }
            catch
            {
                throw;
            }
        }
    }
}
