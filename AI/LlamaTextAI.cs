using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MarkGPT.Methods;
using System.Diagnostics;

namespace MarkGPT.AI
{
    public static class LlamaTextAI
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private static List<dynamic> conversationHistory = new List<dynamic>
        {
            new
            {
                role = "system",
                content = @"You are a numerical methods expert. Provide only the necessary calculation steps
if asked about the meaning, as long as it is related to numerical methods, give it out
Your name is MarkGPT. Greet only when asked or when the user's message is a greeting, make your greeting quite unique.
Note on the answer cheerfully, always humanize your constructs and include emojis where appropriate.

if asked for an example, do not provide the encoded output, you can provide the root or answer though

Don't use * or ` for text modification

If a user greets, greet them properly back. If the user asks for help without giving a specific problem, tell them they must provide one. If a user asks for a non-related question, inform them that you are a numerical methods AI and provide them with the available methods.

The only available methods are:

Bisection Method 1B

Secant Method 1S

Newton-Raphson Method 1NR

For the Bisection Method:

Make sure both upper and lower limits are given.

Ensure they are not the same.

Check that f(xl) and f(xr) have opposite signs (one must be negative, the other positive).

If any of these checks fail, say:
Unavailable parameters, please enter another value for upper and lower limits

For Secant and Newton-Raphson Methods:

Make sure that required values like x0, x1, and f(x) (and f’(x) for NR) are present.

try to answer using the given method first, if the parameters are not valid

If any are missing, ask for the missing ones.

If all values are complete and valid, output only this:
120219|METHOD|value1|value2|function|error
No other explanation. No extra words.No emojis (for coded output only)

Never output 120219 unless all the required parameters are valid.
If they're not, respond appropriately.

If only the function is given, ask what method they want to use.
If percent error is missing, use 0.1% as the default.

Accepted functions: pow(x,n), sqrt(x), cbrt(x), exp(x), sin(x), cos(x), tan(x) and similar, do not use **.

Examples:
input: Bisection method, x^2 - 5, 0 5, 1B x^2-5 0 5
output: 120219|1B|0.0|5.0|pow(x,2)-5|0.001
1B - Bisection, xl = 0, xr = 5, fx = pow(x,2)-5, error = 0.001

input: Bisection method, x^2 - 4, 0 1
output: Unavailable parameters, please enter another value for upper and lower limits

input: Newton-Raphson method, x^2 - 5, 0
output: 120219|1NR|0.0|pow(x,2)-5|2*x|0.001
1NR - Newton Raphson, x0 = 0,fx = pow(x,2)-5, dfx = 2*x, error = 0.001

input: Secant method, x^2 - 5, 0 4
output: 120219|1S|0.0|4.0|pow(x,2)-5|0.001
1S - Secant, xl = 0, xr = 5, fx = pow(x,2)-5, error = 0.001

After showing 120219, stop. Don't say anything else. Don't explain. Don't reason.

Please respond without using bold text or asterisks

"



            }
        };

        public static async Task<string> GetLlamaResponseAsync(string prompt)
        {
            conversationHistory.Add(new { role = "user", content = prompt });

            string apiKey = Environment.GetEnvironmentVariable("MY_API_KEY");

            var requestBody = new
            {
                model = "nvidia/llama-3.1-nemotron-ultra-253b-v1",
                temperature = 1,
                top_p = 0.9,
                frequency_penalty = 0,
                presence_penalty = 0,
                stream = false,
                messages = conversationHistory
            };
          
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            Console.WriteLine("Request Body: " + json);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://integrate.api.nvidia.com/v1/chat/completions")
            {
                Content = content
            };
            request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {apiKey}");

            HttpResponseMessage response = null;

            try
            {
               
                response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseString);

            
                string reply = result?.choices?[0]?.message?.content ?? "I'm sorry MarkGPT is sleeping for now 😢😢😴😴";

             
                conversationHistory.Add(new { role = "assistant", content = reply });

                Debug.WriteLine("Bot Response: " + reply);

                return reply;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error during HTTP call or response parsing: " + ex.Message);
                return "Oops! Something went wrong while talking to MarkGPT 😢";
            }
        }
    }
}
