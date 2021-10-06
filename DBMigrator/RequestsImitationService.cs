using System;
using System.Threading.Tasks;

namespace DBMigrator
{
    public class RequestsImitationService
    {
        private readonly MigrationRepository _migrationRepository;
        private readonly Random _random = new Random();

        public RequestsImitationService(MigrationRepository migrationRepository)
        {
            _migrationRepository = migrationRepository;
        }

        private readonly UserRole[] _roles =
            { UserRole.Default, UserRole.Default, UserRole.Default, UserRole.Admin, UserRole.PrivilegedUser };

        private readonly string[] _lastNames =
        {
            "Щукин", "Лаптев", "Богданов", "Абрамов", "Филиппов", "Григорьев", "Александров", "Иванов", "Баженов",
            "Черкасов", "Цветков", "Антипов", "Борисов", "Смирнов", "Николаев", "Евдокимов", "Журавлев", "Александров",
            "Старостин", "Воронов"
        };

        private readonly string[] _firstNames =
        {
            "Михаил", "Демид", "Александр", "Павел", "Платон", "Родион", "Демид", "Николай", "Богдан", "Тихон",
            "Кирилл", "Григорий", "Матвей", "Давид", "Михаил", "Кирилл", "Максим", "Даниил", "Михаил", "Егор",
        };

        private readonly string[] _secondNames =
        {
            "Тимофеевич", "Владимирович", "Русланович", "Евгеньевич", "Михайлович", "Степанович", "Ильич", "Львович",
            "Демидович", "Дмитриевич", "Александрович", "Денисович", "Даниилович", "Владимирович", "Константинович",
            "Богданович", "Алексеевич", "Алексеевич", "Александрович", "Даниилович",
        };

        public async Task ImitateSaveAsync()
        {
            await _migrationRepository.SaveAsync(GetUser()).ConfigureAwait(false);
        }

        private UserInfo GetUser()
        {
            return new UserInfo(
                Guid.NewGuid(),
                GetFirstName(),
                GetSecondName(),
                GetLastName(),
                GetRole(),
                DateTime.Now.Ticks);
        }

        private UserRole GetRole()
        {
            return _roles[_random.Next(_roles.Length)];
        }

        private string GetFirstName()
        {
            return _firstNames[_random.Next(_firstNames.Length)];
        }

        private string GetSecondName()
        {
            return _secondNames[_random.Next(_secondNames.Length)];
        }

        private string GetLastName()
        {
            return _lastNames[_random.Next(_lastNames.Length)];
        }
    }
}