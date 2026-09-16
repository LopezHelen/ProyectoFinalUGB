using Microsoft.EntityFrameworkCore;
using UGB.MVC.Aplicaciones.Seguras.Entities;
using UGB.MVC.Aplicaciones.Seguras.Interfaces;
using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.Repositories
{
    public class UsersRepository(StoreCTX ctx) : IUsersRepository
    {
        public async Task<users> Insert(users user)
        {
            ctx.users.Add(user);
            await ctx.SaveChangesAsync();
            return user;
        }

        public async Task<users?> GetByEmail(string email)
        {
            return await ctx.users.FirstOrDefaultAsync(x => x.email == email);
        }

        public async Task<users?> Get(int id)
        {
            return await ctx.users.FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<IEnumerable<users>> GetAll()
        {
            return await ctx.users.ToListAsync();
        }

        public async Task<users?> GetByConfirmationToken(Guid token)
        {
            return await ctx.users.FirstOrDefaultAsync(x => x.email_confirmation_token == token);
        }

        public async Task<users> Update(users user)
        {
            ctx.users.Update(user);
            await ctx.SaveChangesAsync();
            return user;
        }

        public async Task<users?> GetByDui(string dui)
        {
            return await ctx.users.FirstOrDefaultAsync(x => x.dui == dui);
        }

        public async Task Delete(users user)
        {
            ctx.users.Remove(user);
            await ctx.SaveChangesAsync();
        }
    }
}
