using System;
using System.Diagnostics;
using System.Reflection;

namespace LanguageExt
{
    public record ActivityEnv(ActivitySource ActivitySource, Activity? Activity, string? ParentId) : IDisposable
    {
        public static readonly ActivityEnv Default;

        static ActivityEnv()
        {
            var asm = Assembly.GetEntryAssembly()?.GetName();
            Default = new ActivityEnv(new ActivitySource(asm?.Name ?? "", asm?.Version?.ToString() ?? "0.0"), null, null);
        }
            
        public void Dispose()
        {
            ActivitySource.Dispose();
            Activity?.Dispose();
        }
    };
}
