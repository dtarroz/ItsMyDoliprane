using ItsMyDoliprane.Business.Enums;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Repository.Boundary;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business.Medications;

public class MedicationAllDrug : MedicationDrug
{
    public MedicationAllDrug(IDrugRepository drugRepository) : base(drugRepository) { }

    public override MedicationState GetMedicationState(List<Medication> medications, bool isAdult) {
        List<RuleMedicationState> rules = new List<RuleMedicationState> { GetRule2Hours(medications) };
        return new MedicationState {
            DrugId = null,
            Opinion = ChoiceMedicationOpinion(rules.Select(r => r.Opinion).ToList()),
            LastMedicationNo = MaxDateTime(rules.Select(r => r.LastMedicationNo).ToList()),
            NextMedicationPossible = MaxDateTime(rules.Select(r => r.NextMedicationPossible).ToList()),
            NextMedicationYes = MaxDateTime(rules.Select(r => r.NextMedicationYes).ToList()),
            NextDrug = null,
            Dosages = new List<MedicationStateDosage>(),
            NumberMedication = 0
        };
    }

    private static RuleMedicationState GetRule2Hours(IEnumerable<Medication> medications) {
        Medication? last = GetLastMedication(medications, DrugCompositionId.Diosmectite);
        float? durationSinceLastMedication = GetDurationBetweenDateTime(last?.DateTime, DateTime.Now);
        switch (durationSinceLastMedication) {
            case null:
            case >= 2:
                return new RuleMedicationState {
                    Opinion = MedicationOpinion.Yes
                };
            default:
                return new RuleMedicationState {
                    Opinion = MedicationOpinion.No,
                    LastMedicationNo = last!.DateTime,
                    NextMedicationPossible = last.DateTime.AddHours(2),
                    NextMedicationYes = last.DateTime.AddHours(2)
                };
        }
    }
}
