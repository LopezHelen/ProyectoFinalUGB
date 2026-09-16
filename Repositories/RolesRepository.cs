using Microsoft.EntityFrameworkCore;
using UGB.MVC.Aplicaciones.Seguras.Entities;
using UGB.MVC.Aplicaciones.Seguras.Interfaces;
using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.Repositories
{
    public class RolesRepository(StoreCTX ctx) : IRolesRepository
    {
        public async Task<roles?> GetByName(string name)
        {
            return await ctx.roles
                .FirstOrDefaultAsync(x => x.name == name);
        }

        public async Task AssignRoleToUser(int userId, int roleId)
        {
            bool roleAlreadyAssigned = await ctx.user_roles
                .AnyAsync(x => x.user_id == userId &&
                               x.role_id == roleId);

            if (roleAlreadyAssigned)
            {
                return;
            }

            ctx.user_roles.Add(new user_roles
            {
                user_id = userId,
                role_id = roleId
            });

            await ctx.SaveChangesAsync();
        }

        public async Task<IEnumerable<string>> GetRoleNamesByUserId(int userId)
        {
            return await ctx.user_roles
                .Where(x => x.user_id == userId)
                .Select(x => x.role.name)
                .ToListAsync();
        }
    }
}