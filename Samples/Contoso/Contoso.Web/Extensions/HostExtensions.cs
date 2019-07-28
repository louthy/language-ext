using System;
using Contoso.Infrastructure.Data;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace Contoso.Web.Extensions
{
    public static class HostExtensions
    {
        public static IHost SeedDatabase(this IHost host)
        {
            var test = use(() => host.Services.CreateScope(), Seed);
            return host;
        }

        static Func<IServiceScope, Unit> Seed = (scope) =>
        {
            var services = scope.ServiceProvider;
            Try(GetDbContext(services)).Bind(ctx => Try(InitializeDb(ctx)))
                .Match(
                    Succ: a => { },
                    Fail: ex => LogException(ex, services));
            return Unit.Default;
        };

        static Func<IServiceProvider, ContosoDbContext> GetDbContext = (provider) =>
            provider.GetRequiredService<ContosoDbContext>();

        static Func<ContosoDbContext, Unit> InitializeDb = (context) =>
            DbInitializer.Initialize(context);

        private static void LogException(Exception ex, IServiceProvider provider) =>
            provider.GetRequiredService<ILogger<Program>>()
                .LogError(ex, "Error occurred while seeding database");
    }
}
