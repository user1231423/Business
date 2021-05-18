using Business.Chat.Models;
using Business.Chat.Services;
using Common.ExceptionHandler.Exceptions;
using Data.Chat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Chat
{
    public class ChatService
    {
        /// <summary>
        /// User service
        /// </summary>
        private readonly UserService _userService;

        /// <summary>
        /// Conversation service
        /// </summary>
        private readonly ConversationService _conversationService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="conversationService"></param>
        public ChatService(UserService userService, ConversationService conversationService)
        {
            _userService = userService;
            _conversationService = conversationService;
        }

        /// <summary>
        /// Send message to conversation
        /// </summary>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public async Task<MessageSent> SendMessage(SendMessage sendMessage)
        {
            try
            {
                //Find message author
                var author = _userService.Load(sendMessage.UserId);

                if (author == null)
                    throw new NotFoundException("User not found");

                //Load conversation
                var conversation = _conversationService.Load(sendMessage.ConversationId, false, true, false, false);

                if (conversation == null)
                    throw new NotFoundException("Conversation not found");

                //Find user conversation
                var userConversation = conversation.Users.Where(x => x.UserId == sendMessage.UserId).FirstOrDefault();

                if (userConversation == null)
                    throw new NotFoundException("User conversation not found");

                if (userConversation.Status == -1)
                    throw new BadRequestException("User was kicked");

                if (userConversation.Status == -2)
                    throw new BadRequestException("User left the conversation");

                //Create message
                var message = new Message()
                {
                    Body = sendMessage.Body,
                    Conversation = conversation,
                    ConversationId = conversation.Id,
                    CreateDate = DateTime.Now,
                    UpdateDate = null,
                    User = author,
                    UserId = author.Id
                };

                //Add message to conversation
                conversation.Messages.Add(message);

                await _conversationService.Update(conversation.Id, conversation);

                var messageSent = new MessageSent()
                {
                    UserIds = conversation.Users.Where(x => x.UserId != sendMessage.UserId).Select(x => x.UserId.ToString()).ToList(),
                    Message = message
                };

                //Return message sent object
                return messageSent;
            }
            catch
            {
                throw;
            }
        }
    }
}
