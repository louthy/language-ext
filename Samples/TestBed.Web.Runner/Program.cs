using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;

var http = new HttpClient();

var sim = (string name) =>
              Scenario.Create(name, async context =>
                                      {
                                          var request  = Http.CreateRequest("GET", $"http://localhost:5000/{name}");
                                          var response = await Http.Send(http, request).ConfigureAwait(false);
                                          return response;
                                      })
                    //.WithoutWarmUp()
                      .WithLoadSimulations(
                           Simulation.Inject(rate: 500,
                                             interval: TimeSpan.FromSeconds(1),
                                             during: TimeSpan.FromSeconds(30)));

NBomberRunner.RegisterScenarios(sim("async")).Run();
NBomberRunner.RegisterScenarios(sim("sync")).Run();
NBomberRunner.RegisterScenarios(sim("fork-async")).Run();
NBomberRunner.RegisterScenarios(sim("fork-sync")).Run();
