using Business.Chat.Extensions;
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
    public class ReadMessageService
    {
        /// <summary>
        /// Chat db context
        /// </summary>
        private ChatDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public ReadMessageService(ChatDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// List user read messages users
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public PagedList<ReadMessage> List(int userId, PaginationParams pagination)
        {
            try
            {
                return _context.MessagesRead.Where(x => x.UserId == userId).ToPagedList(pagination);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Save read message
        /// </summary>
        /// <param name="readMessage"></param>
        /// <returns></returns>
        public async Task<int> Save(ReadMessage readMessage)
        {
            try
            {
                _context.MessagesRead.Add(readMessage);

                await _context.SaveChangesAsync();

                return readMessage.Id;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Load read message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ReadMessage Load(int id)
        {
            try
            {
                return _context.MessagesRead.SingleOrDefault(x => x.Id == id);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update read message
        /// </summary>
        /// <param name="id"></param>
        /// <param name="readMessage"></param>
        /// <returns></returns>
        public async Task<int> Update(int id, ReadMessage readMessage)
        {
            try
            {
                var oldReadMessage = Load(id);

                if (oldReadMessage == null)
                    throw new NotFoundException(Errors.ReadMessageNotFound);

                //Update read message fields
                oldReadMessage.UpdateModifiedFields(readMessage, ref _context);

                _context.Update(oldReadMessage);

                await _context.SaveChangesAsync();

                return oldReadMessage.Id;
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
                var readMessage = Load(id);

                if (readMessage == null)
                    throw new NotFoundException(Errors.ReadMessageNotFound);

                _context.Remove(readMessage);

                await _context.SaveChangesAsync();

                return readMessage.Id;
            }
            catch
            {
                throw;
            }
        }
    }
}
