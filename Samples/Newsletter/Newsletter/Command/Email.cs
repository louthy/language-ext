using Newsletter.Data;
using Newsletter.Effects.Traits;
using Newsletter.UI;
using SendGrid.Helpers.Mail;

namespace Newsletter.Command;

public static class Email<RT>
    where RT : 
        Has<Eff<RT>, EmailIO>,
        Has<Eff<RT>, ConsoleIO>,
        Reads<Eff<RT>, RT, Config>
{
    public static Eff<RT, Unit> sendToAll(Seq<Member> members, Letter letter) =>
        members.Traverse(m => send(m.Name, m.Email, letter))
               .IgnoreF() 
               .As();

    public static Eff<RT, Unit> send(string name, string email, Letter letter) =>
        Effects.Email<RT>
               .send(new EmailAddress("noreply@paullouth.com", "Notes from a Small Functional Island"),
                     new EmailAddress(email, name),
                     letter.Title,
                     letter.PlainText,
                     letter.Html)
              | catchM(Display<Eff<RT>, RT>.error);
}
