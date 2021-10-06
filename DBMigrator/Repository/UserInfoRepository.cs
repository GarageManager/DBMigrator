using System;
using System.Linq;
using System.Threading.Tasks;

namespace DBMigrator.Repository
{
    public class UserInfoRepository : IUserInfoRepository
    {
        private readonly DbCluster _dbCluster;

        public UserInfoRepository(DbCluster dbCluster)
        {
            _dbCluster = dbCluster;
        }

        public Task InsertAsync(UserInfo userInfo)
        {
            var entity = new UserInfoEntity
            {
                Id = userInfo.Id,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                SecondName = userInfo.SecondName,
                RegisteredAt = userInfo.RegisteredAt,
                Role = userInfo.Role
            };
            return _dbCluster.InsertAsync<UserInfoEntity>(entity);
        }

        public async Task DeleteAsync(Guid userId)
        {
            await _dbCluster.DeleteAsync(userId).ConfigureAwait(false);
        }

        public async Task<UserInfo[]> GetAllAsync(int offset, int batchSize)
        {
            var userInfos = await _dbCluster
                .GetAllAsync<UserInfoEntity, long>(offset, batchSize, x => x.RegisteredAt)
                .ConfigureAwait(false);

            return userInfos
                .Select(x => x.ToUserInfo())
                .ToArray();
        }

        public async Task<UserInfo[]> GetAllAsync()
        {
            var userInfos = await _dbCluster
                .GetAllAsync<UserInfoEntity>()
                .ConfigureAwait(false);

            return userInfos
                .Select(x => x.ToUserInfo())
                .ToArray();
        }

        public async Task<UserInfo> FindAsync(Guid userId)
        {
            var entity = await _dbCluster
                .FindAsync<UserInfoEntity>(user => user.Id == userId)
                .ConfigureAwait(false);

            return entity.ToUserInfo();
        }
    }
}