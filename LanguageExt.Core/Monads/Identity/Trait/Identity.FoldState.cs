namespace LanguageExt;

public partial class Identity
{
    public readonly ref struct FoldState(bool HasRun)
    {
        public bool HasRun { get; } = HasRun;
    }
}
