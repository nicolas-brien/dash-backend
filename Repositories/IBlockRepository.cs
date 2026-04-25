using DashBackend.Models;

namespace DashBackend.Repositories
{
    public interface IBlockRepository
    {
        IEnumerable<Block> GetBlocks(Guid dashId);

        void Update(Block block);
        void Upsert(Block block);
        void Delete(Block block);
        void SaveChanges();
    }
}
