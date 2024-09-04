using LanguageExt;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
var builder = WebApplication.CreateBuilder(args);
var app     = builder.Build();

app.MapGet("/sync",
           () =>
           {
               var effect = IO.yield(1000).Map("Hello, World");
               return effect.Run();
           });

app.MapGet("/async",
           async () =>
           {
               var effect = IO.yield(1000).Map("Hello, World");
               return await effect.RunAsync();
           });

app.MapGet("/fork-sync", 
           () => 
           {
               var effect      = IO.yield(1000).Map("Hello, World");
               var computation = awaitIO(forkIO(effect));
               return computation.Run();
           });

app.MapGet("/fork-async",
           () =>
           {
               var effect      = IO.yield(1000).Map("Hello, World");
               var computation = awaitIO(forkIO(effect));
               return computation.Run();
           });

app.Run();
