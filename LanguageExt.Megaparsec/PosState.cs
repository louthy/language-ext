namespace LanguageExt.Megaparsec;

public readonly record struct PosState<S>(
    S Input, 
    int Offset, 
    SourcePos SourcePos, 
    int TabWidth, 
    string LinePrefix);
