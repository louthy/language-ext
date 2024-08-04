using Newsletter.Data;
using Newsletter.Effects.Traits;
using Newsletter.UI;

namespace Newsletter.Command;

public static class Send<RT>
    where RT : 
        Has<Eff<RT>, WebIO>,
        Has<Eff<RT>, JsonIO>,
        Has<Eff<RT>, FileIO>,
        Has<Eff<RT>, EmailIO>,
        Has<Eff<RT>, ConsoleIO>,
        Has<Eff<RT>, EncodingIO>,
        Has<Eff<RT>, DirectoryIO>,
        Reads<Eff<RT>, RT, Config>,
        Reads<Eff<RT>, RT, HttpClient>
{
    public static Eff<RT, Unit> newsletter =>
        from posts     in Posts<RT>.readLastFromApi(4)
        from members   in Members<RT>.readAll
        from templates in Templates<RT>.loadDefault
        from letter    in Newsletter<RT>.make(posts, templates)
        from _1        in Newsletter<RT>.save(letter)
        from _2        in Display<Eff<RT>, RT>.showWhatsAboutToHappen(members)
        from _3        in askUserToConfirmSend
        from _4        in Email<RT>.sendToAll(members, letter)
        from _5        in Display<Eff<RT>, RT>.confirmSent
        select unit;

    static K<Eff<RT>, Unit> askUserToConfirmSend =>
        from k in Console<Eff<RT>, RT>.readKey
        from x in Display<Eff<RT>, RT>.emptyLine
        from _ in k.Key == ConsoleKey.Y
                      ? SuccessEff<RT, Unit>(unit)
                      : Fail(Error.New("user cancelled"))
        select unit;
}
