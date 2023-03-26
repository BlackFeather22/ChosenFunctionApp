using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;

namespace ChosenFunctionApp;

public static class FirstFunction
{
    [FunctionName("FirstFunction")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string name = req.Query["name"];

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        name = name ?? data?.name;

        return name != null
            ? (ActionResult)new OkObjectResult($"Hello, {name}")
            : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        
    }
    
}

public static class DatabaseFunction
{
    [FunctionName("DatabaseFunction")]
    public static async Task<HttpResponseMessage> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestMessage req)
    {
        // Replace with the URL of your API and the name of the table
        string tableName = "MyTable";
        string apiUrl = $"https://xchosen.database.windows.net/entries?table={tableName}&top=5";

        // Make the HTTP GET request to the API
        using (var client = new HttpClient())
        {
            var response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                // Parse the response and extract the data
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<Entry>>(content);

                // Return the data as a JSON response
                return req.CreateResponse(HttpStatusCode.OK, data);
            }
            else
            {
                // Return an error response if the API call failed
                return req.CreateErrorResponse(response.StatusCode, response.ReasonPhrase);
            }
        }
    }

    public class Entry
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    
}

