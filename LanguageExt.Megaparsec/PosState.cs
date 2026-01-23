namespace LanguageExt.Megaparsec;

public readonly record struct PosState<S>(
    S Input, 
    long Offset, 
    SourcePos SourcePos, 
    int TabWidth, 
    string LinePrefix);
