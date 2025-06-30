using System;
using System.Collections.Generic;
using System.Linq;

namespace ItsMyDoliprane.Generator
{
    public static class CheckFileMedicament
    {
        public static void ThrowIfNotValid(List<FileMedicament> fileMedicaments, List<string> drugId, List<string> drugCompositionId) {
            CheckMedicamentDuplication(fileMedicaments);
            foreach (FileMedicament fileMedicament in fileMedicaments) {
                try {
                    CheckMedicament(fileMedicament, drugId, drugCompositionId);
                }
                catch (Exception ex) {
                    throw new Exception($"Médicament '{fileMedicament.Nom}' - {ex.Message}");
                }
            }
        }

        private static void CheckMedicamentDuplication(List<FileMedicament> fileMedicaments) {
            string nom = fileMedicaments.Select(m => m.Nom).GroupBy(n => n).FirstOrDefault(g => g.Count() > 1)?.Key;
            if (!string.IsNullOrEmpty(nom))
                throw new Exception($"Doublon médicament '{nom}' trouvé");
        }

        private static void CheckMedicament(FileMedicament fileMedicament, List<string> drugId, List<string> drugCompositionId) {
            CheckName(fileMedicament.Nom, drugId);
            CheckPosologieEmpty(fileMedicament.Posologies);
            CheckPosologieDuplication(fileMedicament.Posologies);
            foreach (FilePosologie posologie in fileMedicament.Posologies) {
                try {
                    CheckPosologie(posologie, drugId, drugCompositionId);
                }
                catch (Exception ex) {
                    throw new Exception($"Posologie '{posologie.Categorie}' - {ex.Message}");
                }
            }
        }

        private static void CheckName(string nom, ICollection<string> drugId) {
            if (!drugId.Contains(nom))
                throw new Exception($"Le médicament '{nom}' n'a pas été trouvé");
        }

        private static void CheckPosologieEmpty(List<FilePosologie> posologies) {
            if (posologies == null || posologies.Count == 0)
                throw new Exception("Aucune posologie trouvée");
        }

        private static void CheckPosologieDuplication(List<FilePosologie> posologies) {
            string categorie = posologies.Select(p => p.Categorie).GroupBy(c => c).FirstOrDefault(g => g.Count() > 1)?.Key;
            if (!string.IsNullOrEmpty(categorie))
                throw new Exception($"Doublon posologie '{categorie}' trouvé");
        }

        private static void CheckPosologie(FilePosologie posologie, List<string> drugId, List<string> drugCompositionId) {
            CheckReglesEmpty(posologie.Regles);
            CheckReglesDuplication(posologie.Regles);
            foreach (FileRegle regle in posologie.Regles) {
                try {
                    CheckRegle(regle, drugId, drugCompositionId);
                }
                catch (Exception ex) {
                    throw new Exception($"Règle '{regle.Type} {regle.Medicament}' - {ex.Message}");
                }
            }
        }

        private static void CheckReglesEmpty(List<FileRegle> regles) {
            if (regles == null || regles.Count == 0)
                throw new Exception("Aucune règle trouvée");
        }

        private static void CheckReglesDuplication(List<FileRegle> regles) {
            string regle = regles.Select(r => $"{r.Type} {r.Medicament}").GroupBy(c => c).FirstOrDefault(g => g.Count() > 1)?.Key;
            if (!string.IsNullOrEmpty(regle))
                throw new Exception($"Doublon règle '{regle}' trouvée");
        }

        private static void CheckRegle(FileRegle regle, List<string> drugId, List<string> drugCompositionId) {
            CheckRegleMedicament(regle.Medicament, regle.Type, drugId, drugCompositionId);
            CheckPlagesCount(regle.Plages);
            CheckPlagesAvisDuplication(regle.Plages);
            if (IsAvisOrderAsc(regle)) {
                CheckPlagesAvisBeginOui(regle.Plages);
                CheckPlagesAvisOrderAsc(regle.Plages);
            }
            else {
                CheckPlagesAvisBeginNon(regle.Plages);
                CheckPlagesAvisOrderDesc(regle.Plages);
            }
            CheckPlagesBegin0(regle.Plages);
            CheckPlagesEndIntMax(regle.Plages);
            CheckPlagesMaxGreaterThanMin(regle.Plages);
            CheckPlagesOrder(regle.Plages);
        }

        private static void CheckRegleMedicament(string medicament, string type, ICollection<string> drugId,
                                                 ICollection<string> drugCompositionId) {
            if ((type == "DOSAGE" && !drugCompositionId.Contains(medicament))
                || (!drugId.Contains(medicament) && !drugCompositionId.Contains(medicament)))
                throw new Exception($"Le médicament '{medicament}' n'a pas été trouvé");
        }

        private static void CheckPlagesCount(List<FilePlage> plages) {
            if (plages == null || plages.Count < 2)
                throw new Exception("2 lignes min pour les plages");
        }

        private static void CheckPlagesAvisDuplication(List<FilePlage> plages) {
            string avis = plages.Select(p => p.Avis).GroupBy(c => c).FirstOrDefault(g => g.Count() > 1)?.Key;
            if (!string.IsNullOrEmpty(avis))
                throw new Exception($"Doublon avis '{avis}' trouvé");
        }

        private static bool IsAvisOrderAsc(FileRegle regle) {
            List<string> types = new List<string> {
                "DOSAGE",
                "PRISE"
            };
            return types.Contains(regle.Type);
        }

        private static void CheckPlagesAvisBeginOui(List<FilePlage> plages) {
            if (plages[0].Avis != "Oui")
                throw new Exception("La premiére plage doit commencer par Oui");
        }

        private static void CheckPlagesAvisBeginNon(List<FilePlage> plages) {
            if (plages[0].Avis != "Non")
                throw new Exception("La premiére plage doit commencer par Non");
        }

        private static void CheckPlagesAvisOrderAsc(List<FilePlage> plages) {
            List<string> avisOrderAsc = new List<string> {
                "Oui",
                "Avertissement",
                "Non"
            };
            int current = 0;
            for (int i = 1; i < plages.Count; i++) {
                int next = avisOrderAsc.IndexOf(plages[i].Avis);
                if (next < current)
                    throw new Exception("Erreur sur l'ordre des avis");
                current = next;
            }
        }

        private static void CheckPlagesAvisOrderDesc(List<FilePlage> plages) {
            List<string> avisOrderDesc = new List<string> {
                "Non",
                "Avertissement",
                "Possible",
                "Oui"
            };
            int current = 0;
            for (int i = 1; i < plages.Count; i++) {
                int next = avisOrderDesc.IndexOf(plages[i].Avis);
                if (next < current)
                    throw new Exception("Erreur sur l'ordre des avis");
                current = next;
            }
        }

        private static void CheckPlagesBegin0(List<FilePlage> plages) {
            if (plages[0].Min != 0)
                throw new Exception("Erreur d'interval de plages");
        }

        private static void CheckPlagesEndIntMax(List<FilePlage> plages) {
            if (plages.Last().Max != int.MaxValue)
                throw new Exception("Erreur d'interval de plages");
        }

        private static void CheckPlagesMaxGreaterThanMin(List<FilePlage> plages) {
            if (plages.Any(p => p.Min >= p.Max))
                throw new Exception("Erreur d'interval de plages");
        }

        private static void CheckPlagesOrder(List<FilePlage> plages) {
            int current = plages[0].Max;
            for (int i = 1; i < plages.Count; i++) {
                if (plages[i].Min != current)
                    throw new Exception("Erreur d'interval de plages");
                current = plages[i].Max;
            }
        }
    }
}
