namespace DBMigrator.MariaToPostgresMigration
{
    public enum MariaToPostgresMigrationState
    {
        ReadFromMariaWriteToMaria = 0,
        ReadFromMariaWriteToMariaReplicateToPostgres = 10,
        ReadFromPostgresWriteToPostgresReplicateToMaria = 20,
        ReadFromPostgresWriteToPostgres = 30
    }
}