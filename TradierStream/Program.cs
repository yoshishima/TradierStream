using Microsoft.Extensions.Configuration;
using System.Text.Json;
using TradierStream.Tradier;

namespace TradierStream
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);


            IConfiguration configuration = builder.Build();
            var WebSocketUrl = configuration["ApplicationSettings:WebSocketUrl"];
            var sessionURL = configuration["ApplicationSettings:SessionURL"];
            var token = configuration["UserSettings:BearerToken"];
            
            string sessionId = "";
            //list of symbols or options to stream
            string[] symbols = { "SPY", "QQQ" };
            string[] filters = { "All" };
            bool linebreak = false;
            bool validOnly = false;
            bool advancedDetails = false;


            // Get session ID from Tradier
            try
            {
                string response = await TradierAuth.SendPostRequest(sessionURL, token);
                sessionId = TradierAuth.ExtractSessionId(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }



            // Build JSON string to send to Tradier websocket
            string jsonString = TradierWebSocket.JsonBuilder.BuildJsonString(symbols, sessionId, filters,linebreak,validOnly,advancedDetails); ;

            // Connect to Tradier websocket
            await TradierWebSocket.ConnectToWebSocket(WebSocketUrl, sessionId, jsonString);
        }


    }


        



    

}
