using ItsMyDoliprane.Business.Enums;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business.Medications;

public abstract class MedicationDrug
{
    public abstract MedicationState GetMedicationState(List<Medication> medications);

    protected static int GetDosage(IEnumerable<Medication> medications, DrugCompositionId drugCompositionId) {
        return medications.SelectMany(m => m.Dosages).Where(d => d.DrugCompositionId == (int)drugCompositionId).Sum(d => d.Quantity);
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
        return medications.Count(m => m.Dosages.Any(d => d.DrugCompositionId == (int)drugComposition));
    }

    protected static List<Medication> FilterMedication20(IEnumerable<Medication> medications) {
        return medications.Where(m => m.DateTime > DateTime.Now.AddHours(-20)).ToList();
    }
}
