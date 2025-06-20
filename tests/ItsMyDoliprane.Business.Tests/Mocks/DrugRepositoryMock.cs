using System.Collections.Generic;
using ItsMyDoliprane.Repository.Boundary;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business.Tests.Mocks;

public class DrugRepositoryMock : IDrugRepository
{
    public List<Drug> GetDrugs() {
        throw new System.NotImplementedException();
    }

    public List<MedicationDosage> GetDosages(int drugId) {
        return new List<MedicationDosage>();
    }
}
