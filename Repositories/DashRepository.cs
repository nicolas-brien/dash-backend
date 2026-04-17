using Microsoft.EntityFrameworkCore;

using DashBackend.Data;
using DashBackend.Models;

namespace DashBackend.Repositories
{
    public class DashRepository : IDashRepository
    {
        private readonly AppDbContext _db;

        public DashRepository(AppDbContext db)
        {
            _db = db;
        }

        public Dash? GetById(Guid id)
        {
            return _db.Dashes.Find(id);
        }

        public IEnumerable<Dash> GetMyDashes(Guid userId)
        {
            return _db.Dashes.AsNoTracking().Where(d => d.UserId == userId).ToList();
        }

        public void Add(Dash dash)
        {
            _db.Dashes.Add(dash);
        }
        
        public void Update(Dash dash)
        {
            // If an entity with the same key is already tracked, update its values instead of calling Update to avoid tracking conflicts.
            var local = _db.Set<Dash>().Local.FirstOrDefault(d => d.Id == dash.Id);
            if (local != null)
            {
                _db.Entry(local).CurrentValues.SetValues(dash);
                _db.Entry(local).State = EntityState.Modified;
            }
            else
            {
                _db.Dashes.Update(dash);
            }
        }

        public void Delete(Dash dash)
        {
            _db.Dashes.Remove(dash);
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }
    }
}