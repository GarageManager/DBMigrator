using DBMigrator.LegacyRepository;
using DBMigrator.Repository;

namespace DBMigrator
{
    public static class UserInfoExtensions
    {
        public static UserInfoEntity ToUserInfoEntity(this UserInfo userInfo)
        {
            return new UserInfoEntity
            {
                Id = userInfo.Id,
                FirstName = userInfo.FirstName,
                SecondName = userInfo.SecondName,
                LastName = userInfo.LastName,
                Role = userInfo.Role
            };
        }

        public static UserInfoEntity ToLegacyUserInfoEntity(this UserInfo userInfo)
        {
            return new UserInfoEntity
            {
                Id = userInfo.Id,
                FirstName = userInfo.FirstName,
                SecondName = userInfo.SecondName,
                LastName = userInfo.LastName,
                Role = userInfo.Role
            };
        }

        public static UserInfo ToUserInfo(this UserInfoEntity userInfo)
        {
            return userInfo != null
                ? new UserInfo(
                    userInfo.Id,
                    userInfo.FirstName,
                    userInfo.SecondName,
                    userInfo.LastName,
                    userInfo.Role,
                    userInfo.RegisteredAt)
                : null;
        }

        public static UserInfo ToUserInfo(this LegacyUserInfoEntity userInfo)
        {
            return userInfo != null
                ? new UserInfo(
                    userInfo.Id,
                    userInfo.FirstName,
                    userInfo.SecondName,
                    userInfo.LastName,
                    userInfo.Role,
                    userInfo.RegisteredAt)
                : null;
        }
    }
}