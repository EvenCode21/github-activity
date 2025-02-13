using System.Text.Json;
using System.Net.Http.Headers;

namespace github_activity;

class Program
{

    static void Main(string[] args)
    {
        HttpClient client = new HttpClient();

        client.BaseAddress = new Uri($"https://api.github.com/users/{args[0]}/");

        var events = GetEvents(client).Result;

        PrintEvents(events);

    }

    static async Task<Event[]> GetEvents(HttpClient client)
    {
        // headers required for the github api
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

        HttpResponseMessage responseBody = client.GetAsync("events").Result;

        // throw an error if the status code is not successful
        responseBody.EnsureSuccessStatusCode();

        var response = await responseBody.Content.ReadAsStringAsync();


        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        var root = JsonSerializer.Deserialize<Event[]>(response, options);

        return root;
    }

    static void PrintEvents(Event[] events)
    {
        foreach (Event e in events)
        {
            Console.WriteLine(e);
        }
    }
}

