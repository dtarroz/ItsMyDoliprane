using System;
using System.Collections.Generic;
using ItsMyDoliprane.Business.Enums;
using ItsMyDoliprane.Business.Medications;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Business.Tests.Mocks;
using ItsMyDoliprane.Repository.Boundary;
using ItsMyDoliprane.Repository.Models;
using Xunit;

namespace ItsMyDoliprane.Business.Tests.Medications;

public class MedicationAllDrug_Tests
{
    [Fact]
    public void GetMedicationState_Empty() {
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationAllDrug medication = new MedicationAllDrug(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(new List<Medication>(), true);

        Assert.NotNull(medicationState);
        Assert.Null(medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Empty(medicationState.Dosages);
        Assert.Equal(0, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_EmptyAndOtherDrugComposition() {
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = DateTime.Now.AddHours(-2),
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = 1,
                        Quantity = 6000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationAllDrug medication = new MedicationAllDrug(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Null(medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Empty(medicationState.Dosages);
        Assert.Equal(0, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_LessThan2Hours() {
        DateTime dateTime1 = DateTime.Now.AddHours(-1);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime1,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationAllDrug medication = new MedicationAllDrug(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Null(medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime1, medicationState.LastMedicationNo);
        Assert.Equal(dateTime1.AddHours(2), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime1.AddHours(2), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Empty(medicationState.Dosages);
        Assert.Equal(0, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_GreaterThan2Hours() {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationAllDrug medication = new MedicationAllDrug(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Null(medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Empty(medicationState.Dosages);
        Assert.Equal(0, medicationState.NumberMedication);
    }
}
