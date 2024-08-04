namespace Newsletter.Effects.Traits;

public interface WebIO
{
    public Task<byte[]> Download(Uri uri, HttpClient client);
}
