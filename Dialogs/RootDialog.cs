using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using StockBot.Models;
using Newtonsoft.Json;
using System.Net.Http;

namespace StockBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            string StockRateString;
            StockLUIS StLUIS = await GetEntityFromLUIS(activity.Text);
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
            await context.PostAsync(activity.CreateReply(StockRateString));
            context.Wait(MessageReceivedAsync);
        }

        private static async Task<StockLUIS> GetEntityFromLUIS(string Query)
        {
            Query = System.Uri.EscapeDataString(Query);
            StockLUIS Data = new StockLUIS();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/ba882fbe-0898-4feb-8014-b754829de732?subscription-key=770c54f2459d47ea98b1624e364421d0&timezoneOffset=0&verbose=true&q=" + Query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<StockLUIS>(JsonDataResponse);
                }
            }
            return Data;
        }

        private async Task<string> GetStock(string StockSymbol)
        {
            double? dblStockValue = await YahooBot.GetStockRateAsync(StockSymbol);
            if (dblStockValue == null)
            {
                return string.Format("This \"{0}\" is not an valid stock symbol", StockSymbol);
            }
            else
            {
                return string.Format("Stock Price of {0} is {1}", StockSymbol, dblStockValue);
            }
        }
    }
}