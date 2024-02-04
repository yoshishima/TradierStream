using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace TradierStream.Tradier;

/// <summary>
///     Represents a WebSocket client for interacting with Tradier stream API.
/// </summary>
public class TradierWebSocket
{
    private readonly CancellationToken _cancellationToken;

    /// <summary>
    ///     Represents a CancellationTokenSource object used for cancelling asynchronous operations.
    /// </summary>
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    /// <summary>
    ///     Represents the URI of the WebSocket service.
    /// </summary>
    private readonly Uri _serviceUri;

    /// <summary>
    ///     Represents a WebSocket connection for interacting with a Tradier API.
    /// </summary>
    private readonly ClientWebSocket _webSocket;

    /// <summary>
    ///     Represents a Tradier WebSocket connection.
    /// </summary>
    public TradierWebSocket(string url, string token)
    {
        _serviceUri = new Uri(url);
        _webSocket = new ClientWebSocket();
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        _webSocket.Options.SetRequestHeader("Authorization", $"Bearer {token}");
    }

    /// <summary>
    ///     Disconnects the WebSocket connection.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DisconnectAsync()
    {
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, _cancellationToken);
    }

    /// <summary>
    ///     Asynchronously starts receiving data from the WebSocket.
    /// </summary>
    /// <returns>The <see cref="Task" /> representing the asynchronous receiving operation.</returns>
    public async Task StartReceivingAsync()
    {
        var buffer = new ArraySegment<byte>(new byte[4096]);
        try
        {
            while (_webSocket.State == WebSocketState.Open)
                using (var ms = new MemoryStream())
                {
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await _webSocket.ReceiveAsync(buffer, _cancellationToken);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    ms.Seek(0, SeekOrigin.Begin);

                    if (result.MessageType == WebSocketMessageType.Text)
                        using (var reader = new StreamReader(ms, Encoding.UTF8))
                        {
                            var message = await reader.ReadToEndAsync();
                            ProcessMessage(message);
                        }
                    else if (result.MessageType == WebSocketMessageType.Close)
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty,
                            CancellationToken.None);
                }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            // Handle the error (e.g., attempt to reconnect or log the error)
        }
    }

    /// <summary>
    ///     Sends a message to the WebSocket server.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SendMessageAsync(string message)
    {
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
        {
            var byteMessage = Encoding.UTF8.GetBytes(message);
            var seg = new ArraySegment<byte>(byteMessage);
            await _webSocket.SendAsync(seg, WebSocketMessageType.Text, true, _cancellationToken);
        }
        else
        {
            throw new InvalidOperationException("WebSocket is not connected.");
        }
    }

    /// <summary>
    ///     Subscribes to symbols for streaming.
    /// </summary>
    /// <param name="message">The message containing symbols to subscribe to.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SubscribeToSymbolsAsync(string message)
    {
        // Convert the list of symbols to the format expected by your WebSocket service
        //var message = new { action = "subscribe", symbols = symbols };
        //string jsonMessage = JsonSerializer.Serialize(message);
        //var jsonMessage = JsonConvert.SerializeObject(message);
        await SendMessageAsync(message);
    }

    /// <summary>
    ///     Process the incoming message received from the WebSocket.
    /// </summary>
    /// <param name="message">The message received from the WebSocket.</param>
    private void ProcessMessage(string message)
    {
        // Implement your logic to handle incoming messages
        // For example, deserialize the JSON message and update UI or notify the user
        Console.WriteLine("Received message: " + message);
    }

    /// <summary>
    ///     Connects to Tradier WebSocket server.
    /// </summary>
    /// <returns>
    ///     Returns true if the connection is successfully established; otherwise, false.
    /// </returns>
    public async Task<bool> ConnectAsync()
    {
        try
        {
            await _webSocket.ConnectAsync(_serviceUri, _cancellationToken);
            StartReceivingAsync();
            return true; // Successfully connected
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while connecting: {ex.Message}");
            return false; // Failed to connect
        }
    }

    /// <summary>
    ///     Helper class for building JSON strings.
    /// </summary>
    public class JsonBuilder
    {
        /// <summary>
        ///     Build a JSON string using the provided parameters.
        /// </summary>
        /// <param name="symbols">Array of symbols or options to stream</param>
        /// <param name="sessionID">Session ID obtained from Tradier</param>
        /// <param name="filters">Array of filters to apply</param>
        /// <param name="linebreak">Flag to indicate whether to include line breaks</param>
        /// <param name="validOnly">Flag to indicate whether to include only valid data</param>
        /// <param name="advancedDetails">Flag to indicate whether to include advanced details</param>
        /// <returns>A string representation of the JSON object</returns>
        public static string BuildJsonString(string[] symbols, string sessionID, string[] filters, bool linebreak,
            bool validOnly, bool advancedDetails)
        {
            var jsonObject = new
            {
                symbols,
                sessionid = sessionID,
                filter = filters,
                linebreak,
                validOnly,
                advancedDetails
            };

            return JsonSerializer.Serialize(jsonObject);
        }
    }
}