using DBMigrator.MariaToPostgresMigration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DBMigrator.Settings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MigrationSettings
    {
        public MigrationSettings(MariaToPostgresMigrationState state, int offset)
        {
            State = state;
            Offset = offset;
        }

        [JsonConstructor]
        private MigrationSettings()
        {
        }

        [JsonProperty("state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MariaToPostgresMigrationState State { get; private set; }

        [JsonProperty("offset")]
        public int Offset { get; private set; }
    }
}