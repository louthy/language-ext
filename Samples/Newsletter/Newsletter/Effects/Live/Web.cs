using Newsletter.Effects.Traits;

namespace Newsletter.Effects.Live;

public record Web : WebIO
{
    public static readonly WebIO Default = new Web();

    public async Task<byte[]> Download(Uri uri, HttpClient client) =>
        await client.GetByteArrayAsync(uri);
}
