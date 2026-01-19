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
        try
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
                builder.Services.ConfigureHashing();
                builder.Services.ConfigureMediatR();
                builder.Services.ConfigureRareLimiters();
                builder.Services.ConfigureEmailService(builder.Configuration);
                builder.Services.ConfigureErrorHandling();
                builder.Services.ConfigureDatabaseContext(builder.Configuration.GetConnectionString("DevelopmentConnection")!);

                builder.Services.AddScoped<IJwtManager, JwtManager>();
                builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
                builder.Services.AddScoped<IQuickResponseCodeGenerator, QuickResponseCodeGenerator>();
                builder.Services.AddSwaggerGen();
                builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
                builder.Services.AddHostedService<AdminCleanupBackgroundService>();
                builder.Services.AddHostedService<EmployeeCleanupBackgroundService>();
                builder.Services.AddHostedService<PropertySuggestionCleanupBackgroundService>();
                builder.Services.AddHostedService<OrganizationInvitationCleanupBackgroundService>();
            }
            var app = builder.Build();
            {
                Log.Information($"Application running on: {builder.Configuration["ASPNETCORE_URLS"]}");

                app.UseSerilogRequestLogging();
                app.UseExceptionHandler(_ => { });

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseCors(CorsConstants.Policies.FRONTEND_POLICY);
                app.ConfigureStatusCodePages();
                app.UseAuthorization();
                app.UseRateLimiter();
                app.MapControllers();
                app.Run();
            }
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "Unhandled exception while starting the application.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
