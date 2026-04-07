using AstralRecordApi.Authentication;
using AstralRecordApi.Data;
using AstralRecordApi.Options;
using AstralRecordApi.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
});

// Add services to the container.

builder.Services.Configure<FileDatabaseOptions>(
    builder.Configuration.GetSection(FileDatabaseOptions.SectionName));

builder.Services.AddDbContext<AstralRecordDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Connection string 'SqlServer' is not configured.")));

builder.Services.AddSingleton<IBuffRepository, BuffRepository>();
builder.Services.AddSingleton<IClassRepository, ClassRepository>();
builder.Services.AddSingleton<IItemRepository, ItemRepository>();
builder.Services.AddSingleton<ILootRepository, LootRepository>();
builder.Services.AddSingleton<ISetEffectRepository, SetEffectRepository>();
builder.Services.AddSingleton<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();

builder.Services.AddAuthentication(ApiKeyAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
        ApiKeyAuthenticationHandler.SchemeName, _ => { });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "Astral Record API";
        document.Info.Version = "v1";
        document.Info.Description = "Minecraft Purpur サーバー向け MMO RPG プラグイン Astral Record の Web API";

        // API キー認証スキームを OpenAPI ドキュメントに追加
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[ApiKeyAuthenticationHandler.SchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = ApiKeyAuthenticationHandler.HeaderName,
            Description = "リクエストヘッダー `X-Api-Key` に API キーを指定してください。"
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(ApiKeyAuthenticationHandler.SchemeName, document)] = []
        });

        return Task.CompletedTask;
    });
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");

// OpenAPI ドキュメント (/openapi/v1.json)
app.MapOpenApi();

// Scalar API ドキュメント UI (/scalar)
app.MapScalarApiReference("/scalar");

// Swagger UI (/swagger)
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Astral Record API v1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();

logger.LogInformation("静的データの読み込みを開始します");
_ = app.Services.GetRequiredService<IBuffRepository>();
_ = app.Services.GetRequiredService<IItemRepository>();
_ = app.Services.GetRequiredService<ILootRepository>();
logger.LogInformation("静的データの読み込みが完了しました");

app.Run();
