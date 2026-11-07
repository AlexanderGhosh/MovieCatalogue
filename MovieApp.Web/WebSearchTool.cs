using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace MovieApp.Web
{
    public class WebSearchTool(HttpClient http, string apiKey)
    {
        [Description("Searches the web and returns the top results with titles, URLs and content snippets.")]
        public async Task<string> SearchAsync(
            [Description("The search query")] string query,
            CancellationToken ct = default)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.tavily.com/search")
            {
                Content = JsonContent.Create(new TavilyRequest(query, MaxResults: 5))
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            using var response = await http.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TavilyResponse>(ct);
            var items = result?.Results ?? [];

            return string.Join("\n\n", items.Select(r => $"{r.Title}\n{r.Url}\n{r.Content}"));
        }

        private record TavilyRequest(
            [property: JsonPropertyName("query")] string Query,
            [property: JsonPropertyName("max_results")] int MaxResults);

        private record TavilyResponse(
            [property: JsonPropertyName("results")] List<TavilyResult>? Results);

        private record TavilyResult(
            [property: JsonPropertyName("title")] string Title,
            [property: JsonPropertyName("url")] string Url,
            [property: JsonPropertyName("content")] string Content);
    }
}