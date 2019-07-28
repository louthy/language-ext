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
            use(host.Services.CreateScope(), Seed);
            return host;
        }

        static readonly Func<IServiceScope, Unit> Seed = (scope) =>
        {
            var services = scope.ServiceProvider;
            TryGetDbContext(services).Bind(ctx => TryInitializeDb(ctx))
                .Match(
                    Succ: a => { },
                    Fail: ex => LogException(ex, services));
            return Unit.Default;
        };

        private static Try<ContosoDbContext> TryGetDbContext(IServiceProvider provider) => () =>
            provider.GetRequiredService<ContosoDbContext>();

        private static Try<Unit> TryInitializeDb(ContosoDbContext context) => () =>
            DbInitializer.Initialize(context);

        private static void LogException(Exception ex, IServiceProvider provider) =>
            provider.GetRequiredService<ILogger<Program>>()
                .LogError(ex, "Error occurred while seeding database");
    }
}
