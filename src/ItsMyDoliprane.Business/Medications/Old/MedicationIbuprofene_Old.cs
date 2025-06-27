using ItsMyDoliprane.Business.Enums;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Repository.Boundary;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business.Medications;

public class MedicationIbuprofene_Old : MedicationDrug
{
    private readonly MedicationAllDrug _medicationAllDrug;

    public MedicationIbuprofene_Old(MedicationAllDrug medicationAllDrug, IDrugRepository drugRepository) : base(drugRepository) {
        _medicationAllDrug = medicationAllDrug;
    }

    public override MedicationState GetMedicationState(List<Medication> medications, bool isAdult) {
        List<RuleMedicationState> rules = new List<RuleMedicationState> {
            GetRule8Hours(medications),
            GetRule3Drug(medications),
            GetRuleParacetamol(medications),
            GetRuleAllDrug(medications, isAdult)
        };
        return new MedicationState {
            DrugId = DrugId.Ibuprofene,
            Opinion = ChoiceMedicationOpinion(rules.Select(r => r.Opinion).ToList()),
            LastMedicationNo = MaxDateTime(rules.Select(r => r.LastMedicationNo).ToList()),
            NextMedicationPossible = MaxDateTime(rules.Select(r => r.NextMedicationPossible).ToList()),
            NextMedicationYes = MaxDateTime(rules.Select(r => r.NextMedicationYes).ToList()),
            NextDrug = null,
            Dosages = GetDosages(medications, DrugId.Ibuprofene),
            NumberMedication = GetNbDrug(medications)
        };
    }

    private static RuleMedicationState GetRule8Hours(IEnumerable<Medication> medications) {
        List<Medication> medications24 = medications.ToList();
        Medication? last = GetLastMedication(medications24, DrugId.Ibuprofene);
        Medication? lastParacetamol = GetLastMedication(medications24, DrugCompositionId.Paracetamol);
        float? durationSinceLastMedication = GetDurationBetweenDateTime(last?.DateTime, DateTime.Now);
        float? durationSinceLastMedicationParacetamol = GetDurationBetweenDateTime(lastParacetamol?.DateTime, DateTime.Now);
        switch (durationSinceLastMedication) {
            case null:
            case >= 8: return new RuleMedicationState { Opinion = MedicationOpinion.Yes };
            case >= 6:
                return new RuleMedicationState {
                    Opinion =
                        durationSinceLastMedicationParacetamol >= 3 && durationSinceLastMedicationParacetamol < durationSinceLastMedication
                            ? MedicationOpinion.Warning
                            : MedicationOpinion.No,
                    LastMedicationNo = last!.DateTime,
                    NextMedicationPossible = last.DateTime.AddHours(8),
                    NextMedicationYes = last.DateTime.AddHours(8)
                };
            default:
                return new RuleMedicationState {
                    Opinion = MedicationOpinion.No,
                    LastMedicationNo = last!.DateTime,
                    NextMedicationPossible = last.DateTime.AddHours(8),
                    NextMedicationYes = last.DateTime.AddHours(8)
                };
        }
    }

    private static RuleMedicationState GetRule3Drug(IEnumerable<Medication> medications) {
        List<Medication> medications24 = medications.ToList();
        int count = GetNbDrug(medications24, DrugId.Ibuprofene);
        Medication? last = GetLastMedication(medications24, DrugId.Ibuprofene);
        Medication? last3 = medications24.Where(m => m.DrugId == (int)DrugId.Ibuprofene).Skip(2).FirstOrDefault();
        return new RuleMedicationState {
            Opinion = count < 3 ? MedicationOpinion.Yes : MedicationOpinion.No,
            LastMedicationNo = count >= 3 ? last?.DateTime : null,
            NextMedicationPossible = last3?.DateTime.AddDays(1),
            NextMedicationYes = last3?.DateTime.AddDays(1)
        };
    }

    private static RuleMedicationState GetRuleParacetamol(IEnumerable<Medication> medications) {
        List<Medication> medications24 = medications.ToList();
        Medication? lastParacetamol = GetLastMedication(medications24, DrugCompositionId.Paracetamol);
        Medication? lastIbuprofene = GetLastMedication(medications24, DrugId.Ibuprofene);
        float? durationSinceLastMedicationParacetamol = GetDurationBetweenDateTime(lastParacetamol?.DateTime, DateTime.Now);
        float? durationSinceLastMedicationIbuprofene = GetDurationBetweenDateTime(lastIbuprofene?.DateTime, DateTime.Now);
        switch (durationSinceLastMedicationParacetamol) {
            case null: return new RuleMedicationState { Opinion = MedicationOpinion.Yes };
            case < 3:
                return new RuleMedicationState {
                    Opinion = MedicationOpinion.No,
                    LastMedicationNo = lastParacetamol!.DateTime,
                    NextMedicationPossible = lastParacetamol.DateTime.AddHours(4),
                    NextMedicationYes = lastParacetamol.DateTime.AddHours(4)
                };
            case >= 3 and < 4: {
                MedicationOpinion option = durationSinceLastMedicationIbuprofene switch {
                    null => MedicationOpinion.Warning,
                    >= 6 => MedicationOpinion.Warning,
                    _    => MedicationOpinion.No
                };
                return new RuleMedicationState {
                    Opinion = option,
                    LastMedicationNo = lastParacetamol!.DateTime,
                    NextMedicationPossible = lastParacetamol.DateTime.AddHours(4),
                    NextMedicationYes = lastParacetamol.DateTime.AddHours(4)
                };
            }
            default: {
                if (durationSinceLastMedicationParacetamol < durationSinceLastMedicationIbuprofene) {
                    MedicationOpinion option = durationSinceLastMedicationIbuprofene switch {
                        null => MedicationOpinion.Yes,
                        >= 8 => MedicationOpinion.Yes,
                        >= 6 => MedicationOpinion.Warning,
                        _    => MedicationOpinion.No
                    };
                    return new RuleMedicationState {
                        Opinion = option,
                        LastMedicationNo = option != MedicationOpinion.Yes ? lastParacetamol!.DateTime : null,
                        NextMedicationPossible = option != MedicationOpinion.Yes ? lastIbuprofene?.DateTime.AddHours(8) : null,
                        NextMedicationYes = option != MedicationOpinion.Yes ? lastIbuprofene?.DateTime.AddHours(8) : null
                    };
                }
                return new RuleMedicationState { Opinion = MedicationOpinion.Yes };
            }
        }
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
        return GetNbDrug(medications, DrugId.Ibuprofene);
    }
}
