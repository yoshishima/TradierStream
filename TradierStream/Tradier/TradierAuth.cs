using System.Net;
using System.Text.Json;

namespace TradierStream.Tradier;

/// <summary>
///     The TradierAuth class provides methods for sending HTTP POST requests with authorization.
/// </summary>
public class TradierAuth
{
    /// <summary>
    ///     Sends a POST request to the specified URL with the given token as the authorization header.
    /// </summary>
    /// <param name="url">The URL of the request.</param>
    /// <param name="token">The token used for authorization.</param>
    /// <returns>
    ///     Returns the response body of the POST request as a string.
    ///     If the request is successful, the response body is returned.
    ///     If the request fails, an error message is returned.
    /// </returns>
    public static async Task<string> SendPostRequest(string url, string token)
    {
        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.Headers["Authorization"] = $"Bearer {token}";
        request.Accept = "application/json";
        request.ContentType = "application/x-www-form-urlencoded";

        try
        {
            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
        catch (WebException ex)
        {
            using (var stream = ex.Response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return $"Error: {await reader.ReadToEndAsync()}";
            }
        }
    }

    /// <summary>
    ///     Extracts the session ID from the given JSON string.
    /// </summary>
    /// <param name="jsonString">The JSON string from which to extract the session ID.</param>
    /// <returns>The extracted session ID.</returns>
    public static string ExtractSessionId(string jsonString)
    {
        using (var doc = JsonDocument.Parse(jsonString))
        {
            var root = doc.RootElement;
            var streamElement = root.GetProperty("stream");
            var sessionId = streamElement.GetProperty("sessionid").GetString();
            return sessionId;
        }
    }
}