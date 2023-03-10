using Microsoft.EntityFrameworkCore;
using InvestmentTracking.Data;
using InvestmentTracking.Core.Data;
using InvestmentTracking.Data.Repositories;
using Serilog;
using InvestmentTracking.Core.Services;
using InvestmentTracking.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("InvestmentTrackingDb");
builder.Services.AddDbContext<SqlDbContext>(options =>
    options.UseSqlServer(connectionString, opts => opts.EnableRetryOnFailure()));

//add services
builder.Services.AddScoped<IBrokerService, BrokerService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//add repos
builder.Services.AddTransient<IBrokerRepository, BrokerRepository>();
builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();

//serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Services.AddLogging();
builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SqlDbContext>();
    dbContext.ApplyMigrations();
}

app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();
