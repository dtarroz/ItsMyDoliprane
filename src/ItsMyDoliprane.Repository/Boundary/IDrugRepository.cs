using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Repository.Boundary;

public interface IDrugRepository
{
    List<Drug> GetDrugs();
    List<MedicationDosage> GetDosages(int drugId);
}
