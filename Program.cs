using System.Text.Json;
using System.Net.Http.Headers;
using github_activity.Events;

namespace github_activity;

class Program
{
    enum ExitCodes
    {
        Succes = 0,
        UsernameMissing,
        ApiError,
        NoEvents
    }
    static int Main(string[] args)
    {
        HttpClient client = new HttpClient();

        if (args.Length == 0)
        {
            Console.WriteLine("Usage: github-activity <username>");
            return (int) ExitCodes.UsernameMissing;
        }

        client.BaseAddress = new Uri($"https://api.github.com/users/{args[0]}/");
        
        if (client.DefaultRequestHeaders.UserAgent.TryParseAdd(args[0]))
        {
            var events = GetData(client,"events").Result;
            
            if (events.Length == 0)
            {
                Console.WriteLine("No activities found for this user");
                return (int) ExitCodes.NoEvents;
            }
            
            PrintEvents(events);
            return (int) ExitCodes.Succes;
        }
        else
        {
            return (int) ExitCodes.ApiError;
        }
    }

    static async Task<Event[]> GetData(HttpClient client,string endPoint)
    {
        // headers required for the github api
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

        HttpResponseMessage responseBody = client.GetAsync(endPoint).Result.EnsureSuccessStatusCode();
        
        // throw an error if the status code is not successful
        
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
            switch (e.Type)
            {
                case "CreateEvent":
                    if (e.payload.Ref_type == "repository")
                    {
                        Console.WriteLine($"- Created a new repository: {e.Repo.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"- Created a new {e.payload.Ref_type} in {e.Repo.Name}");
                    }
                    break;
                case "DeleteEvent":
                    Console.WriteLine($"- Deleted a {e.payload.Ref_type} in {e.Repo.Name}");
                    break;
                case "ForkEvent":
                    Console.WriteLine($"- Forked {e.Repo.Name}");
                    break;
                case "CommitCommentEvent":
                    Console.WriteLine($"- Created a Commit comment in {e.Repo.Name}");
                    break;
                case "GollumEvent":
                    break;
                case "IssuesEvent":
                    if (e.payload.Action == "opened")
                    {
                        Console.WriteLine($"- Opened a new issue in {e.Repo.Name}");
                    }
                    else if (e.payload.Action == "edited")
                    {
                        Console.WriteLine($"- Edited an issue in {e.Repo.Name}");
                    }
                    else if (e.payload.Action == "closed")
                    {
                        Console.WriteLine($"- Closed an issue in {e.Repo.Name}");
                    }
                    break;
                case "PublicEvent":
                    Console.WriteLine($"- Made public {e.Repo.Name}");
                    break;
                case "PullRequestEvent":
                    if (e.payload.Action == "opened")
                    {
                        Console.WriteLine($"- Pull requested in {e.Repo.Name}");
                    }
                    break;
                case "PushEvent":
                    Console.WriteLine($"- Pushed {e.payload.Size} commits to {e.Repo.Name}");
                    break;
                case "ReleaseEvent":
                    Console.WriteLine($"- Published in {e.Repo.Name}");
                    break;
                case "SponsorshipEvent":
                    break;
                case "WatchEvent":
                    Console.WriteLine($"- Starred {e.Repo.Name}");
                    break;

            }
        }
    }

}

