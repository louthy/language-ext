using Newsletter.Effects.Traits;
using SendGrid.Helpers.Mail;

namespace Newsletter.Effects;

/// <summary>
/// Json parser
/// </summary>
public static class Email<RT>
    where RT : Has<Eff<RT>, EmailIO>,
               Reads<Eff<RT>, RT, Config>
{
    static readonly Eff<RT, EmailIO> trait =
        Stateful.getsM<Eff<RT>, RT, EmailIO>(rt => rt.Trait).As();

    /// <summary>
    /// Send an email
    /// </summary>
    public static Eff<RT, Unit> send(
        EmailAddress from,
        EmailAddress to,
        string subject,
        string plainTextContent,
        string htmlContent) =>
        from key in Config<RT>.sendGridApiKey
        from eml in trait
        from res in liftIO(io => eml.Send(key, @from, to, subject, plainTextContent, htmlContent, io.Token))
        select unit;
}
