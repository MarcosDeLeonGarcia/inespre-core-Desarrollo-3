using Dapper;
using INESPRE.Core.Data;
using INESPRE.Core.Models.Common;
using INESPRE.Core.Services;
using Microsoft.EntityFrameworkCore;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IEventsService, EventsService>();
builder.Services.AddScoped<IProducersService, ProducersService>();
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<IComboItemsService, ComboItemsService>();
builder.Services.AddScoped<IPurchaseOrdersService, PurchaseOrdersService>();
builder.Services.AddScoped<IPurchaseOrderItemsService, PurchaseOrderItemsService>();
builder.Services.AddScoped<IInventoryLotsService, InventoryLotsService>();
builder.Services.AddScoped<ISalesService, SalesService>();
builder.Services.AddScoped<ISaleItemsService, SaleItemsService>();
builder.Services.AddScoped<IPaymentsService, PaymentsService>();
builder.Services.AddScoped<ICashService, CashService>();

// EF Core (SQL Server)
builder.Services.AddDbContext<InespreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dapper: fábrica de conexiones
builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// Endpoints de verificación rápida
app.MapGet("/db-ping", async (InespreDbContext db) =>
{
    var can = await db.Database.CanConnectAsync();
    return Results.Ok(new { database = "INESPRECore", canConnect = can });
});

app.MapGet("/db-ping-dapper", (IDbConnectionFactory factory) =>
{
    using IDbConnection conn = factory.CreateConnection();
    conn.Open();
    using var cmd = conn.CreateCommand();
    cmd.CommandText = "SELECT 1";
    var ok = Convert.ToInt32(cmd.ExecuteScalar()) == 1;
    return Results.Ok(new { ok });
});

app.Run();
