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
        RuleMedicationState ruleParacetamol = GetRuleIbuprofene(medications);
        if (ruleParacetamol.Opinion != null)
            opinions.Add(ruleParacetamol.Opinion.Value);
        if (ruleParacetamol.LastMedicationNo != null)
            lastMedicationsNo.Add(ruleParacetamol.LastMedicationNo.Value);
        if (ruleParacetamol.NextMedicationPossible != null)
            nextMedicationsPossible.Add(ruleParacetamol.NextMedicationPossible.Value);
        if (ruleParacetamol.NextMedicationYes != null)
            nextMedicationsYes.Add(ruleParacetamol.NextMedicationYes.Value);
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

    private static RuleMedicationState GetRuleIbuprofene(IEnumerable<Medication> medications) {
        List<Medication> medications24 = medications.ToList();
        Medication? lastIbuprofene = GetLastMedication(medications24, DrugId.Ibuprofene);
        Medication? lastParacetamol = GetLastMedication(medications24, DrugCompositionId.Paracetamol);
        float? durationSinceLastMedicationIbuprofene = GetDurationBetweenDateTime(lastIbuprofene?.DateTime, DateTime.Now);
        float? durationSinceLastMedicationParacetamol = GetDurationBetweenDateTime(lastParacetamol?.DateTime, DateTime.Now);
        switch (durationSinceLastMedicationIbuprofene) {
            case null: return new RuleMedicationState { Opinion = MedicationOpinion.Yes };
            case < 3:
                return new RuleMedicationState {
                    Opinion = MedicationOpinion.No,
                    LastMedicationNo = lastIbuprofene!.DateTime,
                    NextMedicationPossible = lastIbuprofene.DateTime.AddHours(4),
                    NextMedicationYes = lastIbuprofene.DateTime.AddHours(4)
                };
            case >= 3 and < 4: {
                MedicationOpinion option = durationSinceLastMedicationParacetamol switch {
                    null => MedicationOpinion.Warning,
                    >= 6 => MedicationOpinion.Warning,
                    _    => MedicationOpinion.No
                };
                return new RuleMedicationState {
                    Opinion = option,
                    LastMedicationNo = lastIbuprofene!.DateTime,
                    NextMedicationPossible = lastIbuprofene.DateTime.AddHours(4),
                    NextMedicationYes = lastIbuprofene.DateTime.AddHours(4)
                };
            }
            default: {
                if (durationSinceLastMedicationIbuprofene < durationSinceLastMedicationParacetamol) {
                    MedicationOpinion option = durationSinceLastMedicationParacetamol switch {
                        null => MedicationOpinion.Yes,
                        >= 6 => MedicationOpinion.Yes,
                        _    => MedicationOpinion.No
                    };
                    return new RuleMedicationState {
                        Opinion = option,
                        LastMedicationNo = option != MedicationOpinion.Yes ? lastIbuprofene!.DateTime : null,
                        NextMedicationPossible = option != MedicationOpinion.Yes ? lastParacetamol?.DateTime.AddHours(6) : null,
                        NextMedicationYes = option != MedicationOpinion.Yes ? lastParacetamol?.DateTime.AddHours(6) : null
                    };
                }
                return new RuleMedicationState { Opinion = MedicationOpinion.Yes };
            }
        }
    }
}
