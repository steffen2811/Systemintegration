using System.Text.Json;

namespace Dashboard.Data
{
    public class DataService
    {
        public static async Task<ReturnData> GetDataAsync()
        {
            HttpClient client = new();

            var stream = client.GetStreamAsync("https://localhost:7269/api/GetDashboardData");
            var result = await JsonSerializer.DeserializeAsync<ReturnData>(await stream);

            return result;
        }
    }
}