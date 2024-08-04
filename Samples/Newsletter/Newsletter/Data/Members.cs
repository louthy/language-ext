using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Newsletter.Effects;

namespace Newsletter.Data;

public static class Members<RT>
    where RT : 
    Has<Eff<RT>, EncodingIO>,
    Has<Eff<RT>, FileIO>,
    Has<Eff<RT>, DirectoryIO>,
    Reads<Eff<RT>, RT, Config>
{
    public static Eff<RT, Seq<Member>> readAll =>
        from folder  in Config<RT>.membersFolder
        from path    in readFirstFile(folder)
        from members in readMembers(path)
        select members.Filter(m => m.SubscribedToEmails);
    
    static K<Eff<RT>, string> readFirstFile(string folder) =>
        Directory<Eff<RT>, RT>.enumerateFiles(folder, "*.csv")
                              .Map(fs => fs.OrderDescending()
                                           .AsEnumerableM()
                                           .Head())
                              .Bind(path => path.Match(Some: SuccessEff<RT, string>,
                                                       None: () => Fail(Error.New($"no member files found in {folder}"))));                 

    static Eff<RT, Seq<Member>> readMembers(string path) =>
        liftEff<RT, Seq<Member>>(_ =>
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) 
            {
                HasHeaderRecord = true
            };
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<Row>();
            return records
                .AsEnumerableM()
                .Map(r => new Member(r.id, r.email, r.name, r.subscribed_to_emails == "true", r.tiers == "Supporter"))
                .ToSeq()
                .Strict();
        });

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
