using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DBMigrator.LegacyRepository;
using DBMigrator.MariaToPostgresMigration;
using DBMigrator.Repository;
using DBMigrator.Settings;
using DBMigrator.Tools;
using LinqToDB;
using Newtonsoft.Json;
using NLog;

namespace DBMigrator
{
    internal static class Program
    {
        private static ILogger logger = GetConsoleLogger();
        private static LegacyUserInfoRepository mariaRepository;
        private static UserInfoRepository postgresRepository;

        private static async Task Main()
        {

            var connectionsLocation = Path.Combine(
                MariaToPostgresMigrationSettings.DirectoryPath,
                @"Settings\ConnectionStrings.txt");
            var connections = JsonConvert.DeserializeObject<ConnectionStrings>(File.ReadAllText(connectionsLocation));

            if (connections == null)
            {
                logger.Error("Can't parse ConnectionStrings.txt");
                return;
            }

            var mariaCluster = new DbCluster(ProviderName.MySql, connections.Maria);
            mariaRepository = new LegacyUserInfoRepository(mariaCluster);

            var postgresCluster = new DbCluster(ProviderName.PostgreSQL, connections.Postgres);
            postgresRepository = new UserInfoRepository(postgresCluster);

            var migrationRepository = new MigrationRepository(
                mariaRepository,
                postgresRepository,
                new MariaToPostgresMigrator(logger));
            var imitationService = new RequestsImitationService(migrationRepository);

            new Thread(MigrateEntities).Start();

            while (true)
            {
                await imitationService.ImitateSaveAsync().ConfigureAwait(false);
                Thread.Sleep(100);
            }
        }

        private static void MigrateEntities()
        {
            new UserInfoMariaToPostgresMigrator(mariaRepository, postgresRepository, logger, dryRun: false).Run();
            new UserInfoMariaToPostgresMigrator(
                mariaRepository,
                postgresRepository,
                logger,
                dryRun: false,
                runMode:DatabaseMigrationToolBase<UserInfo>.DatabaseMigrationToolRunMode.Verification).Run();
        }
        private static Logger GetConsoleLogger()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logConsole = new NLog.Targets.ConsoleTarget("logconsole");
            config.AddRuleForAllLevels(logConsole);
            LogManager.Configuration = config;
            return LogManager.GetCurrentClassLogger();
        }
    }
}