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
        List<Medication> medications = GetHeaderMedicationsSinceDate(personId, date, command);
        foreach (Medication medication in medications)
            medication.Dosages = GetDosages(medication.DrugId, command);
        return medications;
    }

    private static List<MedicationDosage> GetDosages(int drugId, SqliteCommand command) {
        List<MedicationDosage> dosages = new List<MedicationDosage>();
        command.CommandText = @"SELECT DRUG_COMPOSITION_PKEY, QUANTITY 
                                FROM DRUG_DOSAGE 
                                WHERE DRUG_PKEY = $drugId";
        command.Parameters.Clear();
        command.Parameters.AddWithValue("$drugId", drugId);
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
            dosages.Add(new MedicationDosage {
                            DrugCompositionId = reader.GetInt32(0),
                            Quantity = reader.GetInt32(1)
                        });
        return dosages;
    }

    private static List<Medication> GetHeaderMedicationsSinceDate(int personId, DateTime date, SqliteCommand command) {
        List<Medication> medications = new List<Medication>();
        command.CommandText = @"SELECT PKEY, DRUG_PKEY, strftime('%Y-%m-%d', DATETIME(DATE, 'unixepoch')),
                                    strftime('%H:%M', DATETIME(DATE, 'unixepoch')),
                                    DATETIME(DATE, 'unixepoch') 
                                FROM MEDICATION 
                                WHERE PERSON_PKEY = $personId
                                  AND DATE > strftime('%s', $since)
                                ORDER BY DATE DESC";
        command.Parameters.Clear();
        command.Parameters.AddWithValue("$personId", personId);
        command.Parameters.AddWithValue("$since", date.ToString("yyyy-MM-dd HH:mm:ss"));
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
            medications.Add(new Medication {
                Id = reader.GetInt32(0),
                DrugId = reader.GetInt32(1), 
                DateTime = reader.GetDateTime(4)
            });
        return medications;
    }

    public void Add(NewMedication newMedication) {
        using SqliteConnection connection = CreateConnectionAndOpenWithForeignKeys();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO MEDICATION (PERSON_PKEY, DRUG_PKEY, DATE)
                                VALUES ($personId, $drugId, strftime('%s', $date));";
        command.Parameters.AddWithValue("$personId", newMedication.PersonId);
        command.Parameters.AddWithValue("$drugId", newMedication.DrugId);
        command.Parameters.AddWithValue("$date", $"{newMedication.Date} {newMedication.Hour}:00");
        command.ExecuteNonQuery();
    }

    public void Delete(int medicationId) {
        using SqliteConnection connection = CreateConnectionAndOpenWithForeignKeys();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM MEDICATION WHERE PKEY = $medicationId";
        command.Parameters.AddWithValue("$medicationId", medicationId);
        command.ExecuteNonQuery();
    }
}
