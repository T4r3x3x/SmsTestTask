using Microsoft.EntityFrameworkCore;

namespace Sms.ConsoleApp.Database;

public sealed class MenuDbContext(DbContextOptions<MenuDbContext> options) : DbContext(options)
{
    internal static DbContextOptions<MenuDbContext> CreateOptions(string connectionString) =>
        new DbContextOptionsBuilder<MenuDbContext>().UseNpgsql(connectionString).Options;

    internal DbSet<MenuItemEntity> MenuItems => Set<MenuItemEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var menuItem = modelBuilder.Entity<MenuItemEntity>();
        menuItem.ToTable("menu_items");
        menuItem.HasKey(item => item.Id);
        menuItem.Property(item => item.Id).HasColumnName("id");
        menuItem.Property(item => item.Article).HasColumnName("article");
        menuItem.Property(item => item.Name).HasColumnName("name");
        menuItem.Property(item => item.Price).HasColumnName("price");
        menuItem.Property(item => item.IsWeighted).HasColumnName("is_weighted");
        menuItem.Property(item => item.FullPath).HasColumnName("full_path");
        menuItem.Property(item => item.Barcodes).HasColumnName("barcodes");
    }
}