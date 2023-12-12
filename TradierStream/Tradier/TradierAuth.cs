using System.Net;
using System.Text.Json;

namespace TradierStream.Tradier
{
    public class TradierAuth
    {
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

        public static string ExtractSessionId(string jsonString)
        {
            using (JsonDocument doc = JsonDocument.Parse(jsonString))
            {
                JsonElement root = doc.RootElement;
                JsonElement streamElement = root.GetProperty("stream");
                string sessionId = streamElement.GetProperty("sessionid").GetString();
                return sessionId;
            }
        }
    }
}
