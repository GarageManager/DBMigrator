using System;
using LinqToDB.Mapping;

namespace DBMigrator.Repository
{
    [Table("user_info")]
    public class UserInfoEntity
    {
        [PrimaryKey]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("second_name")]
        public string SecondName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("role")]
        public UserRole Role { get; set; }

        [Column("registered_at")]
        public long RegisteredAt { get; set; }
    }
}