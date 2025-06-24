using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace ItsMyDoliprane.Generator
{
    [Generator]
    public class MedFileGenerator : ISourceGenerator
    {
        private GeneratorExecutionContext _context;
        private List<string> _drugId;
        private List<string> _drugCompositionId;

        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context) {
            _context = context;
            SearchEnumData();
            foreach (AdditionalText file in context.AdditionalFiles.Where(file => Path.GetExtension(file.Path) == ".med"))
                FileMedToClass(file);
        }

        private void SearchEnumData() {
            _drugId = ExtractNamesFromEnum("ItsMyDoliprane.Business.Enums.DrugId");
            _drugCompositionId = ExtractNamesFromEnum("ItsMyDoliprane.Business.Enums.DrugCompositionId");
        }

        private List<string> ExtractNamesFromEnum(string enumName) {
            var enumType = _context.Compilation.GetTypeByMetadataName(enumName);
            return enumType?.GetMembers().OfType<IFieldSymbol>().Where(f => f.ConstantValue != null).Select(f => f.Name).ToList();
        }

        private void FileMedToClass(AdditionalText fileSource) {
            List<FileMedicament> medicaments = Parse(fileSource);
            if (medicaments == null || !Check(medicaments))
                return;
            Dictionary<string, string> files = ConvertToFile(medicaments);
            foreach (var file in files)
                _context.AddSource(file.Key, SourceText.From(file.Value, Encoding.UTF8));
        }

        private List<FileMedicament> Parse(AdditionalText file) {
            try {
                SourceText sourceText = file.GetText(_context.CancellationToken);
                string content = sourceText?.ToString();
                return ParseMedFile.Parse(content);
            }
            catch (Exception ex) {
                ErrorCompilation("001", $"Fichier '{Path.GetFileName(file.Path)}' - {ex.Message}");
                return null;
            }
        }

        private bool Check(List<FileMedicament> medicaments) {
            try {
                CheckFileMedicament.ThrowIfNotValid(medicaments, _drugId, _drugCompositionId);
                return true;
            }
            catch (Exception ex) {
                ErrorCompilation("002", ex.Message);
                return false;
            }
        }

        private Dictionary<string, string> ConvertToFile(List<FileMedicament> medicaments) {
            return ClassMedicament.ConvertToClassFile(medicaments, _drugCompositionId);
        }

        private void ErrorCompilation(string code, string error) {
            _context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor($"MED{code}", "MED", error, "MED", DiagnosticSeverity.Error, true),
                                                        Location.None));
        }
    }
}
