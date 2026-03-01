using System.Text.Json.Serialization;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Services.Admins.Backgrounds;
using InvenireServer.Application.Services.Employees.Backgrounds;
using InvenireServer.Application.Services.Organizations.Invitations.Backgrounds;
using InvenireServer.Application.Services.Properties.Suggestions.Backgrounds;
using InvenireServer.Domain.Constants;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Infrastructure.Persistence.Repositories;
using InvenireServer.Infrastructure.Utilities.QR;
using InvenireServer.Presentation.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace InvenireServer.Presentation;

/// <summary>
/// Application entry point.
/// </summary>
public class Program
{

    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        {
            builder.Host.ConfigureSerilog(builder.Configuration);
            builder.Host.ConfigureConfiguration();

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 1024 * 1024 * 50; // 50 MB.
            });
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = 1024 * 1024 * 50; // 50 MB.
            });

            builder.Services.ConfigureJwt(builder.Configuration);
            builder.Services.ConfigureCors(builder.Configuration);
            builder.Services.ConfigureForwardedHeaders();
            builder.Services.ConfigureHashing();
            builder.Services.ConfigureMediatR();
            builder.Services.ConfigureRareLimiters();
            builder.Services.ConfigureEmailService(builder.Configuration);
            builder.Services.ConfigureErrorHandling();
            builder.Services.ConfigureDatabaseContext(builder.Configuration.GetConnectionString("Connection")!);

            builder.Services.AddScoped<IJwtManager, JwtManager>();
            builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
            builder.Services.AddScoped<IQuickResponseCodeGenerator, QuickResponseCodeGenerator>();
            builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
            builder.Services.AddHostedService<AdminCleanupBackgroundService>();
            builder.Services.AddHostedService<EmployeeCleanupBackgroundService>();
            builder.Services.AddHostedService<PropertySuggestionCleanupBackgroundService>();
            builder.Services.AddHostedService<OrganizationInvitationCleanupBackgroundService>();
        }
        var app = builder.Build();
        {
            app.UseForwardedHeaders();
            app.UseSerilogRequestLogging();
            app.UseExceptionHandler(_ => { });
            app.UseCors(CorsConstants.Policies.FRONTEND_POLICY);
            app.ConfigureStatusCodePages();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapControllers();

            if (!builder.Environment.IsProduction()) app.RunMigrations();
            app.Run();
        }
    }
}
