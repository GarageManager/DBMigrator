using System;
using System.Threading.Tasks;
using NLog;

namespace DBMigrator.MariaToPostgresMigration
{
	public sealed class MariaToPostgresMigrator
	{
		private readonly ILogger _logger;
		public MariaToPostgresMigrator(ILogger logger)
		{
			_logger = logger;
		}
		
		public async Task PerformWriteOperationAsync(
			Func<Task> writeToMariaAction,
			Func<Task> replicateToPostgresAction,
			Func<Task> writeToPostgresAction,
			Func<Task> replicateToMariaAction)
		{
			var migrationState = MariaToPostgresMigrationSettings.GetMigrationState();

			switch (migrationState)
			{
				case MariaToPostgresMigrationState.ReadFromMariaWriteToMaria:
				case MariaToPostgresMigrationState.ReadFromMariaWriteToMariaReplicateToPostgres:
				{
					await writeToMariaAction().ConfigureAwait(false);

					if (migrationState == MariaToPostgresMigrationState.ReadFromMariaWriteToMariaReplicateToPostgres)
					{
						try
						{
							await replicateToPostgresAction().ConfigureAwait(false);
						}
						catch (Exception exception)
						{
							_logger.Error(exception, "Error on replication to Postgres");
						}
					}

					break;
				}
				case MariaToPostgresMigrationState.ReadFromPostgresWriteToPostgresReplicateToMaria:
				case MariaToPostgresMigrationState.ReadFromPostgresWriteToPostgres:
				{
					await writeToPostgresAction().ConfigureAwait(false);

					if (migrationState == MariaToPostgresMigrationState.ReadFromPostgresWriteToPostgresReplicateToMaria)
					{
						try
						{
							await replicateToMariaAction().ConfigureAwait(false);
						}
						catch (Exception exception)
						{
							_logger.Error(exception, "Error on replication to Maria");
						}
					}

					break;
				}
				default:
					throw new ArgumentOutOfRangeException(nameof(migrationState), migrationState, null);
			}
		}

		public Task<TResult> PerformReadOperationAsync<TResult>(
			Func<Task<TResult>> readFromMariaAction,
			Func<Task<TResult>> readFromPostgresAction)
		{
			var migrationState = MariaToPostgresMigrationSettings.GetMigrationState();

			switch (migrationState)
			{
				case MariaToPostgresMigrationState.ReadFromMariaWriteToMaria:
				case MariaToPostgresMigrationState.ReadFromMariaWriteToMariaReplicateToPostgres:
					return readFromMariaAction();
				case MariaToPostgresMigrationState.ReadFromPostgresWriteToPostgresReplicateToMaria:
				case MariaToPostgresMigrationState.ReadFromPostgresWriteToPostgres:
					return readFromPostgresAction();
				default:
					throw new ArgumentOutOfRangeException(nameof(migrationState), migrationState, null);
			}
		}
	}
}
