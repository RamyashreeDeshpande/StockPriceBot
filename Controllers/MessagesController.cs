using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using StockBot.Models;
using Newtonsoft.Json;

namespace StockBot
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
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
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

        /*
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                string StockRateString;
                StockLUIS StLUIS = await GetEntityFromLUIS(message.Text);
                if (StLUIS.intents.Length > 0)
                {
                    switch (StLUIS.intents[0].intent)
                    {
                        case "StockPrice":
                            StockRateString = await GetStock(StLUIS.entities[0].entity);
                            break;
                        case "StockPrice2":
                            StockRateString = await GetStock(StLUIS.entities[0].entity);
                            break;
                        default:
                            StockRateString = "Sorry, I am not getting you...";
                            break;
                    }
                }
                else
                {
                    StockRateString = "Sorry, I am not getting you...";
                }

                // return our reply to the user  
                return message.CreateReplyMessage(StockRateString);
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }
        */
    }
}