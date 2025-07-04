﻿using ItsMyDoliprane.Repository.Boundary;
using ItsMyDoliprane.Repository.Models;
using ItsMyDoliprane.Repository.Tables;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace ItsMyDoliprane.Repository;

public class DrugRepository : AbstractRepository, IDrugRepository
{
    public DrugRepository(IConfiguration configuration) : base(configuration) { }

    public List<Drug> GetDrugs() {
        using SqliteConnection connection = CreateConnectionAndOpen();
        using SqliteCommand command = connection.CreateCommand();
        List<Drug> drugs = new List<Drug>();
        command.CommandText = "SELECT PKEY, NAME, VISIBLE, FOR_ADULT, FOR_CHILD FROM DRUG";
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
            drugs.Add(new Drug {
                          Id = reader.GetInt32(0),
                          Name = reader.GetString(1),
                          Visible = reader.GetInt32(2) == 1,
                          ForAdult = reader.GetInt32(3) == 1,
                          ForChild = reader.GetInt32(4) == 1
                      });
        return drugs;
    }

    public List<MedicationDosage> GetDosages(int drugId) {
        using SqliteConnection connection = CreateConnectionAndOpen();
        using SqliteCommand command = connection.CreateCommand();
        return DrugDosageTable.GetDosages(drugId, command);
    }
}
