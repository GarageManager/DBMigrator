using System;
using System.Threading.Tasks;
using DBMigrator.MariaToPostgresMigration;

namespace DBMigrator
{
    public class MigrationRepository
    {
        private readonly MariaToPostgresMigrator _mariaToPostgresMigrator;
        private readonly IUserInfoRepository _userInfoMariaRepository;
        private readonly IUserInfoRepository _userInfoPostgresRepository;

        public MigrationRepository(
            IUserInfoRepository userInfoMariaRepository,
            IUserInfoRepository userInfoPostgresRepository,
            MariaToPostgresMigrator mariaToPostgresMigrator)
        {
            _userInfoMariaRepository = userInfoMariaRepository;
            _userInfoPostgresRepository = userInfoPostgresRepository;
            _mariaToPostgresMigrator = mariaToPostgresMigrator;
        }

        public Task SaveAsync(UserInfo user)
        {
            return _mariaToPostgresMigrator.PerformWriteOperationAsync(
                () => _userInfoMariaRepository.InsertAsync(user),
                () => ReplicateUpdatedRuleToPostgresAsync(user.Id),
                () => _userInfoPostgresRepository.InsertAsync(user),
                () => ReplicateUpdatedRuleToMariaAsync(user.Id)
            );
        }

        public Task DeleteAsync(Guid userId)
        {
            return _mariaToPostgresMigrator.PerformWriteOperationAsync(
                () => _userInfoMariaRepository.DeleteAsync(userId),
                () => ReplicateUpdatedRuleToPostgresAsync(userId),
                () => _userInfoPostgresRepository.DeleteAsync(userId),
                () => ReplicateUpdatedRuleToMariaAsync(userId)
            );
        }

        private async Task ReplicateUpdatedRuleToPostgresAsync(Guid userId)
        {
            var updatedRule = await _userInfoMariaRepository
                .FindAsync(userId)
                .ConfigureAwait(false);

            if (updatedRule != null)
            {
                await _userInfoPostgresRepository
                    .InsertAsync(updatedRule)
                    .ConfigureAwait(false);
            }
        }

        private async Task ReplicateUpdatedRuleToMariaAsync(Guid userId)
        {
            var updatedRule = await _userInfoPostgresRepository
                .FindAsync(userId)
                .ConfigureAwait(false);

            if (updatedRule != null)
            {
                await _userInfoMariaRepository
                    .InsertAsync(updatedRule)
                    .ConfigureAwait(false);
            }
        }

        public Task<UserInfo> FindRuleAsync(Guid userId)
        {
            return _mariaToPostgresMigrator.PerformReadOperationAsync(
                () => _userInfoMariaRepository.FindAsync(userId),
                () => _userInfoPostgresRepository.FindAsync(userId)
            );
        }

        public Task<UserInfo[]> GetAllRulesAsync()
        {
            return _mariaToPostgresMigrator.PerformReadOperationAsync(
                () => _userInfoMariaRepository.GetAllAsync(),
                () => _userInfoPostgresRepository.GetAllAsync()
            );
        }
    }
}