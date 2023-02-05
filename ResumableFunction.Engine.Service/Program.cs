using ResumableFunction.Engine.Helpers;
using ResumableFunction.Engine.InOuts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .AddNewtonsoftJson()
    .AddFunctionEngine(builder.Configuration.GetSection(nameof(EngineSettings)).Get<EngineSettings>());
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
//app.Urls.Add("http://localhost:7295");
app.Run();
