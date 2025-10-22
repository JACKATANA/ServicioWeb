using System.Net;
using ConfigServiceAPI.Commons;
using ConfigServiceAPI.Models;
using ConfigServiceAPI.Persistance;
using Microsoft.EntityFrameworkCore;

namespace ConfigServiceAPI.Repository
{
    public class EnviromentRepository
    {
        private readonly ServiceConfigAPIDbContext _dbContext;

        public EnviromentRepository(ServiceConfigAPIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CreateEnviromentAsync(Enviroments enviroment)
        {
            await _dbContext.Enviroment.AddAsync(enviroment);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsEnviromentNameAsync(string name)
        {
            return await _dbContext.Enviroment.AnyAsync(e => e.name.ToLower() == name.ToLower());
        }

        public async Task<(List<Enviroments> Items, int TotalCount)> GetAllEnviromentsAsync(int pageNumber, int pageSize)
        {
            var query = _dbContext.Enviroment.AsNoTracking();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(e => e.name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Enviroments?> GetEnviromentAsync(string name)
        {
            return await _dbContext.Enviroment.FirstOrDefaultAsync(e => e.name == name);
        }

        public async Task<bool> UpdateEnviromentAsync(string env_name, Enviroments updatedEnviroment)
        {
            var existing = await _dbContext.Enviroment.FirstOrDefaultAsync(e => e.name == env_name);

            if (existing == null) return false;

            existing.name = updatedEnviroment.name;
            existing.description = updatedEnviroment.description;
            existing.updatedAt = DateTimeOffset.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEnviromentAsync(string env_name)
        {
            var env = await _dbContext.Enviroment.FirstOrDefaultAsync(e => e.name == env_name);
            if (env == null) return false;

            _dbContext.Enviroment.Remove(env);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Guid> GetEnviromentIdByNameAsync(string env_name)
        {
            var env = await _dbContext.Enviroment.Where(e => e.name == env_name).Select(e => (Guid)e.Id)
                .FirstOrDefaultAsync();

            return env;
        }


        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }








    }
}
