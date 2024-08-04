using System.Text.Json;
using Microsoft.Extensions.Configuration.UserSecrets;
using Newsletter.Command;

var envIO       = EnvIO.New();
var path        = args is [_, var p] ? p : Environment.CurrentDirectory;
var secrets     = loadUserSecrets();
var sendGridKey = secrets.Find("SendGridKey");

var result = args is ["live", ..]
    ? Send<Newsletter.Effects.Live.Runtime>.newsletter.Run(
        Newsletter.Effects.Live.Runtime.New(path, sendGridKey),
        envIO)
    : Send<Newsletter.Effects.Test.Runtime>.newsletter.Run(
        Newsletter.Effects.Test.Runtime.New(path, sendGridKey),
        envIO);

switch (result)
{
    case Fin.Succ<Unit>:
        Console.WriteLine("Complete");
        break;
    
    case Fin.Fail<Unit> (var error):
        Console.WriteLine(error);
        break;
}

// Loads the user-secrets controlled by "dotnet user-secrets" 
HashMap<string, string> loadUserSecrets()
{
    var userSecretsId = "fbefbe46-9ced-4f15-92b1-db45597ea1e3";
    var path          = PathHelper.GetSecretsPathFromSecretsId(userSecretsId);
    var text          = File.ReadAllText(path);
    var json          = JsonDocument.Parse(text);
    return json.RootElement
               .EnumerateObject()
               .AsEnumerableM()
               .Map(e => (e.Name, e.Value.GetString() ?? ""))
               .ToHashMap();
}
