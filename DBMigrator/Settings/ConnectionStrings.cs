using Newtonsoft.Json;

namespace DBMigrator.Settings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ConnectionStrings
    {
        public ConnectionStrings(string maria, string postgres)
        {
            Maria = maria;
            Postgres = postgres;
        }

        [JsonConstructor]
        private ConnectionStrings()
        {
        }

        [JsonProperty("maria")]
        public string Maria { get; private set; }

        [JsonProperty("postgres")]
        public string Postgres { get; private set; }
    }
}