using System;

namespace github_activity.Events
{
    public class Event
    {
        public record Payload(string Action, string Ref_type, int Size);
        public record Repository(int Id, string Name, string Url);

        public record Actor(int id, string login);

        public string Id { get; set; }
        public string Type { get; set; }

        public Actor actor { get; set; }

        public Repository Repo { get; set; }

        public Payload payload { get; set; }
    }
}
