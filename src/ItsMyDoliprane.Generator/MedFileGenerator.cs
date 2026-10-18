using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace ItsMyDoliprane.Generator
{
    [Generator]
    public class MedFileGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var medFiles = context.AdditionalTextsProvider
                .Where(file => Path.GetExtension(file.Path) == ".med")
                .Select((file, ct) => new MedFileContent(file.Path, file.GetText(ct)?.ToString()))
                .Collect();

            var compilationAndFiles = context.CompilationProvider.Combine(medFiles);
            context.RegisterSourceOutput(compilationAndFiles, (spc, source) =>
            {
                Execute(spc, source.Left, source.Right);
            });
        }

        private static void Execute(SourceProductionContext context, Compilation compilation, ImmutableArray<MedFileContent> medFiles)
        {
            List<string> drugId = ExtractNamesFromEnum(compilation, "ItsMyDoliprane.Business.Enums.DrugId");
            List<string> drugCompositionId = ExtractNamesFromEnum(compilation, "ItsMyDoliprane.Business.Enums.DrugCompositionId");

            foreach (var file in medFiles)
            {
                FileMedToClass(context, file, drugId, drugCompositionId);
            }
        }

        private static List<string> ExtractNamesFromEnum(Compilation compilation, string enumName)
        {
            var enumType = compilation.GetTypeByMetadataName(enumName);
            return enumType?.GetMembers().OfType<IFieldSymbol>().Where(f => f.ConstantValue != null).Select(f => f.Name).ToList()
                   ?? new List<string>();
        }

        private static void FileMedToClass(SourceProductionContext context, MedFileContent fileSource, List<string> drugId,
                                           List<string> drugCompositionId)
        {
            List<FileMedicament> medicaments = Parse(context, fileSource);
            if (medicaments == null || !Check(context, medicaments, drugId, drugCompositionId))
                return;

            Dictionary<string, string> files = ConvertToFile(medicaments, drugCompositionId);
            foreach (var file in files)
                context.AddSource(file.Key, SourceText.From(file.Value, Encoding.UTF8));
        }

        private static List<FileMedicament> Parse(SourceProductionContext context, MedFileContent file)
        {
            try
            {
                return ParseMedFile.Parse(file.Content ?? string.Empty);
            }
            catch (Exception ex)
            {
                ErrorCompilation(context, "001", $"Fichier '{Path.GetFileName(file.Path)}' - {ex.Message}");
                return null;
            }
        }

        private static bool Check(SourceProductionContext context, List<FileMedicament> medicaments, List<string> drugId,
                                  List<string> drugCompositionId)
        {
            try
            {
                CheckFileMedicament.ThrowIfNotValid(medicaments, drugId, drugCompositionId);
                return true;
            }
            catch (Exception ex)
            {
                ErrorCompilation(context, "002", ex.Message);
                return false;
            }
        }

        private static Dictionary<string, string> ConvertToFile(List<FileMedicament> medicaments, List<string> drugCompositionId)
        {
            return ClassMedicament.ConvertToClassFile(medicaments, drugCompositionId);
        }

        private static void ErrorCompilation(SourceProductionContext context, string code, string error)
        {
            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor($"MED{code}", "MED", error, "MED", DiagnosticSeverity.Error, true),
                                                       Location.None));
        }

        private sealed class MedFileContent
        {
            public MedFileContent(string path, string content)
            {
                Path = path;
                Content = content;
            }

            public string Path { get; }
            public string Content { get; }
        }
    }
}
