namespace Newsletter.Effects;

public record RuntimeEnv(HttpClient HttpClient, Config Config) : IDisposable
{
    public void Dispose() => 
        HttpClient.Dispose();
}
