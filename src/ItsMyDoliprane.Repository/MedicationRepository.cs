using ItsMyDoliprane.Repository.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace ItsMyDoliprane.Repository;

public class MedicationRepository : AbstractRepository
{
    public MedicationRepository(IConfiguration configuration) : base(configuration) { }

    public List<Medication> GetMedicationsSinceDate(int personId, DateTime date) {
         using SqliteConnection connection = CreateConnectionAndOpen();
        using SqliteCommand command = connection.CreateCommand();
        List<Medication> medications = new List<Medication>();
        command.CommandText = @"SELECT PERSON_PKEY, DRUG_PKEY, strftime('%Y-%m-%d', DATETIME(DATE, 'unixepoch')),
                                    strftime('%H:%M', DATETIME(DATE, 'unixepoch'))
                                FROM MEDICATION 
                                WHERE PERSON_PKEY = $personId
                                  AND DATE > strftime('%s', $since)
                                ORDER BY DATE DESC";
        command.Parameters.AddWithValue("$personId", personId);
        command.Parameters.AddWithValue("$since", date.ToString("yyyy-MM-dd HH:mm:ss"));
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
            medications.Add(new Medication {
                            PersonId = reader.GetInt32(0),
                            DrugId = reader.GetInt32(1),
                            Date = reader.GetString(2),
                            Hour = reader.GetString(3)
                        });
        return medications; 
    }

    public void Add(Medication medication) {
        using SqliteConnection connection = CreateConnectionAndOpenWithForeignKeys();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO MEDICATION (PERSON_PKEY, DRUG_PKEY, DATE)
                                VALUES ($personId, $drugId, strftime('%s', $date));";
        command.Parameters.AddWithValue("$personId", medication.PersonId);
        command.Parameters.AddWithValue("$drugId", medication.DrugId);
        command.Parameters.AddWithValue("$date", $"{medication.Date} {medication.Hour}:00");
        command.ExecuteNonQuery();
    }
}
