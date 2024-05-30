using ItsMyDoliprane.Business.Enums;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business.Medications;

internal static class MedicationDoliprane
{
    public static MedicationState GetMedicationState(List<Medication> medications) {
        Medication? last = GetLastMedication(medications);
        float? durationSinceLastMedication = GetDurationBetweenDateTime(last?.DateTime, DateTime.Now);
        List<MedicationOpinion> opinions = new List<MedicationOpinion>();
        List<DateTime> lastMedicationsNo = new List<DateTime>();
        List<DateTime> nextMedicationsPossible = new List<DateTime>();
        List<DateTime> nextMedicationsYes = new List<DateTime>();
        switch (durationSinceLastMedication) {
            case null:
                opinions.Add(MedicationOpinion.Yes);
                break;
            case < 4:
                opinions.Add(MedicationOpinion.No);
                lastMedicationsNo.Add(last!.DateTime);
                nextMedicationsPossible.Add(last.DateTime.AddHours(4));
                nextMedicationsYes.Add(last.DateTime.AddHours(6));
                break;
            case < 6:
                opinions.Add(MedicationOpinion.Possible);
                lastMedicationsNo.Add(last!.DateTime);
                nextMedicationsYes.Add(last.DateTime.AddHours(6));
                break;
            default:
                opinions.Add(MedicationOpinion.Yes);
                break;
        }
        MedicationOpinion dosageOpinion = GetDosageOpinion(medications);
        opinions.Add(dosageOpinion);
        if (dosageOpinion != MedicationOpinion.Yes) {
            List<Medication> currentMedication = medications.ToList();
            while (currentMedication.Count > 0) {
                DateTime currentDateTime = currentMedication.Last().DateTime;
                currentMedication.RemoveAt(currentMedication.Count - 1);
                if (GetDosageOpinion(currentMedication) == MedicationOpinion.Yes) {
                    lastMedicationsNo.Add(last!.DateTime);
                    nextMedicationsPossible.Add(currentDateTime.AddDays(1));
                    nextMedicationsYes.Add(currentDateTime.AddDays(1));
                    break;
                }
            }
        }
        return new MedicationState {
            DrugId = DrugId.Doliprane,
            Opinion = ChoiceMedicationOpinion(opinions),
            LastMedicationNo = MaxDateTime(lastMedicationsNo),
            NextMedicationPossible = MaxDateTime(nextMedicationsPossible),
            NextMedicationYes = MaxDateTime(nextMedicationsYes)
        };
    }

    private static Medication? GetLastMedication(IEnumerable<Medication> medications) {
        return medications.FirstOrDefault(m => m.Dosages.Any(d => d.DrugCompositionId == (int)DrugCompositionId.Paracetamol));
    }

    private static float? GetDurationBetweenDateTime(DateTime? start, DateTime? end) {
        return start != null && end != null ? (float?)(end.Value - start.Value).TotalHours : null;
    }

    private static MedicationOpinion GetDosageOpinion(List<Medication> medications) {
        int dosage = GetDosage(medications);
        return dosage switch {
            < 2500  => MedicationOpinion.Yes,
            <= 3000 => MedicationOpinion.Warning,
            _       => MedicationOpinion.No
        };
    }

    private static int GetDosage(IEnumerable<Medication> medications) {
        return medications.SelectMany(m => m.Dosages)
                          .Where(d => d.DrugCompositionId == (int)DrugCompositionId.Paracetamol)
                          .Sum(d => d.Quantity);
    }

    private static MedicationOpinion ChoiceMedicationOpinion(ICollection<MedicationOpinion> opinion) {
        if (opinion.Contains(MedicationOpinion.No))
            return MedicationOpinion.No;
        if (opinion.Contains(MedicationOpinion.Warning))
            return MedicationOpinion.Warning;
        return opinion.Contains(MedicationOpinion.Possible) ? MedicationOpinion.Possible : MedicationOpinion.Yes;
    }

    private static DateTime? MaxDateTime(IReadOnlyCollection<DateTime> datetimes) {
        return datetimes.Count > 0 ? datetimes.Max() : null;
    }
}
