using LanguageExt.ClassInstances.Const;
using LanguageExt.Traits;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances.Pred;

public struct CharSatisfy<MIN, MAX> : Pred<char>
    where MIN : Const<char>
    where MAX : Const<char>
{
    [Pure]
    public static bool True(char value) =>
        Range<TChar, char, MIN, MAX>.True(value);
}

public struct Char<CH> : Pred<char>
    where CH : Const<char>
{
    [Pure]
    public static bool True(char value) =>
        CH.Value == value;
}

public struct Letter : Pred<char>
{
    [Pure]
    public static bool True(char value) =>
        Exists<char,
            CharSatisfy<ChA, ChZ>,
            CharSatisfy<Cha, Chz>>.True(value);
}

public struct Digit : Pred<char>
{
    [Pure]
    public static bool True(char value) =>
        CharSatisfy<Ch0, Ch9>.True(value);
}

public struct Space : Pred<char>
{
    [Pure]
    public static bool True(char value) =>
        Exists<char, Char<ChSpace>, Char<ChTab>, Char<ChCR>, Char<ChLF>>.True(value);
}

public struct AlphaNum : Pred<char>
{
    [Pure]
    public static bool True(char value) =>
        Exists<char, Letter, Digit>.True(value);
}
