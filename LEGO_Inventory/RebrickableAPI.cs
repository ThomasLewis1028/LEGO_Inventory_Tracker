using System.Text.Json.Nodes;

namespace LEGO_Inventory;


public class RebrickableApi
{
    private const string BaseUrl = "https://rebrickable.com/api/v3/lego/";
    
    
    public async Task<JsonObject?> GetSetInfo(HttpClient client, string? setId)
    {
        string url = $"{BaseUrl}sets/{setId}/?page_size=1000000&";

        return await SendQuery(client, url);
    }
    
    
    public async Task<JsonObject?> GetSetParts(HttpClient client, string setId)
    {
        string url = $"{BaseUrl}sets/{setId}/parts/?page_size=1000000&inc_minifig_parts=1&";

        return await SendQuery(client, url);
    }
    
    
    public async Task<JsonObject?> GetPartInfo(HttpClient client, string partNum)
    {
        string url = $"{BaseUrl}parts/{partNum}/?";

        return await SendQuery(client, url);
    }

    private async Task<JsonObject?> SendQuery(HttpClient client, string url)
    {
        try
        {
            string? apiKey = Environment.GetEnvironmentVariable("LEGO_API_KEY");
                
            HttpResponseMessage response;
            Console.WriteLine($"{DateTime.Now} Requesting...");
            Uri uri = new Uri($"{url}key={apiKey}");
            response = await client.GetAsync(uri);
            Console.WriteLine($"{DateTime.Now} Response: {response}");
                

            if (response.IsSuccessStatusCode)
                return JsonNode.Parse(await response.Content.ReadAsStringAsync())?.AsObject();
                
            throw new Exception($"Rebrickable API returned status code {(int)response.StatusCode}.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            
            throw;
        }
    }
}