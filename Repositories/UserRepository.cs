using Microsoft.EntityFrameworkCore;

using DashBackend.Data;
using DashBackend.Models;

namespace DashBackend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public User? GetById(Guid id)
        {
            return _db.Users.Find(id);
        }

        public User? GetByUsernameOrEmail(string value)
        {
            return _db.Users
                .FirstOrDefault(u =>
                    u.Username == value || u.Email == value
                );
        }

        public IEnumerable<User> GetAll()
        {
            return _db.Users.AsNoTracking().ToList();
        }

        public void Add(User user)
        {
            _db.Users.Add(user);
        }
        
        public void Update(User user)
        {
            // If an entity with the same key is already tracked, update its values instead of calling Update to avoid tracking conflicts.
            var local = _db.Set<User>().Local.FirstOrDefault(u => u.Id == user.Id);
            if (local != null)
            {
                _db.Entry(local).CurrentValues.SetValues(user);
                _db.Entry(local).State = EntityState.Modified;
            }
            else
            {
                _db.Users.Update(user);
            }
        }

        public void Delete(User user)
        {
            _db.Users.Remove(user);
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }
    }
}