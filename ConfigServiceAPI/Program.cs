using ConfigServiceAPI.Persistance;
using ConfigServiceAPI.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.UseAllOfToExtendReferenceSchemas();
    options.SupportNonNullableReferenceTypes();
});
DotNetEnv.Env.Load();


var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString = $"Host={dbHost};Database={dbName};Username={dbUser};Password={dbPassword}";

builder.Services.AddDbContext<ServiceConfigAPIDbContext>(options =>
    options.UseNpgsql(connectionString));


builder.Services.AddScoped<EnviromentRepository>();
builder.Services.AddScoped<VariablesRepository>();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
