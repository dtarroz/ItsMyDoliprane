using ItsMyDoliprane.Repository.Models;
using Microsoft.Data.Sqlite;

namespace ItsMyDoliprane.Repository.Tables;

internal static class DrugDosageTable
{
    public static List<MedicationDosage> GetDosages(int drugId, SqliteCommand command) {
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
}
