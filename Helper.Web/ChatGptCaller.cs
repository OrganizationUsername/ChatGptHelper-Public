using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Helper.Core.jsonModels;

namespace Helper.Web;

public static class ChatGptCaller
{
    
    public static async Task<EmbeddingData?> GetEmbed(string apiKey, string question)
    {
        var apiUrl = "https://api.openai.com/v1/embeddings";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var modelToRun = "text-embedding-ada-002";

        var requestBody = new
        {
            input = question,
            model = modelToRun,
        };

        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(apiUrl, content);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        try
        {
            var thing = JsonSerializer.Deserialize<EmbeddingResponse>(jsonResponse);
            return thing?.data.FirstOrDefault();
        }
        catch (Exception) { return null; }
    }
    
    public static async Task<int> GetEmbedCost(string apiKey, string question)
    {
        var apiUrl = "https://api.openai.com/v1/embeddings";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var modelToRun = "text-embedding-ada-002";

        var requestBody = new
        {
            input = question,
            model = modelToRun,
        };

        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(apiUrl, content);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        try
        {
            var thing = JsonSerializer.Deserialize<EmbeddingResponse>(jsonResponse);
            return thing != null ? thing.usage.total_tokens : 0;
        }
        catch (Exception) { return 0; }
    }

    public static async Task<ImageResponse?> ImageQuery(
        string apiKey,
        string question,
        int numberOfImages)
    {
        var apiUrl = "https://api.openai.com/v1/images/generations";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var requestBody = new
        {
            prompt = $"{question}",
            n = numberOfImages,
        };
        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(apiUrl, content);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        try
        {
            var thing = JsonSerializer.Deserialize<ImageResponse>(jsonResponse);
            return thing;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to deserialize: {ex.Message}");
            return null;
        }
    }

    public static async Task<QueryResponse?> Query(
        string apiKey,
        string question,
        string modelToRun,
        bool deterministic = true)
    {
        var apiUrl = "https://api.openai.com/v1/completions";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var desiredAnswerCount = deterministic ? 1 : 5;

        var requestBody = new
        {
            prompt = $"{question}\nAnswer:",
            max_tokens = 2000,
            n = desiredAnswerCount,
            stop = "Answer:",
            temperature = deterministic ? 0 : 0.75,
            stream = false,
            model = modelToRun,
        };

        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(apiUrl, content);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        try { return JsonSerializer.Deserialize<QueryResponse>(jsonResponse); }
        catch (Exception ex) { Console.WriteLine($"Failed to deserialize: {ex.Message}"); }

        return null;
    }
}