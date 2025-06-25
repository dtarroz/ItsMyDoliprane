using System;
using System.Collections.Generic;
using System.Linq;

namespace ItsMyDoliprane.Generator
{
    public static class ClassMedicament
    {
        public static Dictionary<string, string> ConvertToClassFile(List<FileMedicament> fileMedicaments, List<string> drugCompositionId) {
            return fileMedicaments.ToDictionary(fileMedicament => $"{GetClassName(fileMedicament.Nom)}.g.cs",
                                                fileMedicament => ConvertTo(fileMedicament, drugCompositionId));
        }

        private static string GetClassName(string nom) {
            return $"Medication{nom}";
        }

        private static string ConvertTo(FileMedicament fileMedicament, List<string> drugCompositionId) {
            MethodPosologie methodPosologieAdulte = GetMethodPosologie(GetPosologie(fileMedicament, "Adulte"), drugCompositionId);
            MethodPosologie methodPosologieEnfant = GetMethodPosologie(GetPosologie(fileMedicament, "Enfant"), drugCompositionId);
            return Generate(fileMedicament.Nom, methodPosologieAdulte, methodPosologieEnfant);
        }

        private static FilePosologie GetPosologie(FileMedicament fileMedicament, string categorie) {
            return fileMedicament.Posologies.FirstOrDefault(p => p.Categorie == categorie);
        }

        private static MethodPosologie GetMethodPosologie(FilePosologie posologie, List<string> drugCompositionId) {
            if (posologie == null)
                return GetMethodPosologieEmpty();
            return new MethodPosologie {
                MedicationFilterHour = posologie.DureeHeures,
                Methods = posologie.Regles.Select(r => GetMethodFromRegle(r, posologie, drugCompositionId)).ToList()
            };
        }

        private static MethodPosologie GetMethodPosologieEmpty() {
            return new MethodPosologie {
                MedicationFilterHour = 24,
                Methods = new List<MethodRegle> { GetMethodNoPosologie() }
            };
        }

        private static MethodRegle GetMethodNoPosologie() {
            return new MethodRegle { Name = "NoPosologie" };
        }

        private static MethodRegle GetMethodFromRegle(FileRegle regle, FilePosologie posologie, List<string> drugCompositionId) {
            switch (regle.Type) {
                case "PRISE":          return GetMethodPrise(regle, posologie, drugCompositionId);
                case "DOSAGE":         return GetMethodDosage(regle, posologie);
                case "ATTENDRE APRES": return GetMethodAttendreApres(regle, posologie, drugCompositionId);
                default:               throw new Exception($"Regle '{regle.Type}' inconnue");
            }
        }

        private static MethodRegle GetMethodPrise(FileRegle regle, FilePosologie posologie, List<string> drugCompositionId) {
            bool isDrugComposition = IsDrug(regle.Medicament, drugCompositionId);
            string count = isDrugComposition
                ? $"int count = GetNbDrugComposition(medications, DrugCompositionId.{regle.Medicament});"
                : $"int count = GetNbDrug(medications, DrugId.{regle.Medicament});";
            string last = isDrugComposition
                ? $"Medication last = GetLastMedication(medications, DrugCompositionId.{regle.Medicament});"
                : $"Medication last = GetLastMedication(medications, DrugId.{regle.Medicament});";
            FilePlage plageOui = regle.Plages[0];
            FilePlage plageAvertissement = regle.Plages.FirstOrDefault(p => p.Avis == "Avertissement");
            string lastYes = isDrugComposition
                ? $"Medication lastYes = medications.Where(m => m.Dosages.Any(d => d.DrugCompositionId == (int)DrugCompositionId.{regle.Medicament})).Skip({plageOui.Max - 1}).FirstOrDefault();"
                : $"Medication lastYes = medications.Where(m => m.DrugId == (int)DrugId.{regle.Medicament}).Skip({plageOui.Max - 1}).FirstOrDefault();";
            string switchOui = $"<= {plageOui.Max} => new RuleMedicationState {{ Opinion = MedicationOpinion.Yes }},";
            string switchAvertissement = plageAvertissement != null
                ? $@"<= {plageAvertissement.Max} => new RuleMedicationState {{
                Opinion = MedicationOpinion.Warning,
                LastMedicationNo = last.DateTime,
                NextMedicationPossible = lastYes.DateTime.AddHours({posologie.DureeHeures}),
                NextMedicationYes = lastYes.DateTime.AddHours({posologie.DureeHeures})
            }},"
                : "";
            string switchNon = $@"_ => new RuleMedicationState {{
                Opinion = MedicationOpinion.No,
                LastMedicationNo = last.DateTime,
                NextMedicationPossible = lastYes.DateTime.AddHours({posologie.DureeHeures}),
                NextMedicationYes = lastYes.DateTime.AddHours({posologie.DureeHeures})
            }}";
            return new MethodRegle {
                Name = $"GetRulePrise{regle.Medicament}{posologie.Categorie}",
                Content = $@"{count}
        {last}
        {lastYes}
        return (count + 1) switch {{
            {switchOui}
            {switchAvertissement}
            {switchNon}
        }};"
            };
        }

        private static MethodRegle GetMethodDosage(FileRegle regle, FilePosologie posologie) {
            throw new NotImplementedException();
        }

        private static MethodRegle GetMethodAttendreApres(FileRegle regle, FilePosologie posologie, List<string> drugCompositionId) {
            bool isDrugComposition = IsDrug(regle.Medicament, drugCompositionId);
            string last = isDrugComposition
                ? $"Medication last = GetLastMedication(medications, DrugCompositionId.{regle.Medicament});"
                : $"Medication last = GetLastMedication(medications, DrugId.{regle.Medicament});";
            FilePlage plageNon = regle.Plages[0];
            FilePlage plageAvertissement = regle.Plages.FirstOrDefault(p => p.Avis == "Avertissement");
            FilePlage plagePossible = regle.Plages.FirstOrDefault(p => p.Avis == "Possible");
            FilePlage plageOui = regle.Plages.Last();
            string switchNon = $@"<= {plageNon.Max} => new RuleMedicationState {{
                Opinion = MedicationOpinion.No,
                LastMedicationNo = last.DateTime,
                NextMedicationPossible = last.DateTime.AddHours({plagePossible?.Min ?? plageOui.Min}),
                NextMedicationYes = last.DateTime.AddHours({plageOui.Min})
            }},";
            string switchAvertissement = plageAvertissement != null
                ? $@"<= {plageAvertissement.Max} => new RuleMedicationState {{
                Opinion = MedicationOpinion.Warning,
                LastMedicationNo = last.DateTime,
                NextMedicationPossible = last.DateTime.AddHours({plagePossible?.Min ?? plageOui.Min}),
                NextMedicationYes = last.DateTime.AddHours({plageOui.Min})
            }},"
                : "";
            string switchPossible = plagePossible != null
                ? $@"<= {plagePossible.Max} => new RuleMedicationState {{
                Opinion = MedicationOpinion.Possible,
                LastMedicationNo = last.DateTime,
                NextMedicationYes = last.DateTime.AddHours({plageOui.Min})
            }},"
                : "";
            string switchOui = $"_ => new RuleMedicationState {{ Opinion = MedicationOpinion.Yes }},";
            return new MethodRegle {
                Name = $"GetRuleAttendreApres{regle.Medicament}{posologie.Categorie}",
                Content = $@"{last}
        float? durationSinceLastMedication = GetDurationBetweenDateTime(last?.DateTime, DateTime.Now);
        return durationSinceLastMedication switch {{
            {switchNon}
            {switchAvertissement}
            {switchPossible}
            {switchOui}
        }};"
            };
        }

        private static bool IsDrug(string drug, ICollection<string> drugId) {
            return drugId.Contains(drug);
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
                {GetCallMethods(methodPosologieAdulte.Methods)}
            }};
        }} else {{ 
            filteredMedications = FilterMedication(medications, {methodPosologieEnfant.MedicationFilterHour});
            rules = new List<RuleMedicationState> {{
                {GetCallMethods(methodPosologieEnfant.Methods)}
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

    {GetMethods(methodPosologieAdulte.Methods)}

    {GetMethods(methodPosologieEnfant.Methods)}

    private RuleMedicationState NoPosologie(List<Medication> medications) {{
        return new RuleMedicationState {{
            Opinion = MedicationOpinion.No,
            LastMedicationNo = DateTime.Now,
            NextMedicationPossible = DateTime.Now.AddHours(6),
            NextMedicationYes = DateTime.Now.AddHours(6)
         }};
    }}

}}";
        }

        private static string GetCallMethods(IEnumerable<MethodRegle> methods) {
            return string.Join(",\r\n                ", methods.Select(m => $"{m.Name}(filteredMedications)"));
        }

        private static string GetMethods(IEnumerable<MethodRegle> methods) {
            return string.Join("\r\n\r\n    ", methods.Where(m => m.Content != null).Select(GetMethod));
        }

        private static string GetMethod(MethodRegle method) {
            return $@"private static RuleMedicationState {method.Name}(List<Medication> medications) {{
        {method.Content}
    }}";
        }
    }
}
