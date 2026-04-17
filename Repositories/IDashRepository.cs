using DashBackend.Models;

namespace DashBackend.Repositories
{
    public interface IDashRepository
    {
        Dash? GetById(Guid id);

        IEnumerable<Dash> GetMyDashes(Guid userId);

        void Add(Dash dash);
        void Update(Dash dash);
        void Delete(Dash dash);
        void SaveChanges();
    }
}
