using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;

namespace DBMigrator.MariaToPostgresMigration
{
    public abstract class DatabaseMigrationToolBase<TEntity> where TEntity : class
    {
        public enum DatabaseMigrationToolRunMode
        {
            Migration = 0,
            Verification = 1
        }

        private readonly ILogger _logger;
        private readonly int _batchSize;
        private readonly bool _dryRun;
        private readonly DatabaseMigrationToolRunMode _runMode;

        private readonly string _alreadyMigratedFilePath =
            Path.Combine(MariaToPostgresMigrationSettings.DirectoryPath, @"Tools\MigratedEntities\AlreadyMigrated.txt");

        private readonly string _migratedFilePath =
            Path.Combine(MariaToPostgresMigrationSettings.DirectoryPath, @"Tools\MigratedEntities\Migrated.txt");

        private readonly string _diffSourceFilePath =
            Path.Combine(MariaToPostgresMigrationSettings.DirectoryPath, @"Tools\MigratedEntities\DiffSource.txt");

        private readonly string _diffDestinationFilePath =
            Path.Combine(MariaToPostgresMigrationSettings.DirectoryPath, @"Tools\MigratedEntities\DiffDestination.txt");

        private readonly string _notExistDestinationFilePath =
            Path.Combine(MariaToPostgresMigrationSettings.DirectoryPath,
                @"Tools\MigratedEntities\NotExistDestination.txt");

        protected DatabaseMigrationToolBase(
            ILogger logger,
            int batchSize = 20,
            bool dryRun = true,
            DatabaseMigrationToolRunMode runMode = DatabaseMigrationToolRunMode.Migration)
        {
            _logger = logger;
            _batchSize = batchSize;
            _dryRun = dryRun;
            _runMode = runMode;
        }

        protected abstract Task<TEntity[]> GetSourceDatabaseEntitiesAsync(int offset, int batchSize);
        protected abstract Task<TEntity> FindDestinationDatabaseEntityAsync(TEntity sourceDatabaseEntity);
        protected abstract Task MigrateSourceDatabaseEntityToDestinationDatabaseAsync(TEntity sourceDatabaseEntity);
        protected abstract bool AreEntitiesEqual(TEntity sourceDatabaseEntity, TEntity destinationDatabaseEntity);

        public async void Run()
        {
            var offset = MariaToPostgresMigrationSettings.GetOffset();

            while (true)
            {
                _logger.Info($"Begin migration, dryRun={_dryRun} runMode={_runMode} offset={offset}");

                var entities = await GetSourceDatabaseEntitiesAsync(offset, _batchSize)
                    .ConfigureAwait(false);

                foreach (var entity in entities)
                {
                    await HandleSourceDatabaseEntityAsync(entity).ConfigureAwait(false);
                }

                _logger.Info($"End migration, offset={offset}");

                if (entities.Length < _batchSize)
                {
                    break;
                }

                offset += _batchSize;
                MariaToPostgresMigrationSettings.UpdateOffset(offset);
            }

            MariaToPostgresMigrationSettings.UpdateOffset(0);
        }

        private async Task HandleSourceDatabaseEntityAsync(TEntity sourceDatabaseEntity)
        {
            var destinationDatabaseEntity =
                await FindDestinationDatabaseEntityAsync(sourceDatabaseEntity).ConfigureAwait(false);

            switch (_runMode)
            {
                case DatabaseMigrationToolRunMode.Migration:
                    await HandleMigrationAsync(sourceDatabaseEntity, destinationDatabaseEntity).ConfigureAwait(false);
                    break;
                case DatabaseMigrationToolRunMode.Verification:
                    HandleVerification(sourceDatabaseEntity, destinationDatabaseEntity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_runMode), _runMode, null);
            }
        }

        private async Task HandleMigrationAsync(TEntity sourceDatabaseEntity, TEntity destinationDatabaseEntity)
        {
            if (destinationDatabaseEntity != null)
            {
                AppendToFile(_alreadyMigratedFilePath, JsonConvert.SerializeObject(sourceDatabaseEntity));
                return;
            }

            if (!_dryRun)
            {
                await MigrateSourceDatabaseEntityToDestinationDatabaseAsync(sourceDatabaseEntity)
                    .ConfigureAwait(false);
            }

            AppendToFile(_migratedFilePath, JsonConvert.SerializeObject(sourceDatabaseEntity));
        }

        private void HandleVerification(TEntity sourceDatabaseEntity, TEntity destinationDatabaseEntity)
        {
            if (destinationDatabaseEntity != null)
            {
                var entitiesAreEqual = AreEntitiesEqual(sourceDatabaseEntity, destinationDatabaseEntity);

                if (!entitiesAreEqual)
                {
                    AppendToFile(_diffSourceFilePath, JsonConvert.SerializeObject(sourceDatabaseEntity));
                    AppendToFile(_diffDestinationFilePath, JsonConvert.SerializeObject(destinationDatabaseEntity));
                }
            }
            else
            {
                AppendToFile(_notExistDestinationFilePath, JsonConvert.SerializeObject(sourceDatabaseEntity));
            }
        }

        private static void AppendToFile(string filePath, string line)
        {
            File.AppendAllLines(filePath, new[] {line});
        }
    }
}