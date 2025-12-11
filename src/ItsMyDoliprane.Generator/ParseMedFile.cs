using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ItsMyDoliprane.Generator
{
    public static class ParseMedFile
    {
        public static List<FileMedicament> Parse(string contentFile) {
            MatchCollection matches = GetMatchMedicament(contentFile);
            if (matches.Count == 0)
                throw new Exception("Aucun médicament trouvé");
            return ExtractFileMedicaments(matches);
        }

        private static MatchCollection GetMatchMedicament(string contentFile) {
            const string pattern = @"(?<!# *)MEDICAMENT\s+(?<name>\w+)\s*\n(?<content>.*?)(?=\bFIN\b)";
            return Regex.Matches(contentFile ?? "", pattern, RegexOptions.Singleline);
        }

        private static List<FileMedicament> ExtractFileMedicaments(MatchCollection matches) {
            List<FileMedicament> filesMedicaments = new List<FileMedicament>();
            foreach (Match match in matches) {
                try {
                    filesMedicaments.Add(ExtractFileMedicament(match));
                }
                catch (Exception ex) {
                    throw new Exception($"Médicament '{GetDrugName(match)}' - {ex.Message}");
                }
            }
            return filesMedicaments;
        }

        private static string GetDrugName(Match match) {
            return match.Groups["name"].Value;
        }

        private static FileMedicament ExtractFileMedicament(Match match) {
            var fileMedicament = new FileMedicament { Nom = GetDrugName(match) };
            List<string> lines = ExtractLines(match);
            FilePosologie currentPosologie = null;
            FileRegle currentRegle = null;
            foreach (string line in lines) {
                if (IsPosologieLine(line)) {
                    currentPosologie = ExtractPosologie(line);
                    fileMedicament.Posologies.Add(currentPosologie);
                }
                else if (IsRegleLine(line) && currentPosologie != null) {
                    currentRegle = ExtractRegle(line);
                    currentPosologie.Regles.Add(currentRegle);
                }
                else if (IsPlageLine(line) && currentRegle != null) {
                    currentRegle.Plages.Add(ExtractPlage(line));
                }
                else {
                    throw new Exception($"Ligne '{line}' - Erreur de syntaxe");
                }
            }
            return fileMedicament;
        }

        private static List<string> ExtractLines(Match match) {
            return match.Groups["content"].Value.Split('\n').Select(l => l.Trim()).Where(l => l != "" && !l.StartsWith("#")).ToList();
        }

        private static bool IsPosologieLine(string line) {
            return line.StartsWith("POSOLOGIE");
        }

        private static FilePosologie ExtractPosologie(string line) {
            var match = Regex.Match(line, @"^POSOLOGIE\s+(Adulte|Enfant)\s+SUR\s+(20|24)h$");
            if (match.Success)
                return new FilePosologie {
                    Categorie = match.Groups[1].Value,
                    DureeHeures = int.Parse(match.Groups[2].Value)
                };
            throw new Exception($"Posologie '{line}' - Erreur de syntaxe");
        }

        private static bool IsRegleLine(string line) {
            return line.StartsWith("DOSAGE") || line.StartsWith("PRISE") || line.StartsWith("ATTENDRE APRES");
        }

        private static FileRegle ExtractRegle(string line) {
            var match = Regex.Match(line, @"^(DOSAGE|PRISE|ATTENDRE APRES)\s+(\w+)$");
            if (match.Success)
                return new FileRegle {
                    Type = match.Groups[1].Value,
                    Medicament = match.Groups[2].Value
                };
            throw new Exception($"Règle '{line}' - Erreur de syntaxe");
        }

        private static bool IsPlageLine(string line) {
            return line.Contains(":");
        }

        private static FilePlage ExtractPlage(string line) {
            Match match = Regex.Match(line, @"^([\d\+\-]+)\s*:\s*(Oui|Possible|Avertissement|Non)$");
            if (match.Success) {
                int? min = GetMinPlage(match.Groups[1].Value);
                int? max = GetMaxPlage(match.Groups[1].Value);
                if (min != null && max != null)
                    return new FilePlage {
                        Min = min.Value,
                        Max = max.Value,
                        Avis = match.Groups[2].Value
                    };
            }
            throw new Exception($"Plage '{line}' - Erreur de syntaxe");
        }

        private static int? GetMinPlage(string plage) {
            if (string.IsNullOrEmpty(plage))
                return null;
            if (plage.EndsWith("+"))
                return ConvertToInt32(plage.Substring(0, plage.Length - 1));
            string[] values = plage.Split('-');
            if (values.Length == 1)
                return ConvertToInt32(values[0]) - 1;
            return values.Length == 2 ? ConvertToInt32(values[0]) : null;
        }

        private static int? GetMaxPlage(string plage) {
            if (string.IsNullOrEmpty(plage))
                return null;
            if (plage.EndsWith("+"))
                return int.MaxValue;
            string[] values = plage.Split('-');
            if (values.Length == 1)
                return ConvertToInt32(values[0]);
            return values.Length == 2 ? ConvertToInt32(values[1]) : null;
        }

        private static int? ConvertToInt32(string text) {
            return int.TryParse(text, out int value) ? value : (int?)null;
        }
    }
}
