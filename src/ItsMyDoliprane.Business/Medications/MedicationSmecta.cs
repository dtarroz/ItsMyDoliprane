using ItsMyDoliprane.Business.Enums;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business.Medications;

public class MedicationSmecta : MedicationDrug
{
    public override MedicationState GetMedicationState(List<Medication> medications) {
        List<RuleMedicationState> rules = new List<RuleMedicationState> {
            GetRule4Hours(medications),
            GetRule2Hours(medications),
            GetRule4Drug(medications)
        };
        return new MedicationState {
            DrugId = DrugId.Smecta,
            Opinion = ChoiceMedicationOpinion(rules.Select(r => r.Opinion).ToList()),
            LastMedicationNo = MaxDateTime(rules.Select(r => r.LastMedicationNo).ToList()),
            NextMedicationPossible = MaxDateTime(rules.Select(r => r.NextMedicationPossible).ToList()),
            NextMedicationYes = MaxDateTime(rules.Select(r => r.NextMedicationYes).ToList()),
            NextDrug = null,
            Dosage = GetNbDrug(medications)
        };
    }

    private static RuleMedicationState GetRule4Hours(IEnumerable<Medication> medications) {
        Medication? last = GetLastMedication(medications, DrugCompositionId.Diosmectite);
        float? durationSinceLastMedication = GetDurationBetweenDateTime(last?.DateTime, DateTime.Now);
        switch (durationSinceLastMedication) {
            case null:
            case >= 4: return new RuleMedicationState { Opinion = MedicationOpinion.Yes };
            default:
                return new RuleMedicationState {
                    Opinion = MedicationOpinion.No,
                    LastMedicationNo = last!.DateTime,
                    NextMedicationPossible = last.DateTime.AddHours(4),
                    NextMedicationYes = last.DateTime.AddHours(4)
                };
        }
    }

    private static RuleMedicationState GetRule2Hours(IEnumerable<Medication> medications) {
        Medication? last = medications.FirstOrDefault(m => m.Dosages.All(d => d.DrugCompositionId != (int)DrugCompositionId.Diosmectite));
        float? durationSinceLastMedication = GetDurationBetweenDateTime(last?.DateTime, DateTime.Now);
        switch (durationSinceLastMedication) {
            case null:
            case >= 2: return new RuleMedicationState { Opinion = MedicationOpinion.Yes };
            default:
                return new RuleMedicationState {
                    Opinion = MedicationOpinion.No,
                    LastMedicationNo = last!.DateTime,
                    NextMedicationPossible = last.DateTime.AddHours(2),
                    NextMedicationYes = last.DateTime.AddHours(2)
                };
        }
    }

    private static RuleMedicationState GetRule4Drug(IEnumerable<Medication> medications) {
        List<Medication> medications20 = FilterMedication20(medications);
        int count = GetNbDrugComposition(medications20, DrugCompositionId.Diosmectite);
        Medication? last = GetLastMedication(medications20, DrugCompositionId.Diosmectite);
        Medication? last4 = medications20.Where(m => m.Dosages.Any(d => d.DrugCompositionId == (int)DrugCompositionId.Diosmectite))
                                         .Skip(3)
                                         .FirstOrDefault();
        return new RuleMedicationState {
            Opinion = count < 4 ? MedicationOpinion.Yes : MedicationOpinion.No,
            LastMedicationNo = count >= 4 ? last?.DateTime : null,
            NextMedicationPossible = last4?.DateTime.AddHours(20),
            NextMedicationYes = last4?.DateTime.AddHours(20)
        };
    }

    private static int GetNbDrug(IEnumerable<Medication> medications) {
        List<Medication> medications20 = FilterMedication20(medications);
        return GetNbDrugComposition(medications20, DrugCompositionId.Diosmectite);
    }
}
