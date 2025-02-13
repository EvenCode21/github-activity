using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace github_activity
{
    public record Payload
    {
        string? Action { get; set; }

    }
    public record Repository(int Id, string Name, string Url);

    public record Actor(int id, string login);

    public record Event(string id, string type, Actor actor,Repository repo, Payload payload);
}
