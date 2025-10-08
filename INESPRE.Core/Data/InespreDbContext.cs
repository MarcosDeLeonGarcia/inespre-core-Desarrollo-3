using Microsoft.EntityFrameworkCore;

// Models
using INESPRE.Core.Models.Roles;
using INESPRE.Core.Models.Users;
using INESPRE.Core.Models.Producers;
using INESPRE.Core.Models.Products;
using INESPRE.Core.Models.Events;
using INESPRE.Core.Models.Purchasing;
using INESPRE.Core.Models.Inventory;
using INESPRE.Core.Models.Sales;
using INESPRE.Core.Models.Payments;

namespace INESPRE.Core.Data;

public class InespreDbContext : DbContext
{
    public InespreDbContext(DbContextOptions<InespreDbContext> options) : base(options) { }

    // === DbSets (solo lectura/escritura ligera; SPs siguen con Dapper) ===
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();

    public DbSet<Producer> Producers => Set<Producer>();

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ComboItem> ComboItems => Set<ComboItem>();

    public DbSet<Event> Events => Set<Event>();

    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();

    public DbSet<InventoryLot> InventoryLots => Set<InventoryLot>();

    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbo");

        // Roles
        modelBuilder.Entity<Role>(e =>
        {
            e.ToTable("Roles");
            e.HasKey(x => x.RoleId);
            e.Property(x => x.Name).HasMaxLength(50);
            e.HasIndex(x => x.Name).IsUnique();
        });

        // Users
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.HasKey(x => x.UserId);
            e.Property(x => x.Username).HasMaxLength(50);
            e.HasIndex(x => x.Username).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
        });

        // Producers
        modelBuilder.Entity<Producer>(e =>
        {
            e.ToTable("Producers");
            e.HasKey(x => x.ProducerId);
            e.Property(x => x.Name).HasMaxLength(120);
        });

        // Products
        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.HasKey(x => x.ProductId);
            e.Property(x => x.Name).HasMaxLength(120);
            e.Property(x => x.SKU).HasMaxLength(40);
            e.Property(x => x.Unit).HasMaxLength(10);
        });

        // ComboItems (clave compuesta)
        modelBuilder.Entity<ComboItem>(e =>
        {
            e.ToTable("ComboItems");
            e.HasKey(x => new { x.ComboProductId, x.ComponentProductId });
        });

        // Events
        modelBuilder.Entity<Event>(e =>
        {
            e.ToTable("Events");
            e.HasKey(x => x.EventId);
            e.Property(x => x.Name).HasMaxLength(120);
            e.Property(x => x.EventType).HasMaxLength(10);
            e.Property(x => x.Status).HasMaxLength(15);
            e.HasIndex(x => x.EventDateTime);
        });

        // PurchaseOrders
        modelBuilder.Entity<PurchaseOrder>(e =>
        {
            e.ToTable("PurchaseOrders");
            e.HasKey(x => x.POId);
            e.Property(x => x.Status).HasMaxLength(12);
        });

        // PurchaseOrderItems (LineTotal es columna calculada en SQL)
        modelBuilder.Entity<PurchaseOrderItem>(e =>
        {
            e.ToTable("PurchaseOrderItems");
            e.HasKey(x => x.POItemId);
            e.Property(x => x.LineTotal)
             .HasColumnType("decimal(18,2)")
             .ValueGeneratedOnAddOrUpdate(); // computed column
        });

        // InventoryLots
        modelBuilder.Entity<InventoryLot>(e =>
        {
            e.ToTable("InventoryLots");
            e.HasKey(x => x.LotId);
            e.Property(x => x.Location).HasMaxLength(20);
            e.HasIndex(x => new { x.ProductId, x.AvailableQty });
        });

        // Sales
        modelBuilder.Entity<Sale>(e =>
        {
            e.ToTable("Sales");
            e.HasKey(x => x.SaleId);
            e.Property(x => x.PaymentMethod).HasMaxLength(20);
            e.Property(x => x.Status).HasMaxLength(10);
            e.HasIndex(x => new { x.EventId, x.SaleDate });
        });

        // SaleItems (LineTotal calculado)
        modelBuilder.Entity<SaleItem>(e =>
        {
            e.ToTable("SaleItems");
            e.HasKey(x => x.SaleItemId);
            e.Property(x => x.LineTotal)
             .HasColumnType("decimal(18,2)")
             .ValueGeneratedOnAddOrUpdate(); // computed column
        });

        // Payments
        modelBuilder.Entity<Payment>(e =>
        {
            e.ToTable("Payments");
            e.HasKey(x => x.PaymentId);
            e.Property(x => x.Method).HasMaxLength(20);
            e.Property(x => x.Status).HasMaxLength(10);
        });

        base.OnModelCreating(modelBuilder);
    }
}
