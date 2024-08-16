using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;

var http = new HttpClient();

/*var scenarioV4 = Scenario.Create("v4", async context =>
                                       {
                                           var request = Http.CreateRequest("GET", "http://localhost:5223");

                                           var response = await Http.Send(http, request).ConfigureAwait(false);

                                           return response;
                                       })
                         .WithoutWarmUp()
                         .WithLoadSimulations(
                              Simulation.Inject(rate: 500,
                                                interval: TimeSpan.FromSeconds(1),
                                                during: TimeSpan.FromSeconds(30))
                          );*/

var scenarioV5 = Scenario.Create("v5", async context =>
                                       {
                                           var request  = Http.CreateRequest("GET", "http://localhost:5000");
                                           var response = await Http.Send(http, request).ConfigureAwait(false);

                                           return response;
                                       })
                         .WithoutWarmUp()
                         .WithLoadSimulations(
                              Simulation.Inject(rate: 500,
                                                interval: TimeSpan.FromSeconds(1),
                                                during: TimeSpan.FromSeconds(30))
                          );

//NBomberRunner.RegisterScenarios(scenarioV4).Run();
NBomberRunner.RegisterScenarios(scenarioV5).Run();
