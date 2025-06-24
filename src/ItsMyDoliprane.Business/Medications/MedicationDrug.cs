using ItsMyDoliprane.Business.Enums;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Repository.Boundary;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business.Medications;

public abstract class MedicationDrug
{
    private readonly IDrugRepository _drugRepository;

    protected MedicationDrug(IDrugRepository drugRepository) {
        _drugRepository = drugRepository;
    }

    public abstract MedicationState GetMedicationState(List<Medication> medications, bool isAdult);

    protected static int GetDosage(IEnumerable<Medication> medications, DrugCompositionId drugCompositionId) {
        return GetDosage(medications, (int)drugCompositionId);
    }

    private static int GetDosage(IEnumerable<Medication> medications, int drugCompositionId) {
        return medications.SelectMany(m => m.Dosages).Where(d => d.DrugCompositionId == drugCompositionId).Sum(d => d.Quantity);
    }

    protected static float? GetDurationBetweenDateTime(DateTime? start, DateTime? end) {
        return start != null && end != null ? (float?)(end.Value - start.Value).TotalHours : null;
    }

    protected static Medication? GetLastMedication(IEnumerable<Medication> medications, DrugCompositionId drugCompositionId) {
        return medications.FirstOrDefault(m => m.Dosages.Any(d => d.DrugCompositionId == (int)drugCompositionId));
    }

    protected static Medication? GetLastMedication(IEnumerable<Medication> medications, DrugId drugId) {
        return medications.FirstOrDefault(m => m.DrugId == (int)drugId);
    }

    protected static DateTime? MaxDateTime(IEnumerable<DateTime?> datetimes) {
        List<DateTime> filteredDateTime = datetimes.Where(d => d != null).Select(d => d!.Value).ToList();
        return MaxDateTime(filteredDateTime);
    }

    protected static DateTime? MaxDateTime(List<DateTime> datetimes) {
        return datetimes.Any() ? datetimes.Max() : null;
    }

    protected static MedicationOpinion ChoiceMedicationOpinion(List<MedicationOpinion?> opinions) {
        List<MedicationOpinion> filteredMedicationOpinion = opinions.Where(o => o != null).Select(o => o!.Value).ToList();
        return ChoiceMedicationOpinion(filteredMedicationOpinion);
    }

    protected static MedicationOpinion ChoiceMedicationOpinion(List<MedicationOpinion> opinions) {
        if (opinions.Contains(MedicationOpinion.No))
            return MedicationOpinion.No;
        if (opinions.Contains(MedicationOpinion.Warning))
            return MedicationOpinion.Warning;
        return opinions.Contains(MedicationOpinion.Possible) ? MedicationOpinion.Possible : MedicationOpinion.Yes;
    }

    protected static int GetNbDrug(IEnumerable<Medication> medications, DrugId drug) {
        return medications.Count(d => d.DrugId == (int)drug);
    }

    protected static int GetNbDrugComposition(IEnumerable<Medication> medications, DrugCompositionId drugComposition) {
        return GetNbDrugComposition(medications, (int)drugComposition);
    }

    private static int GetNbDrugComposition(IEnumerable<Medication> medications, int drugComposition) {
        return medications.Count(m => m.Dosages.Any(d => d.DrugCompositionId == drugComposition));
    }

    protected static List<Medication> FilterMedication20(IEnumerable<Medication> medications) {
        return medications.Where(m => m.DateTime > DateTime.Now.AddHours(-20)).ToList();
    }

    protected static List<Medication> FilterMedication(IEnumerable<Medication> medications, int hour) {
        return medications.Where(m => m.DateTime > DateTime.Now.AddHours(-1 * hour)).ToList();
    }

    protected List<MedicationStateDosage> GetDosages(List<Medication> medications, DrugId drug) {
        List<MedicationDosage> dosages = _drugRepository.GetDosages((int)drug);
        return dosages.Select(dosage => new MedicationStateDosage {
                                  DrugCompositionId = (DrugCompositionId)dosage.DrugCompositionId,
                                  TotalQuantity = GetDosage(medications, dosage.DrugCompositionId),
                                  Number = GetNbDrugComposition(medications, dosage.DrugCompositionId)
                              })
                      .ToList();
    }
}
