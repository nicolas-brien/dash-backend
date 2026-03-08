using DashBackend.Models;

namespace DashBackend.Repositories
{
    public interface IUserRepository
    {
        User? GetById(Guid id);
        User? GetByUsernameOrEmail(string value);

        IEnumerable<User> GetAll();

        void Add(User user);
        void Update(User user);
        void Delete(User user);
        void SaveChanges();
    }
}
