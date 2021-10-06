using System;
using System.Threading.Tasks;

namespace DBMigrator
{
    public interface IUserInfoRepository
    {

        public Task InsertAsync(UserInfo userInfo);

        public Task DeleteAsync(Guid userId);

        public Task<UserInfo[]> GetAllAsync();

        public Task<UserInfo[]> GetAllAsync(int offset, int batchSize);

        public Task<UserInfo> FindAsync(Guid userId);
    }
}