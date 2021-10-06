using System;
using System.Linq;
using System.Threading.Tasks;

namespace DBMigrator.LegacyRepository
{
    public class LegacyUserInfoRepository : IUserInfoRepository
    {
        private readonly DbCluster _dbCluster;

        public LegacyUserInfoRepository(DbCluster dbCluster)
        {
            _dbCluster = dbCluster;
        }

        public async Task InsertAsync(UserInfo userInfo)
        {
            var entity = new LegacyUserInfoEntity
            {
                Id = userInfo.Id,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                SecondName = userInfo.SecondName,
                RegisteredAt = userInfo.RegisteredAt,
                Role = userInfo.Role
            };
            await _dbCluster.InsertAsync<LegacyUserInfoEntity>(entity).ConfigureAwait(false);
        }

        public async Task DeleteAsync(Guid userId)
        {
            await _dbCluster.DeleteAsync(userId);
        }

        public async Task<UserInfo[]> GetAllAsync(int offset, int batchSize)
        {
            var userInfos = await _dbCluster
                .GetAllAsync<LegacyUserInfoEntity, long>(offset, batchSize, x => x.RegisteredAt)
                .ConfigureAwait(false);

            return userInfos
                .Select(x => x.ToUserInfo())
                .ToArray();
        }
        
        public async Task<UserInfo[]> GetAllAsync()
        {
            var userInfos = await _dbCluster
                .GetAllAsync<LegacyUserInfoEntity>()
                .ConfigureAwait(false);

            return userInfos
                .Select(x => x.ToUserInfo())
                .ToArray();
        }

        public async Task<UserInfo> FindAsync(Guid userId)
        {
            var entity = await _dbCluster
                .FindAsync<LegacyUserInfoEntity>(user => user.Id == userId)
                .ConfigureAwait(false);

            return entity.ToUserInfo();
        }
    }
}