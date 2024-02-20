using System;
using System.IO;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Live;

public record TextReadIO : Sys.Traits.TextReadIO
{
    public static readonly Sys.Traits.TextReadIO Default =
        new TextReadIO();

    /// <summary>
    /// Read a line of text from the stream
    /// </summary>
    public IO<Option<string>> ReadLine(TextReader reader) =>
        liftIO(async env =>
               {
                   var str = await reader.ReadLineAsync(env.Token).ConfigureAwait(false);
                   return str == null
                              ? None
                              : Some(str);
               });

    /// <summary>
    /// Read the rest of the text in the stream
    /// </summary>
    public IO<string> ReadToEnd(TextReader reader) =>
        liftIO(env => reader.ReadToEndAsync(env.Token));

    /// <summary>
    /// Read chars from the stream into the buffer
    /// Returns the number of chars read
    /// </summary>
    public IO<int> Read(TextReader reader, Memory<char> buffer) =>
        liftVIO(env => reader.ReadAsync(buffer, env.Token));

    /// <summary>
    /// Close the reader
    /// </summary>
    public IO<Unit> Close(TextReader reader) =>
        lift(() =>
             {
                 reader.Close();
                 reader.Dispose();
                 return unit;
             });
}
