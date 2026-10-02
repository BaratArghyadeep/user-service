using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.DBContext
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(
            DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
    }
}
