using System.Collections.Generic;
using ItsMyDoliprane.Business.Enums;
using ItsMyDoliprane.Repository.Boundary;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business.Tests.Mocks;

public class DrugRepositoryMock : IDrugRepository
{
    public List<Drug> GetDrugs() {
        throw new System.NotImplementedException();
    }

    public List<MedicationDosage> GetDosages(int drugId) {
        if (drugId == (int)DrugId.Smecta)
            return new List<MedicationDosage> {
                new() {
                    DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                    Quantity = 3000
                }
            };
        return new List<MedicationDosage>();
    }
}
