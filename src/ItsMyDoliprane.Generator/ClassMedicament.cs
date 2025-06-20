using System.Collections.Generic;
using System.Linq;

namespace ItsMyDoliprane.Generator
{
    public static class ClassMedicament
    {
        public static Dictionary<string, string> ConvertToClassFile(List<FileMedicament> fileMedicaments, List<string> drugId,
                                                                    List<string> drugCompositionId) {
            return fileMedicaments.ToDictionary(fileMedicament => $"{GetClassName(fileMedicament.Nom)}.g.cs",
                                                fileMedicament => ConvertTo(fileMedicament, drugId, drugCompositionId));
        }

        private static string GetClassName(string nom) {
            return $"Medication{nom}";
        }

        private static string ConvertTo(FileMedicament fileMedicament, List<string> drugId, List<string> drugCompositionId) {
            MethodPosologie methodPosologieAdulte = GetMethodPosologie(GetPosologie(fileMedicament, "Adulte"));
            MethodPosologie methodPosologieEnfant = GetMethodPosologie(GetPosologie(fileMedicament, "Enfant"));
            return Generate(fileMedicament.Nom, methodPosologieAdulte, methodPosologieEnfant);
        }

        private static FilePosologie GetPosologie(FileMedicament fileMedicament, string categorie) {
            return fileMedicament.Posologies.FirstOrDefault(p => p.Categorie == categorie);
        }

        private static MethodPosologie GetMethodPosologie(FilePosologie posologie) {
            if (posologie == null)
                return GetMethodPosologieEmpty();
            return new MethodPosologie { MedicationFilterHour = posologie.DureeHeures };
        }

        private static MethodPosologie GetMethodPosologieEmpty() {
            return new MethodPosologie { MedicationFilterHour = 24 };
        }

        private static string Generate(string nom, MethodPosologie methodPosologieAdulte, MethodPosologie methodPosologieEnfant) {
            return $@"
using ItsMyDoliprane.Business.Enums;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Repository;
using ItsMyDoliprane.Repository.Boundary;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business.Medications;

public class {GetClassName(nom)} : MedicationDrug
{{
    public {GetClassName(nom)}(IDrugRepository drugRepository) : base(drugRepository) {{}}

    public override MedicationState GetMedicationState(List<Medication> medications, bool isAdult) {{
        List<RuleMedicationState> rules;
        List<Medication> filteredMedications;
        if (isAdult) {{
            filteredMedications = FilterMedication(medications, {methodPosologieAdulte.MedicationFilterHour});
            rules = new List<RuleMedicationState> {{
                // TODO
            }};
        }} else {{ 
            filteredMedications = FilterMedication(medications, {methodPosologieEnfant.MedicationFilterHour});
            rules = new List<RuleMedicationState> {{
                // TODO
            }};
        }}
        return new MedicationState {{
            DrugId = DrugId.{nom},
            Opinion = ChoiceMedicationOpinion(rules.Select(r => r.Opinion).ToList()),
            LastMedicationNo = MaxDateTime(rules.Select(r => r.LastMedicationNo).ToList()),
            NextMedicationPossible = MaxDateTime(rules.Select(r => r.NextMedicationPossible).ToList()),
            NextMedicationYes = MaxDateTime(rules.Select(r => r.NextMedicationYes).ToList()),
            NextDrug = null,
            Dosages = GetDosages(filteredMedications, DrugId.{nom}),
            NumberMedication = GetNbDrug(filteredMedications, DrugId.{nom})
        }};
    }}
}}";
        }
    }
}
