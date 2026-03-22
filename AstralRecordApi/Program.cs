using AstralRecordApi.Authentication;
using AstralRecordApi.Options;
using AstralRecordApi.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<FileDatabaseOptions>(
    builder.Configuration.GetSection(FileDatabaseOptions.SectionName));

builder.Services.AddSingleton<IBuffRepository, BuffRepository>();
builder.Services.AddSingleton<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

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
        document.Components!.SecuritySchemes![ApiKeyAuthenticationHandler.SchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = ApiKeyAuthenticationHandler.HeaderName,
            Description = "リクエストヘッダー `X-Api-Key` に API キーを指定してください。"
        };

        document.Security!.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(ApiKeyAuthenticationHandler.SchemeName, document)] = []
        });

        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Scalar API ドキュメント UI (/scalar)
app.MapOpenApi();
app.MapScalarApiReference("/scalar");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();

_ = app.Services.GetRequiredService<IBuffRepository>();
_ = app.Services.GetRequiredService<IItemRepository>();

app.Run();
