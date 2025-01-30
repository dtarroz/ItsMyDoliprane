using ItsMyDoliprane.Business.Enums;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business.Medications;

public class MedicationHumex : MedicationDrug
{
    private class RuleMedicationStateHumex : RuleMedicationState
    {
        public bool BanHumexJour { get; init; }
        public bool BanHumexNuit { get; init; }
    }

    private readonly MedicationAllDrug _medicationAllDrug;

    public MedicationHumex(MedicationAllDrug medicationAllDrug) {
        _medicationAllDrug = medicationAllDrug;
    }

    public override MedicationState GetMedicationState(List<Medication> medications, bool isAdult) {
        List<RuleMedicationStateHumex> rules = new List<RuleMedicationStateHumex> {
            GetRule4Hours(medications),
            GetRule6HoursAfterHumexNuit(medications),
            GetRuleDosage(medications),
            GetRule1Nuit(medications),
            GetRule3Jour(medications),
            GetRuleIbuprofene(medications),
            GetRuleAllDrug(medications, isAdult)
        };
        rules.Add(GetBanDrug(rules, medications));
        return new MedicationState {
            DrugId = DrugId.Humex,
            Opinion = ChoiceMedicationOpinion(rules.Select(r => r.Opinion).ToList()),
            LastMedicationNo = MaxDateTime(rules.Select(r => r.LastMedicationNo).ToList()),
            NextMedicationPossible = MaxDateTime(rules.Select(r => r.NextMedicationPossible).ToList()),
            NextMedicationYes = MaxDateTime(rules.Select(r => r.NextMedicationYes).ToList()),
            NextDrug = ChoiceNextDrug(rules.Any(r => r.BanHumexJour), rules.Any(r => r.BanHumexNuit)),
            Dosage = GetDosage(medications, DrugCompositionId.Paracetamol),
            NumberMedication = GetNbDrug(medications)
        };
    }

    private static RuleMedicationStateHumex GetRule4Hours(List<Medication> medications) {
        Medication? last = GetLastMedication(medications, DrugCompositionId.Paracetamol);
        float? durationSinceLastMedication = GetDurationBetweenDateTime(last?.DateTime, DateTime.Now);
        switch (durationSinceLastMedication) {
            case null:
            case >= 4: return new RuleMedicationStateHumex { Opinion = MedicationOpinion.Yes };
            default:
                return new RuleMedicationStateHumex {
                    Opinion = MedicationOpinion.No,
                    LastMedicationNo = last!.DateTime,
                    NextMedicationPossible = last.DateTime.AddHours(4),
                    NextMedicationYes = last.DateTime.AddHours(4)
                };
        }
    }

    private static RuleMedicationStateHumex GetRule6HoursAfterHumexNuit(List<Medication> medications) {
        Medication? last = GetLastMedication(medications, DrugCompositionId.Paracetamol);
        if (last?.DrugId != (int)DrugId.HumexNuit)
            last = null;
        float? durationSinceLastMedication = GetDurationBetweenDateTime(last?.DateTime, DateTime.Now);
        switch (durationSinceLastMedication) {
            case null:
            case >= 6: return new RuleMedicationStateHumex { Opinion = MedicationOpinion.Yes };
            default:
                return new RuleMedicationStateHumex {
                    Opinion = MedicationOpinion.No,
                    LastMedicationNo = last!.DateTime,
                    NextMedicationPossible = last.DateTime.AddHours(6),
                    NextMedicationYes = last.DateTime.AddHours(6),
                    BanHumexNuit = true
                };
        }
    }

    private static RuleMedicationStateHumex GetRuleDosage(List<Medication> medications) {
        MedicationOpinion opinion = GetDosageOpinion(medications);
        Medication? last = GetLastMedication(medications, DrugCompositionId.Paracetamol);
        if (opinion == MedicationOpinion.Yes)
            return new RuleMedicationStateHumex { Opinion = opinion };
        List<Medication> currentMedications = medications.ToList();
        DateTime currentDateTime;
        do {
            currentDateTime = currentMedications.Last().DateTime;
            currentMedications.RemoveAt(currentMedications.Count - 1);
            if (GetDosageOpinion(currentMedications) == MedicationOpinion.Yes)
                break;
        } while (currentMedications.Count > 0);
        return new RuleMedicationStateHumex {
            Opinion = opinion,
            LastMedicationNo = last!.DateTime,
            NextMedicationPossible = currentDateTime.AddDays(1),
            NextMedicationYes = currentDateTime.AddDays(1)
        };
    }

    private static RuleMedicationStateHumex GetRule1Nuit(List<Medication> medications) {
        List<Medication> medications20 = FilterMedication20(medications);
        int count = medications20.Count(m => m.DrugId == (int)DrugId.HumexNuit);
        return count >= 1
            ? new RuleMedicationStateHumex { BanHumexNuit = true }
            : new RuleMedicationStateHumex { Opinion = MedicationOpinion.Yes };
    }

    private static RuleMedicationStateHumex GetRule3Jour(List<Medication> medications) {
        List<Medication> medications20 = FilterMedication20(medications);
        int count = medications20.Count(m => m.DrugId == (int)DrugId.HumexJour);
        Medication? last = GetLastMedication(medications20, DrugCompositionId.Paracetamol);
        Medication? last3 = medications20.Where(m => m.DrugId == (int)DrugId.HumexJour).Skip(3).FirstOrDefault();
        if (count >= 3)
            return new RuleMedicationStateHumex {
                BanHumexJour = true,
                BanHumexNuit = count > 3,
                LastMedicationNo = count > 3 ? last?.DateTime : null,
                NextMedicationPossible = last3?.DateTime.AddHours(20),
                NextMedicationYes = last3?.DateTime.AddHours(20)
            };
        return new RuleMedicationStateHumex { Opinion = MedicationOpinion.Yes };
    }

    private RuleMedicationStateHumex GetRuleAllDrug(List<Medication> medications, bool isAdult) {
        MedicationState state = _medicationAllDrug.GetMedicationState(medications, isAdult);
        return new RuleMedicationStateHumex {
            Opinion = state.Opinion,
            LastMedicationNo = state.LastMedicationNo,
            NextMedicationPossible = state.NextMedicationPossible,
            NextMedicationYes = state.NextMedicationYes
        };
    }

    private static RuleMedicationStateHumex GetRuleIbuprofene(IEnumerable<Medication> medications) {
        List<Medication> medications24 = medications.ToList();
        Medication? lastIbuprofene = GetLastMedication(medications24, DrugId.Ibuprofene);
        Medication? lastParacetamol = GetLastMedication(medications24, DrugCompositionId.Paracetamol);
        float? durationSinceLastMedicationIbuprofene = GetDurationBetweenDateTime(lastIbuprofene?.DateTime, DateTime.Now);
        float? durationSinceLastMedicationParacetamol = GetDurationBetweenDateTime(lastParacetamol?.DateTime, DateTime.Now);
        switch (durationSinceLastMedicationIbuprofene) {
            case null: return new RuleMedicationStateHumex { Opinion = MedicationOpinion.Yes };
            case < 3:
                return new RuleMedicationStateHumex {
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
                return new RuleMedicationStateHumex {
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
                    return new RuleMedicationStateHumex {
                        Opinion = option,
                        LastMedicationNo = option != MedicationOpinion.Yes ? lastIbuprofene!.DateTime : null,
                        NextMedicationPossible = option != MedicationOpinion.Yes ? lastParacetamol?.DateTime.AddHours(6) : null,
                        NextMedicationYes = option != MedicationOpinion.Yes ? lastParacetamol?.DateTime.AddHours(6) : null
                    };
                }
                return new RuleMedicationStateHumex { Opinion = MedicationOpinion.Yes };
            }
        }
    }

    private static RuleMedicationStateHumex GetBanDrug(List<RuleMedicationStateHumex> rules, List<Medication> medications) {
        bool banHumexJour = rules.Any(r => r.BanHumexJour);
        bool banHumexNuit = rules.Any(r => r.BanHumexNuit);
        List<Medication> medications20 = FilterMedication20(medications);
        Medication? last = GetLastMedication(medications20, DrugCompositionId.Paracetamol);
        Medication? first = medications20.LastOrDefault(m => m.DrugId is (int)DrugId.HumexJour or (int)DrugId.HumexNuit);
        return banHumexJour && banHumexNuit
            ? new RuleMedicationStateHumex {
                Opinion = MedicationOpinion.No,
                LastMedicationNo = last!.DateTime,
                NextMedicationPossible = first!.DateTime.AddHours(20),
                NextMedicationYes = first.DateTime.AddHours(20)
            }
            : new RuleMedicationStateHumex { Opinion = MedicationOpinion.Yes };
    }

    private DrugId? ChoiceNextDrug(bool banHumexJour, bool banHumexNuit) {
        switch (banHumexJour) {
            case true when banHumexNuit:
            case false when !banHumexNuit: return null;
            case true: return DrugId.HumexNuit;
            default:   return DrugId.HumexJour;
        }
    }

    private static MedicationOpinion GetDosageOpinion(List<Medication> medications) {
        int dosage = GetDosage(medications, DrugCompositionId.Paracetamol);
        return dosage switch {
            < 3000  => MedicationOpinion.Yes,
            <= 3500 => MedicationOpinion.Warning,
            _       => MedicationOpinion.No
        };
    }

    private static int GetNbDrug(IEnumerable<Medication> medications) {
        List<Medication> medications20 = FilterMedication20(medications);
        return GetNbDrug(medications20, DrugId.HumexJour) + GetNbDrug(medications20, DrugId.HumexNuit);
    }
}
