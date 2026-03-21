using AstralRecordApi.Options;
using AstralRecordApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<FileDatabaseOptions>(
    builder.Configuration.GetSection(FileDatabaseOptions.SectionName));

builder.Services.AddSingleton<IBuffRepository, BuffRepository>();
builder.Services.AddSingleton<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

_ = app.Services.GetRequiredService<IBuffRepository>();
_ = app.Services.GetRequiredService<IItemRepository>();

app.Run();
