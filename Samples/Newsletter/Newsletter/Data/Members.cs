using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Newsletter.Effects;

namespace Newsletter.Data;

public static class Members<M, RT>
    where RT : 
        Has<M, EncodingIO>,
        Has<M, FileIO>,
        Has<M, DirectoryIO>,
        Reads<M, RT, Config>
    where M :
        Monad<M>,
        Fallible<M>,
        Stateful<M, RT>
{
    public static K<M, Seq<Member>> readAll =>
        from folder  in Config<M, RT>.membersFolder
        from path    in readFirstFile(folder)
        select readMembers(path).Filter(m => m.SubscribedToEmails);
    
    static K<M, string> readFirstFile(string folder) =>
        Directory<M, RT>.enumerateFiles(folder, "*.csv")
                              .Map(fs => fs.OrderDescending()
                                           .AsEnumerableM()
                                           .Head())
                              .Bind(path => path.Match(
                                        Some: M.Pure,
                                        None: M.Fail<string>(Error.New($"no member files found in {folder}"))));

    static Seq<Member> readMembers(string path)
    {
        var       config  = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        using var reader  = new StreamReader(path);
        using var csv     = new CsvReader(reader, config);
        var       records = csv.GetRecords<Row>();
        return records
              .AsEnumerableM()
              .Map(r => new Member(r.id, r.email, r.name, r.subscribed_to_emails == "true", r.tiers == "Supporter"))
              .ToSeq()
              .Strict();
    }

    record Row(
        string id,
        string email,
        string name,
        string note,
        string subscribed_to_emails,
        string complimentary_plan,
        string stripe_customer_id,
        string created_at,
        string deleted_at,
        string labels,
        string tiers);
}

public record Member(string Id, string Email, string Name, bool SubscribedToEmails, bool Supporter);
