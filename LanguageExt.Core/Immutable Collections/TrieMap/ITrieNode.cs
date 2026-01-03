namespace LanguageExt;

internal enum TrieNodeTag
{
    Entries,
    Collision,
    Empty
}

interface ITrieNode
{
    TrieNodeTag Type { get; }
}
