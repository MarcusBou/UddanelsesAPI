using Microsoft.EntityFrameworkCore;
using UddanelsesAPI.Models;

namespace UddanelsesAPI
{
    public class EducationContext : DbContext
    {
        private IConfiguration _configuration;
        public EducationContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DB"));
        }

    }
}
