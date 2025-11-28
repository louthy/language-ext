namespace LanguageExt;

/// <summary>
/// A unit type that represents `Seq.Empty`.  This type can be implicitly
/// converted to `Seq〈A〉`.
/// </summary>
public readonly struct SeqEmpty
{
    public static SeqEmpty Default = new();
}
