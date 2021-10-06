using System;
using LinqToDB.Mapping;

namespace DBMigrator.LegacyRepository
{
    [Table("userinfo")]
    public class LegacyUserInfoEntity
    {
        [PrimaryKey]
        [Column("Id")]
        public Guid Id { get; set; }

        [Column("firstName")]
        public string FirstName { get; set; }

        [Column("secondName")]
        public string SecondName { get; set; }

        [Column("lastName")]
        public string LastName { get; set; }

        [Column("role")]
        public UserRole Role { get; set; }

        [Column("registeredAt")]
        public long RegisteredAt { get; set; }
    }
}