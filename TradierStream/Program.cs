using Microsoft.Extensions.Configuration;
using TradierStream.Tradier;

namespace TradierStream;

/// <summary>
/// Represents the entry point and main class for the TradierStream application.
/// </summary>
internal class Program
{
    /// <summary>
    /// Entry point for the application.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task Main()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true);


        IConfiguration configuration = builder.Build();

        var WebSocketUrl = configuration["ApplicationSettings:WebSocketUrl"];
        var sessionURL = configuration["ApplicationSettings:SessionURL"];
        var token = configuration["UserSettings:BearerToken"];


        var sessionId = "";
        //list of symbols or options to stream
        string[] symbols = { "SPY", "QQQ" };
        string[] filters = { "All" };
        var linebreak = false;
        var validOnly = false;
        var advancedDetails = false;


        // Get session ID from Tradier
        try
        {
            var response = await TradierAuth.SendPostRequest(sessionURL, token);
            sessionId = TradierAuth.ExtractSessionId(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        var jsonString =
            TradierWebSocket.JsonBuilder.BuildJsonString(symbols, sessionId, filters, linebreak, validOnly,
                advancedDetails);
        ;

        var client = new TradierWebSocket(WebSocketUrl, sessionId);
        var connected = await client.ConnectAsync();

        if (connected) await client.SubscribeToSymbolsAsync(jsonString);

        Task.Run(async () => await client.StartReceivingAsync());
        
        Console.WriteLine("Press 'q' to quit.");
        while (Console.ReadKey().KeyChar != 'q')
        {
            Console.WriteLine("Running... Press 'q' to quit.");
            Thread.Sleep(1000);
        }
        
        client.DisconnectAsync();
        await Task.Delay(1000);
    }
}