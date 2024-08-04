using Newsletter.Data;
using Newsletter.Effects.Traits;
using Newsletter.UI;

namespace Newsletter.Command;

public static class Send<M, RT>
    where RT : 
        Has<M, WebIO>,
        Has<M, JsonIO>,
        Has<M, FileIO>,
        Has<M, EmailIO>,
        Has<M, ConsoleIO>,
        Has<M, EncodingIO>,
        Has<M, DirectoryIO>,
        Reads<M, RT, Config>,
        Reads<M, RT, HttpClient>
    where M :
        Monad<M>,
        Fallible<M>,
        Stateful<M, RT>
{
    public static K<M, Unit> newsletter =>
        from posts     in Posts<M, RT>.readLastFromApi(4)
        from members   in Members<M, RT>.readAll
        from templates in Templates<M, RT>.loadDefault
        from letter    in Newsletter<M, RT>.make(posts, templates)
        from _1        in Newsletter<M, RT>.save(letter)
        from _2        in Display<M, RT>.showWhatsAboutToHappen(members)
        from _3        in askUserToConfirmSend
        from _4        in Email<M, RT>.sendToAll(members, letter)
        from _5        in Display<M, RT>.confirmSent
        select unit;

    static K<M, Unit> askUserToConfirmSend =>
        from k in Console<M, RT>.readKey
        from x in Display<M, RT>.emptyLine
        from _ in k.Key == ConsoleKey.Y
                      ? pure<M, Unit>(unit)
                      : error<M>(Error.New("user cancelled"))
        select unit;
}
