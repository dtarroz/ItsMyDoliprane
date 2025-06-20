using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ItsMyDoliprane.Generator.Tests;

public class CheckFileMedicament_Tests
{
    [TestCase(@"MEDICAMENT Toto1_Compo
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", "Toto1_Compo")]
    [TestCase(@"MEDICAMENT Toto3
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", "Toto3")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Toto1_Compo
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto4
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", "Toto4")]
    public void ThrowIfNotValid_Nom_Drug(string content, string nom) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);
        Exception? exception = Assert.Throws<Exception>(() => CheckFileMedicament_ThrowIfNotValid(fileMedicaments));

        Assert.NotNull(exception);
        Assert.AreEqual($"Médicament '{nom}' - Le médicament '{nom}' n'a pas été trouvé", exception!.Message);
    }

    private static void CheckFileMedicament_ThrowIfNotValid(List<FileMedicament> fileMedicaments) {
        List<string> drugId = new() {
            "Toto1",
            "Toto2"
        };
        List<string> drugCompositionId = new() {
            "Toto1_Compo",
            "Toto2_Compo"
        };
        CheckFileMedicament.ThrowIfNotValid(fileMedicaments, drugId, drugCompositionId);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", "Toto1")]
    [TestCase(@"MEDICAMENT Toto2
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", "Toto2")]
    public void ThrowIfNotValid_Nom_Duplication(string content, string nom) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);
        Exception? exception = Assert.Throws<Exception>(() => CheckFileMedicament_ThrowIfNotValid(fileMedicaments));

        Assert.NotNull(exception);
        Assert.AreEqual($"Doublon médicament '{nom}' trouvé", exception!.Message);
    }

    [TestCase(@"MEDICAMENT Toto1
                FIN", "Toto1")]
    public void ThrowIfNotValid_Posologie_Empty(string content, string nom) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);
        Exception? exception = Assert.Throws<Exception>(() => CheckFileMedicament_ThrowIfNotValid(fileMedicaments));

        Assert.NotNull(exception);
        Assert.AreEqual($"Médicament '{nom}' - Aucune posologie trouvée", exception!.Message);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Toto1
                      0-3 : Oui
                      3+ : Non
                  POSOLOGIE Adulte SUR 24h
                    PRISE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "Adulte")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                  POSOLOGIE Enfant SUR 24h
                    PRISE Toto1_Compo
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "Enfant")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Toto1
                      0-3 : Oui
                      3+ : Non
                  POSOLOGIE Enfant SUR 24h
                    PRISE Toto1_Compo
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Enfant SUR 24h
                    PRISE Toto1_Compo
                      0-3 : Oui
                      3+ : Non
                  POSOLOGIE Enfant SUR 24h
                    PRISE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                FIN", 2, "Enfant")]
    public void ThrowIfNotValid_Posologie_Categorie_Duplication(string content, int indexMedicament, string categorie) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);
        Exception? exception = Assert.Throws<Exception>(() => CheckFileMedicament_ThrowIfNotValid(fileMedicaments));

        Assert.NotNull(exception);
        Assert.AreEqual($"Médicament 'Toto{indexMedicament}' - Doublon posologie '{categorie}' trouvé", exception!.Message);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                  POSOLOGIE Enfant SUR 24h
                    PRISE Toto2
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "Adulte")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Toto1_Compo
                      0-3 : Oui
                      3+ : Non
                  POSOLOGIE Enfant SUR 24h
                    PRISE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Adulte SUR 24h
                    PRISE Toto1
                      0-3 : Oui
                      3+ : Non
                  POSOLOGIE Enfant SUR 24h
                FIN", 2, "Enfant")]
    public void ThrowIfNotValid_Regle_Empty(string content, int indexMedicament, string categorie) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);
        Exception? exception = Assert.Throws<Exception>(() => CheckFileMedicament_ThrowIfNotValid(fileMedicaments));

        Assert.NotNull(exception);
        Assert.AreEqual($"Médicament 'Toto{indexMedicament}' - Posologie '{categorie}' - Aucune règle trouvée", exception!.Message);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Toto1
                      0-3 : Oui
                      3+ : Non
                    PRISE Toto1
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "Adulte", "PRISE Toto1")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Toto2
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Toto2
                      0-3 : Oui
                      3+ : Non
                    PRISE Toto2
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "Adulte", "PRISE Toto2")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Toto1_Compo
                      0-3 : Oui
                      3+ : Non
                    PRISE Toto1
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Enfant SUR 24h
                    DOSAGE Toto1_Compo
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Toto1_Compo
                      0-3 : Oui
                      3+ : Non
                FIN", 2, "Enfant", "DOSAGE Toto1_Compo")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 20h
                    ATTENDRE APRES Toto2_Compo
                      0-3 : Non
                      3+ : Oui
                    ATTENDRE APRES Toto2_Compo
                      0-3 : Non
                      3+ : Oui
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Enfant SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    PRISE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "Enfant", "ATTENDRE APRES Toto2_Compo")]
    public void ThrowIfNotValid_Regle_Duplication(string content, int indexMedicament, string categorie, string typeRegle) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);
        Exception? exception = Assert.Throws<Exception>(() => CheckFileMedicament_ThrowIfNotValid(fileMedicaments));

        Assert.NotNull(exception);
        Assert.AreEqual($"Médicament 'Toto{indexMedicament}' - Posologie '{categorie}' - Doublon règle '{typeRegle}' trouvée",
                        exception!.Message);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 20h
                    ATTENDRE APRES Toto1
                      0-3 : Non
                      3+ : Oui
                    DOSAGE Toto3
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "Enfant", "DOSAGE Toto3", "Toto3")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Toto3
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Toto1
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "Adulte", "PRISE Toto3", "Toto3")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    ATTENDRE APRES Toto3
                      0-3 : Non
                      3+ : Oui
                FIN", 1, "Enfant", "ATTENDRE APRES Toto3", "Toto3")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    PRISE Toto1
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Enfant SUR 24h
                    ATTENDRE APRES Toto2_Compo
                      0-3 : Non
                      3+ : Oui
                    PRISE Toto3
                      0-3 : Oui
                      3+ : Non
                FIN", 2, "Enfant", "PRISE Toto3", "Toto3")]
    public void ThrowIfNotValid_Regle_DrugOrDrugComposition(string content, int indexMedicament, string categorie, string regle, string medicament) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);
        Exception? exception = Assert.Throws<Exception>(() => CheckFileMedicament_ThrowIfNotValid(fileMedicaments));

        Assert.NotNull(exception);
        Assert.AreEqual($"Médicament 'Toto{indexMedicament}' - Posologie '{categorie}' - Règle '{regle}' - Le médicament '{medicament}' n'a pas été trouvé",
                        exception!.Message);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                    PRISE Toto1
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "Enfant", "DOSAGE Toto2_Compo")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 20h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    PRISE Toto1
                FIN", 1, "Adulte", "PRISE Toto1")]
    public void ThrowIfNotValid_Plage_Count(string content, int indexMedicament, string categorie, string regle) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);
        Exception? exception = Assert.Throws<Exception>(() => CheckFileMedicament_ThrowIfNotValid(fileMedicaments));

        Assert.NotNull(exception);
        Assert.AreEqual($"Médicament 'Toto{indexMedicament}' - Posologie '{categorie}' - Règle '{regle}' - 2 lignes min pour les plages",
                        exception!.Message);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 20h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      4 : Non
                    PRISE Toto1
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "Adulte", "DOSAGE Toto2_Compo")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 20h
                    DOSAGE Toto2_Compo
                      1-3 : Oui
                      3+ : Non
                    PRISE Toto1
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "Adulte", "DOSAGE Toto2_Compo")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 20h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    PRISE Toto1
                      0-2 : Oui
                      3+ : Non
                FIN", 1, "Adulte", "PRISE Toto1")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 20h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    ATTENDRE APRES Toto2
                      0-2 : Non
                      1+ : Oui
                FIN", 1, "Enfant", "ATTENDRE APRES Toto2")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 20h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    PRISE Toto1
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Adulte SUR 20h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    PRISE Toto1
                      0-3 : Oui
                      3+ : Non
                  POSOLOGIE Enfant SUR 24h
                    ATTENDRE APRES Toto2_Compo
                      0-4 : Non
                      4-7 : Possible
                      8+ : Oui 
                    PRISE Toto1
                      0-2 : Oui
                      4+ : Non
                FIN", 2, "Enfant", "ATTENDRE APRES Toto2_Compo")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 20h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      6-3 : Avertissement
                      6+ : Non
                    PRISE Toto1
                      0-10 : Oui
                      10+ : Non
                FIN", 1, "Adulte", "DOSAGE Toto2_Compo")]
    public void ThrowIfNotValid_Plage_MinMax(string content, int indexMedicament, string categorie, string regle) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);
        Exception? exception = Assert.Throws<Exception>(() => CheckFileMedicament_ThrowIfNotValid(fileMedicaments));

        Assert.NotNull(exception);
        Assert.AreEqual($"Médicament 'Toto{indexMedicament}' - Posologie '{categorie}' - Règle '{regle}' - Erreur d'interval de plages",
                        exception!.Message);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Avertissement
                      3+ : Non
                    PRISE Toto1
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "Adulte", "DOSAGE Toto2_Compo")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    PRISE Toto1
                      0-3 : Avertissement
                      3+ : Non
                FIN", 1, "Enfant", "PRISE Toto1")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    PRISE Toto1
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Adulte SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    PRISE Toto1
                      0-3 : Avertissement
                      3+ : Non
                FIN", 2, "Adulte", "PRISE Toto1")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    PRISE Toto1
                      0-3 : Non
                      3+ : Avertissement
                FIN", 1, "Adulte", "PRISE Toto1")]
    public void ThrowIfNotValid_Plage_BeginOui(string content, int indexMedicament, string categorie, string regle) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);
        Exception? exception = Assert.Throws<Exception>(() => CheckFileMedicament_ThrowIfNotValid(fileMedicaments));

        Assert.NotNull(exception);
        Assert.AreEqual($"Médicament 'Toto{indexMedicament}' - Posologie '{categorie}' - Règle '{regle}' - La premiére plage doit commencer par Oui",
                        exception!.Message);
    }


    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    ATTENDRE APRES Toto1
                      0-3 : Oui
                      3+ : Avertissement
                FIN", 1, "Adulte", "ATTENDRE APRES Toto1")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    ATTENDRE APRES Toto1
                      0-3 : Avertissement
                      3+ : Oui
                FIN", 1, "Adulte", "ATTENDRE APRES Toto1")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    ATTENDRE APRES Toto1
                      0-3 : Possible
                      3+ : Oui
                FIN", 1, "Adulte", "ATTENDRE APRES Toto1")]
    public void ThrowIfNotValid_Plage_BeginNon(string content, int indexMedicament, string categorie, string regle) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);
        Exception? exception = Assert.Throws<Exception>(() => CheckFileMedicament_ThrowIfNotValid(fileMedicaments));

        Assert.NotNull(exception);
        Assert.AreEqual($"Médicament 'Toto{indexMedicament}' - Posologie '{categorie}' - Règle '{regle}' - La premiére plage doit commencer par Non",
                        exception!.Message);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    PRISE Toto1
                      0-3 : Oui
                      3-10 : Oui
                      10+ : Non
                FIN", 1, "Adulte", "PRISE Toto1", "Oui")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    PRISE Toto1
                      0-3 : Oui
                      3-10 : Avertissement
                      10-12 : Oui
                      12+ : Non
                FIN", 1, "Adulte", "PRISE Toto1", "Oui")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    PRISE Toto1
                      0-3 : Oui
                      3-10 : Avertissement
                      10-12 : Avertissement
                      12+ : Non
                FIN", 1, "Adulte", "PRISE Toto1", "Avertissement")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    ATTENDRE APRES Toto1
                      0-3 : Oui
                      3-10 : Avertissement
                      10-12 : Avertissement
                      12+ : Non
                FIN", 1, "Adulte", "ATTENDRE APRES Toto1", "Avertissement")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    ATTENDRE APRES Toto1
                      0-3 : Oui
                      3-10 : Possible
                      10-12 : Possible
                      12+ : Non
                FIN", 1, "Adulte", "ATTENDRE APRES Toto1", "Possible")]
    public void ThrowIfNotValid_Plage_Avis_Duplication(string content, int indexMedicament, string categorie, string regle, string avis) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);
        Exception? exception = Assert.Throws<Exception>(() => CheckFileMedicament_ThrowIfNotValid(fileMedicaments));

        Assert.NotNull(exception);
        Assert.AreEqual($"Médicament 'Toto{indexMedicament}' - Posologie '{categorie}' - Règle '{regle}' - Doublon avis '{avis}' trouvé",
                        exception!.Message);
    }
    
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    PRISE Toto1
                      0-3 : Oui
                      3-10 : Non
                      10+ : Avertissement
                FIN", 1, "Adulte", "PRISE Toto1")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    PRISE Toto1
                      0-3 : Oui
                      3-10 : Non
                      10+ : Avertissement
                FIN", 1, "Adulte", "PRISE Toto1")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Toto1
                      0-3 : Oui
                      3-10 : Non
                      10+ : Avertissement
                FIN", 1, "Adulte", "DOSAGE Toto1")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Toto1
                      0-3 : Oui
                      3-10 : Non
                      10+ : Avertissement
                FIN", 1, "Adulte", "DOSAGE Toto1")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    ATTENDRE APRES Toto1
                      0-3 : Non
                      3-10 : Possible
                      10-12 : Avertissement
                      123+ : Oui
                FIN", 1, "Adulte", "ATTENDRE APRES Toto1")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    ATTENDRE APRES Toto1
                      0-3 : Non
                      3-10 : Oui
                      10+ : Possible
                FIN", 1, "Enfant", "ATTENDRE APRES Toto1")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    ATTENDRE APRES Toto1
                      0-3 : Non
                      3-10 : Oui
                      10+ : Avertissement
                FIN", 1, "Enfant", "ATTENDRE APRES Toto1")]
    public void ThrowIfNotValid_Plage_OrderAvis(string content, int indexMedicament, string categorie, string regle) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);
        Exception? exception = Assert.Throws<Exception>(() => CheckFileMedicament_ThrowIfNotValid(fileMedicaments));

        Assert.NotNull(exception);
        Assert.AreEqual($"Médicament 'Toto{indexMedicament}' - Posologie '{categorie}' - Règle '{regle}' - Erreur sur l'ordre des avis",
                        exception!.Message);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Toto1_Compo
                      0-3 : Oui
                      3+ : Avertissement
                    PRISE Toto1
                      0-3 : Oui
                      3-10 : Avertissement
                      10+ : Non
                    ATTENDRE APRES Toto2
                      0-3 : Non
                      3-10 : Avertissement
                      10+ : Possible
                    ATTENDRE APRES Toto1
                      0-3  : Non
                      3-10 : Avertissement
                      11   : Possible
                      11+  : Oui
            POSOLOGIE Enfant SUR 20h
                    DOSAGE Toto2_Compo
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Toto1_Compo
                      0-3 : Oui
                      3+ : Avertissement
                    PRISE Toto1
                      0-3 : Oui
                      3-10 : Avertissement
                      10+ : Non
                    ATTENDRE APRES Toto2
                      0-3 : Non
                      3-10 : Avertissement
                      10+ : Oui
                    ATTENDRE APRES Toto1
                      0-3  : Non
                      3-10 : Avertissement
                      11   : Possible
                      11+  : Oui
                FIN")]
    public void ThrowIfNotValid_Ok(string content) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);
        
        Assert.DoesNotThrow(() => CheckFileMedicament_ThrowIfNotValid(fileMedicaments));
    }
}
