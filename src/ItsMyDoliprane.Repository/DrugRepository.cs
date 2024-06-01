using ItsMyDoliprane.Repository.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace ItsMyDoliprane.Repository;

public class DrugRepository : AbstractRepository
{
    public DrugRepository(IConfiguration configuration) : base(configuration) { }

    public List<Drug> GetDrugs() {
        using SqliteConnection connection = CreateConnectionAndOpen();
        using SqliteCommand command = connection.CreateCommand();
        List<Drug> drugs = new List<Drug>();
        command.CommandText = "SELECT PKEY, NAME, VISIBLE FROM DRUG";
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
            drugs.Add(new Drug {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Visible = reader.GetInt32(2) == 1
                        });
        return drugs;
    }
}
