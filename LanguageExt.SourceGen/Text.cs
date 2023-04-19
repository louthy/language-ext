namespace LanguageExt.SourceGen;

public class Text
{
    public static bool FirstCharIsUpper(string name) =>
        name.Length > 0 && char.IsUpper(name[0]);
}
