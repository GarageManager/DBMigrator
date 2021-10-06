using System;
using System.IO;
using DBMigrator.Settings;
using Newtonsoft.Json;

namespace DBMigrator.MariaToPostgresMigration
{
    public static class MariaToPostgresMigrationSettings
    {
        public static readonly string DirectoryPath = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));

        private static MigrationSettings _settings;

        public static MariaToPostgresMigrationState GetMigrationState()
        {
            _settings = JsonConvert.DeserializeObject<MigrationSettings>(
                File.ReadAllText(Path.Combine(DirectoryPath, @"Settings\Settings.txt")));

            if (_settings == null)
                return MariaToPostgresMigrationState.ReadFromMariaWriteToMaria;

            switch (_settings.State)
            {
                case MariaToPostgresMigrationState.ReadFromMariaWriteToMaria:
                case MariaToPostgresMigrationState.ReadFromPostgresWriteToPostgres:
                case MariaToPostgresMigrationState.ReadFromMariaWriteToMariaReplicateToPostgres:
                case MariaToPostgresMigrationState.ReadFromPostgresWriteToPostgresReplicateToMaria:
                    return _settings.State;
                default:
                    return MariaToPostgresMigrationState.ReadFromMariaWriteToMaria;
            }
        }

        public static void UpdateOffset(int newOffset)
        {
            _settings = new MigrationSettings(_settings.State, newOffset);

            File.WriteAllText(
                Path.Combine(DirectoryPath, @"Settings\Settings.txt"),
                JsonConvert.SerializeObject(_settings)
            );
        }

        public static int GetOffset()
        {
            _settings = JsonConvert.DeserializeObject<MigrationSettings>(
                File.ReadAllText(Path.Combine(DirectoryPath, @"Settings\Settings.txt")));

            if (_settings == null)
            {
                throw new NullReferenceException("Can't read settings");
            }

            return _settings.Offset;
        }
    }
}