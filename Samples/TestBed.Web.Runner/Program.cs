using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;

var http = new HttpClient();

var syncV5 = Scenario.Create("v5-sync", async context =>
                                        {
                                            var request  = Http.CreateRequest("GET", "http://localhost:5000/sync");
                                            var response = await Http.Send(http, request).ConfigureAwait(false);

                                            return response;
                                        })
                     .WithoutWarmUp()
                     .WithLoadSimulations(
                          Simulation.Inject(rate: 500,
                                            interval: TimeSpan.FromSeconds(1),
                                            during: TimeSpan.FromSeconds(30))
                      );

var asyncV5 = Scenario.Create("v5-async", async context =>
                                         {
                                             var request  = Http.CreateRequest("GET", "http://localhost:5000/async");
                                             var response = await Http.Send(http, request).ConfigureAwait(false);

                                             return response;
                                         })
                      .WithoutWarmUp()
                      .WithLoadSimulations(
                           Simulation.Inject(rate: 500,
                                             interval: TimeSpan.FromSeconds(1),
                                             during: TimeSpan.FromSeconds(30))
                       );

NBomberRunner.RegisterScenarios(asyncV5).Run();
NBomberRunner.RegisterScenarios(syncV5).Run();
