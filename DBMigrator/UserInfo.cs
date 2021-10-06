using System;

namespace DBMigrator
{
    public class UserInfo
    {
        public UserInfo(Guid id, string firstName, string secondName, string lastName, UserRole role, long registeredAt)
        {
            Id = id;
            FirstName = firstName;
            SecondName = secondName;
            LastName = lastName;
            Role = role;
            RegisteredAt = registeredAt;
        }
        
        public Guid Id { get; }

        public string FirstName { get; }

        public string SecondName { get; }

        public string LastName { get; }

        public UserRole Role { get; }
        
        public long RegisteredAt { get; }
    }
}