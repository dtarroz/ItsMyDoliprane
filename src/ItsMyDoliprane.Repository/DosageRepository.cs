using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace ItsMyDoliprane.Repository;

public class DosageRepository : AbstractRepository
{
    public DosageRepository(IConfiguration configuration) : base(configuration) { }

    public int GetDosageSinceDate(int personId, int compositionId, DateTime date) {
        using SqliteConnection connection = CreateConnectionAndOpen();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT IFNULL(SUM(DRUG_DOSAGE.QUANTITY), 0)
                                FROM DRUG_DOSAGE
                                INNER JOIN MEDICATION ON MEDICATION.DRUG_PKEY = DRUG_DOSAGE.DRUG_PKEY
                                WHERE DRUG_DOSAGE.DRUG_COMPOSITION_PKEY =  $compositionId
                                  AND MEDICATION.PERSON_PKEY = $personId
                                  AND MEDICATION.DATE > strftime('%s', $since)";
        command.Parameters.AddWithValue("$personId", personId);
        command.Parameters.AddWithValue("$compositionId", compositionId);
        command.Parameters.AddWithValue("$since", date.ToString("yyyy-MM-dd HH:mm:ss"));
        return Convert.ToInt32(command.ExecuteScalar());
    }
}
