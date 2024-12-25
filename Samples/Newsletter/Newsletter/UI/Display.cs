using Newsletter.Data;

namespace Newsletter.UI;

public static class Display<M, RT>
    where RT : 
        Has<M, ConsoleIO>
    where M :
        Monad<M>,
        Fallible<M>
{
    public static K<M, Unit> emptyLine =>
        Console<M, RT>.writeEmptyLine;
    
    public static K<M, Unit> writeLine(string line) =>
        Console<M, RT>.writeLine(line);
    
    public static K<M, Unit> confirmSent =>
        Console<M, RT>.writeLine("Newsletter successfully sent to members");
    
    public static K<M, Unit> error(Error err) =>
        Console<M, RT>.writeLine(err.ToString());

    public static K<M, Unit> showWhatsAboutToHappen(Seq<Member> members) =>
        from _1 in writeLine($"You're about to send the newsletter to {members.Count} members")
        from _2 in emptyLine
        from _3 in writeLine("Below are some email addresses from the batch:")
        from _4 in members.Take(10)
                          .Map(m => $"\t{m.Email}")
                          .Traverse(writeLine)
        from _5 in emptyLine
        from _6 in writeLine("Are you sure you want to send the emails? (y/n)")
        select unit;
}
