using static LanguageExt.Prelude;
var builder = WebApplication.CreateBuilder(args);
var app     = builder.Build();

app.MapGet("/sync",
           () =>
           {
               var effect = yieldFor(1000).Map("Hello, World");
               return effect.Run();
           });

app.MapGet("/async",
           async () =>
           {
               var effect = yieldFor(1000).Map("Hello, World");
               return await effect.RunAsync();
           });

app.MapGet("/fork-sync", 
           () => 
           {
               var effect      = yieldFor(1000).Map("Hello, World");
               var computation = awaitIO(fork(effect));
               return computation.Run();
           });

app.MapGet("/fork-async",
           () =>
           {
               var effect      = yieldFor(1000).Map("Hello, World");
               var computation = awaitIO(fork(effect));
               return computation.Run();
           });

app.Run();
