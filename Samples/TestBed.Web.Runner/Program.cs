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
                      .WithoutWarmUp()
                      .WithLoadSimulations(
                           Simulation.Inject(rate: 500,
                                             interval: TimeSpan.FromSeconds(1),
                                             during: TimeSpan.FromSeconds(30)));

var fork = sim("fork");
var sync = sim("sync");
var async = sim("async");

NBomberRunner.RegisterScenarios(fork).Run();
NBomberRunner.RegisterScenarios(sync).Run();
NBomberRunner.RegisterScenarios(async).Run();
