using ItsMyDoliprane.Business.Enums;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business.Medications;

public class MedicationDoliprane : MedicationDrug
{
    private readonly MedicationAllDrug _medicationAllDrug;

    public MedicationDoliprane(MedicationAllDrug medicationAllDrug) {
        _medicationAllDrug = medicationAllDrug;
    }

    public override MedicationState GetMedicationState(List<Medication> medications, bool isAdult) {
        Medication? last = GetLastMedication(medications, DrugCompositionId.Paracetamol);
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
                nextMedicationsPossible.Add(last.DateTime.AddHours(isAdult ? 4 : 6));
                nextMedicationsYes.Add(last.DateTime.AddHours(6));
                break;
            case < 6:
                if (isAdult) {
                    opinions.Add(MedicationOpinion.Possible);
                    lastMedicationsNo.Add(last!.DateTime);
                    nextMedicationsYes.Add(last.DateTime.AddHours(6));
                }
                else {
                    opinions.Add(MedicationOpinion.No);
                    lastMedicationsNo.Add(last!.DateTime);
                    nextMedicationsPossible.Add(last.DateTime.AddHours(6));
                    nextMedicationsYes.Add(last.DateTime.AddHours(6));
                }
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
        MedicationState medicationStateAllDrug = _medicationAllDrug.GetMedicationState(medications, isAdult);
        opinions.Add(medicationStateAllDrug.Opinion);
        if (medicationStateAllDrug.LastMedicationNo != null)
            lastMedicationsNo.Add(medicationStateAllDrug.LastMedicationNo.Value);
        if (medicationStateAllDrug.NextMedicationPossible != null)
            nextMedicationsPossible.Add(medicationStateAllDrug.NextMedicationPossible.Value);
        if (medicationStateAllDrug.NextMedicationYes != null)
            nextMedicationsYes.Add(medicationStateAllDrug.NextMedicationYes.Value);
        return new MedicationState {
            DrugId = DrugId.Doliprane,
            Opinion = ChoiceMedicationOpinion(opinions),
            LastMedicationNo = MaxDateTime(lastMedicationsNo),
            NextMedicationPossible = MaxDateTime(nextMedicationsPossible),
            NextMedicationYes = MaxDateTime(nextMedicationsYes),
            Dosage = GetDosage(medications, DrugCompositionId.Paracetamol)
        };
    }

    private static MedicationOpinion GetDosageOpinion(List<Medication> medications) {
        int dosage = GetDosage(medications, DrugCompositionId.Paracetamol);
        return dosage switch {
            < 2500  => MedicationOpinion.Yes,
            <= 3000 => MedicationOpinion.Warning,
            _       => MedicationOpinion.No
        };
    }
}
