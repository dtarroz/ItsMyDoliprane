using ItsMyDoliprane.Business.Enums;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business.Medications;

public class MedicationAntibiotique : MedicationDrug
{
    public override MedicationState GetMedicationState(List<Medication> medications) {
        List<RuleMedicationState> rules = new List<RuleMedicationState> {
            GetRule4Hours(medications),
            GetRule3Drug(medications)
        };
        return new MedicationState {
            DrugId = DrugId.Antibiotique,
            Opinion = ChoiceMedicationOpinion(rules.Select(r => r.Opinion).ToList()),
            LastMedicationNo = MaxDateTime(rules.Select(r => r.LastMedicationNo).ToList()),
            NextMedicationPossible = MaxDateTime(rules.Select(r => r.NextMedicationPossible).ToList()),
            NextMedicationYes = MaxDateTime(rules.Select(r => r.NextMedicationYes).ToList()),
            NextDrug = null,
            Dosage = GetNbDrug(medications)
        };
    }

    private static RuleMedicationState GetRule4Hours(IEnumerable<Medication> medications) {
        Medication? last = GetLastMedication(medications, DrugId.Antibiotique);
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

    private static RuleMedicationState GetRule3Drug(IReadOnlyCollection<Medication> medications) {
        List<Medication> medications20 = FilterMedication20(medications);
        int count = GetNbDrug(medications20, DrugId.Antibiotique);
        Medication? last = GetLastMedication(medications20, DrugId.Antibiotique);
        Medication? last3 = medications20.Where(m => m.DrugId == (int)DrugId.Antibiotique).Skip(2).FirstOrDefault();
        return new RuleMedicationState {
            Opinion = count < 3 ? MedicationOpinion.Yes : MedicationOpinion.No,
            LastMedicationNo = count >= 3 ? last?.DateTime : null,
            NextMedicationPossible = last3?.DateTime.AddHours(20),
            NextMedicationYes = last3?.DateTime.AddHours(20)
        };
    }

    private static int GetNbDrug(IEnumerable<Medication> medications) {
        List<Medication> medications20 = FilterMedication20(medications);
        return GetNbDrug(medications20, DrugId.Antibiotique);
    }
}
