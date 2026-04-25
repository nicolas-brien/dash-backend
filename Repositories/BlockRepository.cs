using Microsoft.EntityFrameworkCore;

using DashBackend.Data;
using DashBackend.Models;

namespace DashBackend.Repositories
{
    public class BlockRepository : IBlockRepository
    {
        private readonly AppDbContext _db;

        public BlockRepository(AppDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Block> GetBlocks(Guid dashId)
        {
            return _db.Blocks.Where(b => b.DashId == dashId).ToList();
        }

        public void Update(Block block)
        {
            var local = _db.Set<Block>().Local.FirstOrDefault(b => b.Id == block.Id);
            if (local != null)
            {
                _db.Entry(local).CurrentValues.SetValues(block);
                _db.Entry(local).State = EntityState.Modified;
            }
            else
            {
                _db.Blocks.Update(block);
            }
        }

        public void Upsert(Block block)
        {
            var local = _db.Set<Block>().Local.FirstOrDefault(b => b.Id == block.Id);
            if (local != null)
            {
                _db.Entry(local).CurrentValues.SetValues(block);
                _db.Entry(local).State = EntityState.Modified;
            }
            else
            {
                if (_db.Set<Block>().Any(b => b.Id == block.Id))
                {
                    _db.Blocks.Update(block);
                }
                else
                {
                    _db.Blocks.Add(block);
                }
            }
        }

        public void Delete(Block block)
        {
            _db.Blocks.Remove(block);
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }
    }
}