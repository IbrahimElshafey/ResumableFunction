using Microsoft.EntityFrameworkCore;
using ResumableFunction.Abstraction;
using ResumableFunction.Engine.Abstraction;
using ResumableFunction.Engine.Data.Sqlite;
using ResumableFunction.Engine.Data.SqlServer;
using ResumableFunction.Engine.EfDataImplementation;
using ResumableFunction.Engine.Helpers;
using ResumableFunction.Engine.InOuts;
using System.Data.SqlTypes;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddFunctionEngine();

builder.Services.AddDbContext<EngineDataContext>(options =>
{
    var config = builder.Configuration.GetSection(nameof(EngineSettings)).Get<EngineSettings>();
    if (config.ProviderName.ToLower().Equals("sqlite"))
    {
        options.UseSqlite(
            config.SqliteConnection!,
            x => x.MigrationsAssembly(typeof(SqliteMarker).Assembly.GetName().Name)
        );
    }
    if (config.ProviderName.ToLower().Equals("sqlserver"))
    {
        options.UseSqlServer(
            config.SqlServerConnection!,
            x => x.MigrationsAssembly(typeof(SqlServerMarker).Assembly.GetName().Name)
        );
    }
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<EngineSettings>(builder.Configuration.GetSection(nameof(EngineSettings)));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
