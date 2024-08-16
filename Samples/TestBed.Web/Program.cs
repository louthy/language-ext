using LanguageExt;
using static LanguageExt.Prelude;
var builder = WebApplication.CreateBuilder(args);
var app     = builder.Build();

app.MapGet("/", async () =>
                {
                    var effect = liftIO(async () =>
                                        {
                                            await Task.Delay(1000).ConfigureAwait(false);
                                            return unit;
                                        });

                    await effect.RunAsync();
                });

app.Run();
