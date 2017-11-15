﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using Newtonsoft.Json;

namespace AttendanceBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {

                if (message.MembersAdded[0].Name != "Bot")
                {
                    ConnectorClient client = new ConnectorClient(new Uri(message.ServiceUrl));
                    var reply = message.CreateReply();
                    reply.Text = "Hello, welcome to Smart School. How may I help you?";
                    client.Conversations.ReplyToActivityAsync(reply);
                }

            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                //var message = await result;
                //await context.PostAsync("Hello. How may I help you?");
                //context.Wait(MessageReceivedAsync);

                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}