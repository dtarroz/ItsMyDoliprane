using ItsMyDoliprane.Business.Enums;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business.Medications;

public class MedicationHumex : MedicationDrug
{
    private class RuleMedicationState
    {
        public MedicationOpinion? Opinion { get; init; }
        public DateTime? LastMedicationNo { get; init; }
        public DateTime? NextMedicationPossible { get; init; }
        public DateTime? NextMedicationYes { get; init; }
        public bool BanHumexJour { get; init; }
        public bool BanHumexNuit { get; init; }
    }

    public override MedicationState GetMedicationState(List<Medication> medications) {
        List<RuleMedicationState> rules = new List<RuleMedicationState> {
            GetRule4Hours(medications),
            GetRule6HoursAfterHumexNuit(medications),
            GetRuleDosage(medications),
            GetRule1Nuit(medications),
            GetRule3Jour(medications)
        };
        rules.Add(GetBanDrug(rules, medications));
        return new MedicationState {
            DrugId = DrugId.Humex,
            Opinion = ChoiceMedicationOpinion(rules.Select(r => r.Opinion).ToList()),
            LastMedicationNo = MaxDateTime(rules.Select(r => r.LastMedicationNo).ToList()),
            NextMedicationPossible = MaxDateTime(rules.Select(r => r.NextMedicationPossible).ToList()),
            NextMedicationYes = MaxDateTime(rules.Select(r => r.NextMedicationYes).ToList()),
            NextDrug = ChoiceNextDrug(rules.Any(r => r.BanHumexJour), rules.Any(r => r.BanHumexNuit)),
            Dosage = GetDosage(medications, DrugCompositionId.Paracetamol)
        };
    }

    private static List<Medication> FilterMedication20(List<Medication> medications) {
        return medications.Where(m => m.DateTime > DateTime.Now.AddHours(-20)).ToList();
    }

    private static RuleMedicationState GetRule4Hours(List<Medication> medications) {
        Medication? last = GetLastMedication(medications, DrugCompositionId.Paracetamol);
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

    private static RuleMedicationState GetRule6HoursAfterHumexNuit(List<Medication> medications) {
        Medication? last = GetLastMedication(medications, DrugCompositionId.Paracetamol);
        if (last?.DrugId != (int)DrugId.HumexNuit)
            last = null;
        float? durationSinceLastMedication = GetDurationBetweenDateTime(last?.DateTime, DateTime.Now);
        switch (durationSinceLastMedication) {
            case null:
            case >= 6: return new RuleMedicationState { Opinion = MedicationOpinion.Yes };
            default:
                return new RuleMedicationState {
                    Opinion = MedicationOpinion.No,
                    LastMedicationNo = last!.DateTime,
                    NextMedicationPossible = last.DateTime.AddHours(6),
                    NextMedicationYes = last.DateTime.AddHours(6),
                    BanHumexNuit = true
                };
        }
    }

    private static RuleMedicationState GetRuleDosage(List<Medication> medications) {
        MedicationOpinion opinion = GetDosageOpinion(medications);
        Medication? last = GetLastMedication(medications, DrugCompositionId.Paracetamol);
        if (opinion == MedicationOpinion.Yes)
            return new RuleMedicationState { Opinion = opinion };
        List<Medication> currentMedications = medications.ToList();
        DateTime currentDateTime;
        do {
            currentDateTime = currentMedications.Last().DateTime;
            currentMedications.RemoveAt(currentMedications.Count - 1);
            if (GetDosageOpinion(currentMedications) == MedicationOpinion.Yes)
                break;
        } while (currentMedications.Count > 0);
        return new RuleMedicationState {
            Opinion = opinion,
            LastMedicationNo = last!.DateTime,
            NextMedicationPossible = currentDateTime.AddDays(1),
            NextMedicationYes = currentDateTime.AddDays(1)
        };
    }

    private static RuleMedicationState GetRule1Nuit(List<Medication> medications) {
        List<Medication> medications20 = FilterMedication20(medications);
        int count = medications20.Count(m => m.DrugId == (int)DrugId.HumexNuit);
        return count >= 1 ? new RuleMedicationState { BanHumexNuit = true } : new RuleMedicationState { Opinion = MedicationOpinion.Yes };
    }

    private static RuleMedicationState GetRule3Jour(List<Medication> medications) {
        List<Medication> medications20 = FilterMedication20(medications);
        int count = medications20.Count(m => m.DrugId == (int)DrugId.HumexJour);
        Medication? last = GetLastMedication(medications20, DrugCompositionId.Paracetamol);
        Medication? last3 = medications20.Where(m => m.DrugId == (int)DrugId.HumexJour).Skip(3).FirstOrDefault();
        if (count >= 3)
            return new RuleMedicationState {
                BanHumexJour = true,
                BanHumexNuit = count > 3,
                LastMedicationNo = count > 3 ? last?.DateTime : null,
                NextMedicationPossible = last3?.DateTime.AddHours(20),
                NextMedicationYes = last3?.DateTime.AddHours(20)
            };
        return new RuleMedicationState { Opinion = MedicationOpinion.Yes };
    }

    private static RuleMedicationState GetBanDrug(List<RuleMedicationState> rules, List<Medication> medications) {
        bool banHumexJour = rules.Any(r => r.BanHumexJour);
        bool banHumexNuit = rules.Any(r => r.BanHumexNuit);
        List<Medication> medications20 = FilterMedication20(medications);
        Medication? last = GetLastMedication(medications20, DrugCompositionId.Paracetamol);
        Medication? first = medications20.LastOrDefault(m => m.DrugId is (int)DrugId.HumexJour or (int)DrugId.HumexNuit);
        return banHumexJour && banHumexNuit
            ? new RuleMedicationState {
                Opinion = MedicationOpinion.No,
                LastMedicationNo = last!.DateTime,
                NextMedicationPossible = first!.DateTime.AddHours(20),
                NextMedicationYes = first.DateTime.AddHours(20)
            }
            : new RuleMedicationState { Opinion = MedicationOpinion.Yes };
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
}
