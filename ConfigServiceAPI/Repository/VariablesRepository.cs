using ConfigServiceAPI.Commons;
using ConfigServiceAPI.DTOs;
using ConfigServiceAPI.Models;
using ConfigServiceAPI.Persistance;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ConfigServiceAPI.Repository
{
    public class VariablesRepository
    {
        private readonly ServiceConfigAPIDbContext _dbContext;

        public VariablesRepository(ServiceConfigAPIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CreateVariablesAsync(Variables variable)
        {
            await _dbContext.Variables.AddAsync(variable);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsVariableNameAsync(string name)
        {
            return await _dbContext.Enviroment.AnyAsync(e => e.name.ToLower() == name.ToLower());
        }

        public async Task<(List<VariableDTO> variables, int totalItems)> GetVariablesByEnviromentAsync(Guid idEnviroment, int page, int pageSize)
        {

            var query = _dbContext.Variables
                .Where(v => v.EnviromentId == idEnviroment)
                .OrderBy(v => v.name);

            var totalItems = await query.CountAsync();

            var variables = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new VariableDTO
                {
                    name = v.name,
                    value = v.value,
                    description = v.description,
                    isSensitive = v.isSensitive,
                    createdAt = v.createdAt,
                    updatedAt = v.updatedAt
                })
                .ToListAsync();

            return (variables, totalItems);
        }

        public async Task<Variables?> GetVariableAsync(string name, Guid idEnviroment)
        {
            return await _dbContext.Variables.FirstOrDefaultAsync(e => e.name == name && e.EnviromentId == idEnviroment);
        }

        public async Task<bool> UpdateVariableAsync(string env_name, string var_name, Variables updatedVariable)
        {
            var existing = await _dbContext.Variables.FirstOrDefaultAsync(e => e.name == var_name);

            if (existing == null) return false;

            existing.name = updatedVariable.name;
            existing.description = updatedVariable.description;
            existing.value = updatedVariable.value;
            existing.isSensitive = updatedVariable.isSensitive;
            existing.updatedAt = DateTimeOffset.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVariableAsync(string env_name, string var_name)
        {
            var var = await _dbContext.Variables.FirstOrDefaultAsync(e => e.name == var_name);
            if (var == null) return false;

            _dbContext.Variables.Remove(var);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Dictionary<string, string>> GenerateMassConsuptiomJson(Guid enviromentId)
        {
            var variables = await _dbContext.Variables
                .Where(v => v.EnviromentId == enviromentId)
                .Select(v => new { v.name, v.value })
                .ToListAsync();

            return variables.ToDictionary(v => v.name, v => v.value);
        }


        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

    }
}
