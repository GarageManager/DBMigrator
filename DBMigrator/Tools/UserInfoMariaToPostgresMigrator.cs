using System.Threading.Tasks;
using DBMigrator.MariaToPostgresMigration;
using NLog;

namespace DBMigrator.Tools
{
	internal sealed class UserInfoMariaToPostgresMigrator : DatabaseMigrationToolBase<UserInfo>
	{
		private readonly IUserInfoRepository _userInfoMariaRepository;
		private readonly IUserInfoRepository _userInfoPostgresRepository;

		public UserInfoMariaToPostgresMigrator(
			IUserInfoRepository userInfoMariaRepository,
			IUserInfoRepository userInfoPostgresRepository,
			ILogger logger,
			int batchSize = 20,
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
			return _userInfoMariaRepository.GetAllAsync(offset, batchSize);
		}
		protected override Task<UserInfo> FindDestinationDatabaseEntityAsync(UserInfo sourceDatabaseEntity)
		{
			return _userInfoPostgresRepository.FindAsync(sourceDatabaseEntity.Id);
		}

		protected override Task MigrateSourceDatabaseEntityToDestinationDatabaseAsync(UserInfo sourceDatabaseEntity)
		{
			return _userInfoPostgresRepository.InsertAsync(sourceDatabaseEntity);
		}

		protected override bool AreEntitiesEqual(UserInfo sourceUserInfo, UserInfo destinationUserInfo)
		{
			return sourceUserInfo.Id == destinationUserInfo.Id
			       && sourceUserInfo.FirstName.Equals(destinationUserInfo.FirstName)
			       && sourceUserInfo.LastName.Equals(destinationUserInfo.LastName)
			       && sourceUserInfo.SecondName.Equals(destinationUserInfo.SecondName)
			       && sourceUserInfo.Role == destinationUserInfo.Role;
		}
	}
}
