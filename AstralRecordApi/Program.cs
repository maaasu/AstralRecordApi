using AstralRecordApi.Options;
using AstralRecordApi.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<FileDatabaseOptions>(
    builder.Configuration.GetSection(FileDatabaseOptions.SectionName));

builder.Services.AddSingleton<IBuffRepository, BuffRepository>();
builder.Services.AddSingleton<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "Astral Record API";
        document.Info.Version = "v1";
        document.Info.Description = "Minecraft Purpur サーバー向け MMO RPG プラグイン Astral Record の Web API";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Scalar API ドキュメント UI (/scalar)
app.MapOpenApi();
app.MapScalarApiReference("/scalar");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

_ = app.Services.GetRequiredService<IBuffRepository>();
_ = app.Services.GetRequiredService<IItemRepository>();

app.Run();
