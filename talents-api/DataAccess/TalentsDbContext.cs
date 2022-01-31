using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentsApi.Models.Entities;

namespace TalentsApi.DataAccess
{
    public class TalentsDbContext : DbContext
    {
        public TalentsDbContext(DbContextOptions<TalentsDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserRoleIds> UserRoleIds { get; set; }
    }
}
