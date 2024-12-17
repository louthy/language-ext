namespace LanguageExt.Core;

/// <summary>
/// Certain characters don't work in the XML comments, so we use these alternatives
///
///     &lt;  becomes 〈     (Unicode 3008)
///     &gt;  becomes 〉     (Unicode 3009)
///     &amp; becomes ＆     (Unicode FF06)
/// 
/// </summary>
public class Comment_Alternatives;
