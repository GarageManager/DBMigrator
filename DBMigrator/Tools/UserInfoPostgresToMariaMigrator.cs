using System.Threading.Tasks;
using DBMigrator.MariaToPostgresMigration;
using NLog;

namespace DBMigrator.Tools
{
	internal sealed class UserInfoPostgresToMariaMigrator : DatabaseMigrationToolBase<UserInfo>
	{
		private readonly IUserInfoRepository _userInfoMariaRepository;
		private readonly IUserInfoRepository _userInfoPostgresRepository;

		public UserInfoPostgresToMariaMigrator(
			IUserInfoRepository userInfoMariaRepository,
			IUserInfoRepository userInfoPostgresRepository,
			ILogger logger,
			int batchSize = 200,
			bool dryRun = true,
			DatabaseMigrationToolRunMode runMode = DatabaseMigrationToolRunMode.Migration)
			: base(
				logger,
				batchSize,
				dryRun,
				runMode)
		{
			_userInfoMariaRepository = userInfoMariaRepository;
			_userInfoPostgresRepository = userInfoPostgresRepository;
		}

		protected override Task<UserInfo[]> GetSourceDatabaseEntitiesAsync(int offset, int batchSize)
		{
			return _userInfoPostgresRepository.GetAllAsync(offset, batchSize);
		}

		protected override Task<UserInfo> FindDestinationDatabaseEntityAsync(UserInfo sourceDatabaseEntity)
		{
			return _userInfoMariaRepository.FindAsync(sourceDatabaseEntity.Id);
		}

		protected override Task MigrateSourceDatabaseEntityToDestinationDatabaseAsync(UserInfo sourceDatabaseEntity)
		{
			return _userInfoMariaRepository.InsertAsync(sourceDatabaseEntity);
		}

		protected override bool AreEntitiesEqual(UserInfo sourceDatabaseEntity, UserInfo destinationDatabaseEntity)
		{
			return sourceDatabaseEntity.Id == destinationDatabaseEntity.Id
			       && sourceDatabaseEntity.FirstName.Equals(destinationDatabaseEntity.FirstName)
			       && sourceDatabaseEntity.LastName.Equals(destinationDatabaseEntity.LastName)
			       && sourceDatabaseEntity.SecondName.Equals(destinationDatabaseEntity.SecondName)
			       && sourceDatabaseEntity.Role == destinationDatabaseEntity.Role;
		}
	}
}
