
namespace LanguageExt
{
    public interface AsyncPair<SyncA, AsyncA>
    {
        AsyncA ToAsync(SyncA sa);
    }
}
