using ItsMyDoliprane.Business.Enums;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Repository.Boundary;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business.Medications;

public class MedicationAntibiotique_Old : MedicationDrug
{
    private readonly MedicationAllDrug _medicationAllDrug;

    public MedicationAntibiotique_Old(MedicationAllDrug medicationAllDrug, IDrugRepository drugRepository) : base(drugRepository) {
        _medicationAllDrug = medicationAllDrug;
    }

    public override MedicationState GetMedicationState(List<Medication> medications, bool isAdult) {
        List<RuleMedicationState> rules = new List<RuleMedicationState> {
            GetRule4Hours(medications),
            GetRule3Drug(medications),
            GetRuleAllDrug(medications, isAdult)
        };
        return new MedicationState {
            DrugId = DrugId.Antibiotique,
            Opinion = ChoiceMedicationOpinion(rules.Select(r => r.Opinion).ToList()),
            LastMedicationNo = MaxDateTime(rules.Select(r => r.LastMedicationNo).ToList()),
            NextMedicationPossible = MaxDateTime(rules.Select(r => r.NextMedicationPossible).ToList()),
            NextMedicationYes = MaxDateTime(rules.Select(r => r.NextMedicationYes).ToList()),
            NextDrug = null,
            Dosages = GetDosages(FilterMedication20(medications), DrugId.Antibiotique),
            NumberMedication = GetNbDrug(medications)
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
        List<Medication> medications20 = FilterMedication20(medications);
        return GetNbDrug(medications20, DrugId.Antibiotique);
    }
}
