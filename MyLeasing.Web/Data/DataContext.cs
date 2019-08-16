namespace MyLeasing.Web.Data
{
    using Microsoft.EntityFrameworkCore;
    using MyLeasing.Web.Data.Entities;

    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Owner> Owners { get; set; }
    }
}
