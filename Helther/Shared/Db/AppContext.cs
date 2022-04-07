using Helther.Shared.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Helther.Shared.Db;

public sealed class AppContext: DbContext
{
    public DbSet<Service> Services { get; set; }
    public DbSet<Check> Checks { get; set; }

    public string DbPath { get; }

    public AppContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "storage.db");

        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Service>().ToTable("Services");
        modelBuilder.Entity<Check>().ToTable("History");

        modelBuilder.Entity<Service>()
            .HasMany(s => s.History)
            .WithOne(s => s.Service);
    }
}