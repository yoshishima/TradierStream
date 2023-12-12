using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace TradierStream.Tradier
{
    public class TradierWebSocket
    {
        public static async Task ConnectToWebSocket(string uri, string token, string jsonString)
        {
            using (var webSocket = new ClientWebSocket())
            {
                // Add your headers (e.g., Authorization header)
                webSocket.Options.SetRequestHeader("Authorization", $"Bearer {token}");

                // Connect
                await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                Console.WriteLine("Connected!");

                // Send parameters if required (as per server requirement)
                //string parameters = "{\"symbols\": [\"SPY231220C00462000\",\"SPY\",\"MSFT\"], \"sessionid\": \"" + token + "\", \"linebreak\": false, \"advancedDetails\": false}";
                string parameters = jsonString;
                await Send(webSocket, parameters);

                // Example to receive messages
                await Receive(webSocket);
            }
        }

        private static async Task Send(ClientWebSocket webSocket, string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            Console.WriteLine("Sent data: " + data);
        }

        private static async Task Receive(ClientWebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine(message);

                // Handle close message
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    Console.WriteLine("WebSocket connection closed.");
                }
            }
        }

        public class JsonBuilder
        {
            public static string BuildJsonString(string[] symbols, string sessionID, string[] filters, bool linebreak, bool validOnly, bool advancedDetails)
            {
                var jsonObject = new
                {
                    symbols = symbols,
                    sessionid = sessionID,
                    filter = filters,
                    linebreak = linebreak,
                    validOnly = validOnly,
                    advancedDetails = advancedDetails
                };

                return JsonSerializer.Serialize(jsonObject);
            }
        }
    }
}
