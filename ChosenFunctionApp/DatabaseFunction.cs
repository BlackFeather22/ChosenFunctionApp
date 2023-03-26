using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ChosenFunctionApp;

public static class DatabaseFunction
{
    [FunctionName("DatabaseFunction")]
    public static async Task<string> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
    {
        // Replace with your connection string
        string connectionString = "Server=tcp:xchosen.database.windows.net,1433;Initial Catalog=blazorDatabase;Persist Security Info=False;User ID=CloudSA326ab916;Password=Matabase@1324;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        // Replace with your table name and query
        string tableName = "MyTable";
        string query = $"SELECT * FROM {tableName}";

        // Connect to the database and execute the query
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    var data = new List<Entry>();
                    while (await reader.ReadAsync())
                    {
                        data.Add(new Entry
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }

                    // Return the data as a JSON response
                    return JsonConvert.SerializeObject(data);
                }
            }
        }
    }

    public class Entry
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
    
