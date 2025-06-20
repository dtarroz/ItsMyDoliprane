using ItsMyDoliprane.Business.Enums;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Repository.Boundary;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business.Medications;

public class MedicationTopalgic : MedicationDrug
{
    private readonly MedicationAllDrug _medicationAllDrug;

    public MedicationTopalgic(MedicationAllDrug medicationAllDrug, IDrugRepository drugRepository) : base(drugRepository) {
        _medicationAllDrug = medicationAllDrug;
    }

    public override MedicationState GetMedicationState(List<Medication> medications, bool isAdult) {
        List<RuleMedicationState> rules = new List<RuleMedicationState> {
            GetRule6_8Hours(medications),
            GetRule4Drug(medications),
            GetRuleAllDrug(medications, isAdult)
        };
        return new MedicationState {
            DrugId = DrugId.Topalgic,
            Opinion = ChoiceMedicationOpinion(rules.Select(r => r.Opinion).ToList()),
            LastMedicationNo = MaxDateTime(rules.Select(r => r.LastMedicationNo).ToList()),
            NextMedicationPossible = MaxDateTime(rules.Select(r => r.NextMedicationPossible).ToList()),
            NextMedicationYes = MaxDateTime(rules.Select(r => r.NextMedicationYes).ToList()),
            NextDrug = null,
            Dosage = GetNbDrug(medications),
            Dosages = GetDosages(medications, DrugId.Topalgic),
            NumberMedication = GetNbDrug(medications)
        };
    }

    private static RuleMedicationState GetRule6_8Hours(IEnumerable<Medication> medications) {
        List<Medication> medications24 = medications.ToList();
        Medication? last = GetLastMedication(medications24, DrugCompositionId.Tramadol);
        float? durationSinceLastMedication = GetDurationBetweenDateTime(last?.DateTime, DateTime.Now);
        switch (durationSinceLastMedication) {
            case null:
            case >= 8: return new RuleMedicationState { Opinion = MedicationOpinion.Yes };
            case >= 6:
                return new RuleMedicationState {
                    Opinion = MedicationOpinion.Possible,
                    LastMedicationNo = last!.DateTime,
                    NextMedicationYes = last.DateTime.AddHours(8)
                };
            default:
                return new RuleMedicationState {
                    Opinion = MedicationOpinion.No,
                    LastMedicationNo = last!.DateTime,
                    NextMedicationPossible = last.DateTime.AddHours(6),
                    NextMedicationYes = last.DateTime.AddHours(8)
                };
        }
    }

    private static RuleMedicationState GetRule4Drug(IEnumerable<Medication> medications) {
        List<Medication> medications24 = medications.ToList();
        int count = GetNbDrug(medications24, DrugId.Topalgic);
        Medication? last = GetLastMedication(medications24, DrugId.Ibuprofene);
        Medication? last4 = medications24.Where(m => m.DrugId == (int)DrugId.Topalgic).Skip(3).FirstOrDefault();
        return new RuleMedicationState {
            Opinion = count < 4 ? MedicationOpinion.Yes : MedicationOpinion.No,
            LastMedicationNo = count >= 4 ? last?.DateTime : null,
            NextMedicationPossible = last4?.DateTime.AddDays(1),
            NextMedicationYes = last4?.DateTime.AddDays(1)
        };
    }

    private RuleMedicationState GetRuleAllDrug(List<Medication> medications, bool isAdult) {
        MedicationState state = _medicationAllDrug.GetMedicationState(medications, isAdult);
        return new RuleMedicationState {
            Opinion = state.Opinion,
            LastMedicationNo = state.LastMedicationNo,
            NextMedicationPossible = state.NextMedicationPossible,
            NextMedicationYes = state.NextMedicationYes
        };
    }

    private static int GetNbDrug(IEnumerable<Medication> medications) {
        return GetNbDrug(medications, DrugId.Topalgic);
    }
}
