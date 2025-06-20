using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ItsMyDoliprane.Generator.Tests;

public class ParseMedFile_Tests
{
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase(" \n ")]
    [TestCase("tests")]
    [TestCase("MEDICAMENT Toto1")]
    [TestCase(@"MEDICAMENT Toto1
                fin")]
    [TestCase(@"Medicament Toto1
                FIN")]
    [TestCase(@"MEDICAMENT 
                FIN")]
    [TestCase(@"MEDICAMENT Toto 1 
                FIN")]
    [TestCase(@"# MEDICAMENT Toto1
                FIN")]
    public void Parse_Empty(string content) {
        Exception? exception = Assert.Throws<Exception>(() => ParseMedFile.Parse(content));

        Assert.NotNull(exception);
        Assert.AreEqual("Aucun médicament trouvé", exception!.Message);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1)]
    [TestCase(@"MEDICAMENT Toto1
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
                FIN", 2)]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h

                    PRISE Doliprane

                      0-3 : Oui
                      3+ : Non

                FIN

                TESTS

                MEDICAMENT Toto2
                POSOLOGIE Adulte SUR 24h
                PRISE Doliprane
                0-3 : Oui
                3+ : Non

                FIN", 2)]
    public void Parse_MEDICAMENT_Count(string content, int count) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);

        Assert.AreEqual(count, fileMedicaments.Count);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1)]
    [TestCase(@"MEDICAMENT Toto1
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
                FIN", 2)]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h

                    PRISE Doliprane

                      0-3 : Oui
                      3+ : Non

                FIN

                TESTS

                MEDICAMENT Toto2        
                POSOLOGIE Adulte SUR 24h
                PRISE Doliprane
                0-3 : Oui
                3+ : Non

                FIN", 2)]
    public void Parse_MEDICAMENT_Nom(string content, int count) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);

        foreach (int index in Enumerable.Range(1, count))
            Assert.AreEqual($"Toto{index}", fileMedicaments[index - 1].Nom);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE ADULTE SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "POSOLOGIE ADULTE SUR 24h")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte sur 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "POSOLOGIE Adulte sur 24h")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Adulte SUR 24
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 2, "POSOLOGIE Adulte SUR 24")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte sur 
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "POSOLOGIE Adulte sur")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Adulte SUR h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 2, "POSOLOGIE Adulte SUR h")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Ovule SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "POSOLOGIE Ovule SUR 24h")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                  POSOLOGIE ENFANT SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "POSOLOGIE ENFANT SUR 24h")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIES Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "POSOLOGIES Adulte SUR 24h")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIES Adulte SUR 22h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "POSOLOGIES Adulte SUR 22h")]
    public void Parse_POSOLOGIE_ErrorSyntax(string content, int indexMedicament, string posologieError) {
        Exception? exception = Assert.Throws<Exception>(() => ParseMedFile.Parse(content));

        Assert.NotNull(exception);
        Assert.AreEqual($"Médicament 'Toto{indexMedicament}' - Posologie '{posologieError}' - Erreur de syntaxe", exception!.Message);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1)]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1)]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 2)]
    [TestCase(@"MEDICAMENT Toto1

                  POSOLOGIE Adulte SUR 24h

                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non

                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane

                      0-3 : Oui

                      3+ : Non
                FIN", 2)]
    [TestCase(@"MEDICAMENT Toto1

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

                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane

                      0-3 : Oui

                      3+ : Non
                FIN", 2)]
    public void Parse_POSOLOGIE_Count(string content, int count) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);

        Assert.NotNull(fileMedicaments);
        Assert.NotNull(fileMedicaments[^1].Posologies);
        Assert.AreEqual(count, fileMedicaments[^1].Posologies.Count);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", "Adulte")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", "Enfant")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", "Adulte", "Enfant")]
    [TestCase(@"MEDICAMENT Toto1

                  POSOLOGIE Adulte SUR 24h

                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non

                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane

                      0-3 : Oui

                      3+ : Non
                FIN", "Adulte", "Enfant")]
    [TestCase(@"MEDICAMENT Toto1

                  POSOLOGIE Adulte SUR 24h

                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN

                MEDICAMENT Toto2

                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane

                      0-3 : Oui

                      3+ : Non

                  POSOLOGIE Adulte SUR 24h

                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", "Enfant", "Adulte")]
    public void Parse_POSOLOGIE_Categorie(string content, params string[] categories) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);

        Assert.NotNull(fileMedicaments);
        Assert.NotNull(fileMedicaments[^1].Posologies);
        Assert.AreEqual(categories.Length, fileMedicaments[^1].Posologies.Count);
        for (int i = 0; i < categories.Length; i++)
            Assert.AreEqual(categories[i], fileMedicaments[^1].Posologies[i].Categorie);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 24)]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 20h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 20)]
    [TestCase(@"MEDICAMENT Toto1

                  POSOLOGIE Adulte SUR 20h

                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non

                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane

                      0-3 : Oui

                      3+ : Non
                FIN", 20, 24)]
    [TestCase(@"MEDICAMENT Toto1

                  POSOLOGIE Adulte SUR 24h

                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN

                MEDICAMENT Toto2

                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane

                      0-3 : Oui

                      3+ : Non

                  POSOLOGIE Adulte SUR 20h

                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 24, 20)]
    public void Parse_POSOLOGIE_DureeHeures(string content, params int[] dureeHeures) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);

        Assert.NotNull(fileMedicaments);
        Assert.NotNull(fileMedicaments[^1].Posologies);
        Assert.AreEqual(dureeHeures.Length, fileMedicaments[^1].Posologies.Count);
        for (int i = 0; i < dureeHeures.Length; i++)
            Assert.AreEqual(dureeHeures[i], fileMedicaments[^1].Posologies[i].DureeHeures);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISES Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "PRISES Doliprane")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE APRES Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "PRISE APRES Doliprane")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    DOSAGE Doli prane
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "DOSAGE Doli prane")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Adulte SUR 24h
                    ATTENDRE APRES
                      0-3 : Oui
                      3+ : Non
                FIN", 2, "ATTENDRE APRES")]
    public void Parse_Regles_ErrorSyntax(string content, int indexMedicament, string regleError) {
        Exception? exception = Assert.Throws<Exception>(() => ParseMedFile.Parse(content));

        Assert.NotNull(exception);
        Assert.AreEqual($"Médicament 'Toto{indexMedicament}' - Règle '{regleError}' - Erreur de syntaxe", exception!.Message);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1)]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 2)]
    [TestCase(@"MEDICAMENT Toto1

                  POSOLOGIE Enfant SUR 24h

                    PRISE Doliprane

                      0-3 : Oui

                      3+ : Non

                    ATTENDRE APRES Doliprane
                      0-3 : Oui

                      3+ : Non
                FIN", 2)]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Adulte SUR 24h

                    PRISE Doliprane

                      0-3 : Oui
                      3+ : Non
                FIN", 1)]
    public void Parse_Regles_Count(string content, int count) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);

        Assert.NotNull(fileMedicaments);
        Assert.NotNull(fileMedicaments[^1].Posologies);
        Assert.NotNull(fileMedicaments[^1].Posologies[^1].Regles);
        Assert.AreEqual(count, fileMedicaments[^1].Posologies[^1].Regles.Count);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", "PRISE")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", "DOSAGE")]
    [TestCase(@"MEDICAMENT Toto1

                  POSOLOGIE Enfant SUR 24h

                    PRISE Doliprane

                      0-3 : Oui

                      3+ : Non

                    ATTENDRE APRES Doliprane
                      0-3 : Oui

                      3+ : Non
                FIN", "ATTENDRE APRES")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Adulte SUR 24h

                    PRISE Doliprane

                      0-3 : Oui
                      3+ : Non
                FIN", "PRISE")]
    public void Parse_Regles_TypeRegle(string content, string typeRegle) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);

        Assert.NotNull(fileMedicaments);
        Assert.NotNull(fileMedicaments[^1].Posologies);
        Assert.NotNull(fileMedicaments[^1].Posologies[^1].Regles);
        Assert.AreEqual(typeRegle, fileMedicaments[^1].Posologies[^1].Regles[^1].Type);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", "Doliprane")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Ibuprofene
                      0-3 : Oui
                      3+ : Non
                FIN", "Ibuprofene")]
    [TestCase(@"MEDICAMENT Toto1

                  POSOLOGIE Enfant SUR 24h

                    PRISE Doliprane

                      0-3 : Oui

                      3+ : Non

                    ATTENDRE APRES Smecta
                      0-3 : Oui

                      3+ : Non
                FIN", "Smecta")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Adulte SUR 24h

                    PRISE Toto

                      0-3 : Oui
                      3+ : Non
                FIN", "Toto")]
    public void Parse_Regles_Medicament(string content, string medicament) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);

        Assert.NotNull(fileMedicaments);
        Assert.NotNull(fileMedicaments[^1].Posologies);
        Assert.NotNull(fileMedicaments[^1].Posologies[^1].Regles);
        Assert.AreEqual(medicament, fileMedicaments[^1].Posologies[^1].Regles[^1].Medicament);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0- : Oui
                      4+ : Non
                FIN", 1, "0- : Oui")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      4++ : Non
                FIN", 1, "4++ : Non")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      a+5 : Non
                FIN", 1, "a+5 : Non")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    ATTENDRE APRES Doliprane
                      0-3 : Oui
                      : Non
                FIN", 1, ": Non")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    ATTENDRE APRES Doliprane
                      0-3 : Oui
                      3+: Possible
                    DOSAGE Doliprane
                      0-3 : Avertissement
                      4+:
                FIN", 1, "4+:")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    ATTENDRE APRES Doliprane
                      -3 : Oui
                      3+: Avertissement
                    DOSAGE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "-3 : Oui")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    ATTENDRE APRES Doliprane
                      0-3 : Oui
                      +: Non
                    DOSAGE Doliprane
                      0-3 : Oui
                      3+ : Avertissement
                FIN", 1, "+: Non")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    ATTENDRE APRES Doliprane
                      0-3 : Oui
                      3+: Non
                    DOSAGE Doliprane
                      0-3 : Yes
                      3+ : Non
                FIN", 1, "0-3 : Yes")]
    public void Parse_Plages_ErrorSyntax(string content, int indexMedicament, string plageError) {
        Exception? exception = Assert.Throws<Exception>(() => ParseMedFile.Parse(content));

        Assert.NotNull(exception);
        Assert.AreEqual($"Médicament 'Toto{indexMedicament}' - Plage '{plageError}' - Erreur de syntaxe", exception!.Message);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    ATTENDRE APRES Doliprane
                      0-3 : Oui
                      3+: Non
                FIN", 2)]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      4 : Avertissement
                      4+ : Non
                    ATTENDRE APRES Doliprane
                      0-3 : Oui
                      4 : Possible
                      5-6 : Avertissement
                      6+ : Non
                FIN", 4)]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    DOSAGE Doliprane
                      0-3 : Oui
                      4 : Avertissement
                      4+ : Non
                FIN
                MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    DOSAGE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 2)]
    public void Parse_Plages_Count(string content, int count) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);

        Assert.NotNull(fileMedicaments);
        Assert.NotNull(fileMedicaments[^1].Posologies);
        Assert.NotNull(fileMedicaments[^1].Posologies[^1].Regles);
        Assert.NotNull(fileMedicaments[^1].Posologies[^1].Regles[^1].Plages);
        Assert.AreEqual(count, fileMedicaments[^1].Posologies[^1].Regles[^1].Plages.Count);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                FIN", 0)]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      4+ : Non
                FIN", 4)]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane

                      0-3 : Oui

                      4 : Avertissement

                      5+ : Non
                FIN", 5)]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Doliprane
                      0-3 : Oui
                      4 : Non
                FIN", 3)]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Doliprane
                      0-3 : Oui
                      3+ : Non
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Doliprane
                      0-3 : Oui
                      4-6 : Non
                FIN", 4)]
    public void Parse_Plages_Min(string content, int min) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);
        
        Assert.NotNull(fileMedicaments);
        Assert.NotNull(fileMedicaments[^1].Posologies);
        Assert.NotNull(fileMedicaments[^1].Posologies[^1].Regles);
        Assert.NotNull(fileMedicaments[^1].Posologies[^1].Regles[^1].Plages);
        Assert.AreEqual(min, fileMedicaments[^1].Posologies[^1].Regles[^1].Plages[^1].Min);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                FIN", 3)]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", int.MaxValue)]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane

                      0-3 : Oui

                      4 : Avertissement

                      5+ : Non
                FIN", int.MaxValue)]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Doliprane
                      0-3 : Oui
                      4 : Non
                FIN", 4)]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Doliprane
                      0-3 : Oui
                      3+ : Non
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                    DOSAGE Doliprane
                      0-3 : Oui
                      3-6 : Non
                FIN", 6)]
    public void Parse_Plages_Max(string content, int max) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);

        Assert.NotNull(fileMedicaments);
        Assert.NotNull(fileMedicaments[^1].Posologies);
        Assert.NotNull(fileMedicaments[^1].Posologies[^1].Regles);
        Assert.NotNull(fileMedicaments[^1].Posologies[^1].Regles[^1].Plages);
        Assert.AreEqual(max, fileMedicaments[^1].Posologies[^1].Regles[^1].Plages[^1].Max);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Non
                      3+ : Oui
                FIN", "Oui")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", "Non")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    ATTENDRE APRES Doliprane

                      0-3 : Oui

                      3+ : Possible
                FIN", "Possible")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Enfant SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      4+ : Non
                    DOSAGE Doliprane
                      0-3 : Oui
                      3+ : Avertissement
                FIN", "Avertissement")]
    public void Parse_Plages_Avis(string content, string avis) {
        List<FileMedicament> fileMedicaments = ParseMedFile.Parse(content);

        Assert.NotNull(fileMedicaments);
        Assert.NotNull(fileMedicaments[^1].Posologies);
        Assert.NotNull(fileMedicaments[^1].Posologies[^1].Regles);
        Assert.NotNull(fileMedicaments[^1].Posologies[^1].Regles[^1].Plages);
        Assert.AreEqual(avis, fileMedicaments[^1].Posologies[^1].Regles[^1].Plages[^1].Avis);
    }

    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    Attendre APRES Doliprane
                      0-3 : Oui
                      3+: Non
                    DOSAGE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "Attendre APRES Doliprane")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    ATTENDRE apres Doliprane
                      0-3 : Oui
                      3+: Non
                    DOSAGE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "ATTENDRE apres Doliprane")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    Prise Doliprane
                      0-3 : Oui
                      3+: Non
                    DOSAGE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "Prise Doliprane")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+: Non
                    Dosage Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "Dosage Doliprane")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+: Non
                    DOSAGE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN
                MEDICAMENT Toto2
                  POSOLOGIE Adulte SUR 24h
                    PRISE Doliprane
                      0-3 : Oui
                      3+ Non
                    DOSAGE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 2, "3+ Non")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    ATTENDRE APRES Doliprane
                      0-3 : Oui
                      3+: Non
                    AUTRE Doliprane
                      0-3 : Oui
                      3+ : Non
                FIN", 1, "AUTRE Doliprane")]
    [TestCase(@"MEDICAMENT Toto1
                  Posologie Adulte SUR 24h
                    ATTENDRE APRES Doliprane
                      0-3 : Oui
                      3+: Non
                FIN", 1, "Posologie Adulte SUR 24h")]
    [TestCase(@"MEDICAMENT Toto1
                  ATTENDRE APRES Doliprane
                    0-3 : Oui
                    3+: Non
                FIN", 1, "ATTENDRE APRES Doliprane")]
    [TestCase(@"MEDICAMENT Toto1
                  POSOLOGIE Adulte SUR 24h
                    0-3 : Oui
                    3+ : Non
                FIN", 1, "0-3 : Oui")]
    public void Parse_Line_ErrorSyntax(string content, int indexMedicament, string lineError) {
        Exception? exception = Assert.Throws<Exception>(() => ParseMedFile.Parse(content));

        Assert.NotNull(exception);
        Assert.AreEqual($"Médicament 'Toto{indexMedicament}' - Ligne '{lineError}' - Erreur de syntaxe", exception!.Message);
    }
}
