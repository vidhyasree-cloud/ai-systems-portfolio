using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Azure.AI.OpenAI; 

namespace Vidhya.Portfolio.Api
{
    public static class IncidentAgent
    {
        [FunctionName("AnalyzeIncident")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            // 1. Get the incident description from the Frontend request
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string userIncident = data?.incident;

            // 2. Define the Agentic System Prompt (The SRE Personality)
            string systemPrompt = @"You are a Senior Microsoft SRE Agent. 
            Analyze the provided incident and return: 
            1. Likely Root Cause. 
            2. A specific KQL query for investigation.
            3. A recommended MTTR reduction action.";

            // 3. Setup Azure OpenAI Client (Logic for recruiters to see)
            // In production, these would be pulled from Azure Key Vault
            var client = new OpenAIClient(new System.Uri("https://your-resource.openai.azure.com/"), new Azure.AzureKeyCredential("API_KEY_HERE"));
            
            var options = new ChatCompletionsOptions()
            {
                Messages = {
                    new ChatRequestSystemMessage(systemPrompt),
                    new ChatRequestUserMessage(userIncident)
                },
                Temperature = 0.7f
            };

            // 4. Return the AI-generated RCA to the website
            // var response = await client.GetChatCompletionsAsync("gpt-4", options);
            return new OkObjectResult("AI Analysis Complete. (C# Backend logic ready for deployment)");
        }
    }
}
