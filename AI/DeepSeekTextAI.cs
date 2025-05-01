using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MarkGPT.AI
{
    public static class DeepSeekTextAI
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<string> GetDeepSeekResponseAsync(string prompt)
        {
           prompt = "You're a numerical methods expert. Please only respond with the required calculation steps. Do not explain or provide extra text." + prompt;
            string openRouterApiKey = "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"; // Replace with your OpenRouter API key
            var url = "https://openrouter.ai/api/v1/chat/completions"; // OpenRouter API endpoint

            // Create the request body similar to the Python example
            var requestBody = new
            {
                model = "deepseek/deepseek-chat:free", // Model name based on your example
                messages = new[]
                {
            new { role = "user", content = prompt }
        }
            };

            var json = JsonConvert.SerializeObject(requestBody); // Serialize the object to JSON
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Create the HttpRequestMessage and set the necessary headers
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            // Add the Authorization header to the request
            requestMessage.Headers.Add("Authorization", $"Bearer {openRouterApiKey}");



            try
            {
                // Send the POST request with the HttpRequestMessage
                var response = await httpClient.SendAsync(requestMessage);

                // If response is not successful, log the status code
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return $"Error: {response.StatusCode} - {errorResponse}";
                }

                // Read and log the response string for debugging
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserialize the response
                dynamic result = JsonConvert.DeserializeObject(responseString);

                // Check if we have a valid response and return the content
                if (result?.choices?.Count > 0)
                {
                    return result?.choices?[0]?.message?.content ?? "No content in the response.";
                }
                else
                {
                    return "No response from OpenRouter.";
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the request
                return $"Exception: {ex.Message}";
            }
            
        }
    }
}
  
