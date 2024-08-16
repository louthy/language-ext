using LanguageExt;
using static LanguageExt.Prelude;
var builder = WebApplication.CreateBuilder(args);
var app     = builder.Build();

app.MapGet("/sync", () =>
                    {
                        var effect = liftIO(async () =>
                                            {
                                                await Task.Delay(1000).ConfigureAwait(false);
                                                return unit;
                                            });

                        // TODO: This is 5x slower than the RunAsync version below
                        // TODO: Make it faster
                        var computation = from f in effect.Fork()
                                          from r in f.Await
                                          select r;

                        return computation.Run();
                    });

app.MapGet("/async", async () =>
                     {
                         var effect = liftIO(async () =>
                                             {
                                                 await Task.Delay(1000).ConfigureAwait(false);
                                                 return unit;
                                             });

                         return await effect.RunAsync();
                     });

app.Run();
