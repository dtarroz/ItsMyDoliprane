using System;
using System.Collections.Generic;
using ItsMyDoliprane.Business.Enums;
using ItsMyDoliprane.Business.Medications;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Repository.Models;
using Xunit;

namespace ItsMyDoliprane.Business.Tests.Medications;

public class MedicationAntibiotique_Tests
{
    [Fact]
    public void GetMedicationState_Empty() {
        MedicationAntibiotique medication = new MedicationAntibiotique(new MedicationAllDrug());
        MedicationState medicationState = medication.GetMedicationState(new List<Medication>());

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Antibiotique, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(0, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_EmptyAndOtherDrugComposition() {
        DateTime dateTime2 = DateTime.Now.AddHours(-2);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime2,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = 2,
                        Quantity = 6000
                    }
                }
            }
        };
        MedicationAntibiotique medication = new MedicationAntibiotique(new MedicationAllDrug());
        MedicationState medicationState = medication.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Antibiotique, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(0, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_1_LessThan4Hours() {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage>()
            }
        };
        MedicationAntibiotique medication = new MedicationAntibiotique(new MedicationAllDrug());
        MedicationState medicationState = medication.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Antibiotique, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_1_GreaterThan4Hours() {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage>()
            }
        };
        MedicationAntibiotique medicationAntibiotique = new MedicationAntibiotique(new MedicationAllDrug());
        MedicationState medicationState = medicationAntibiotique.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Antibiotique, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_2_LessThan4Hours() {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage>()
            }
        };
        MedicationAntibiotique medicationAntibiotique = new MedicationAntibiotique(new MedicationAllDrug());
        MedicationState medicationState = medicationAntibiotique.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Antibiotique, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(2, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_2_GreaterThan4Hours() {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage>()
            }
        };
        MedicationAntibiotique medicationAntibiotique = new MedicationAntibiotique(new MedicationAllDrug());
        MedicationState medicationState = medicationAntibiotique.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Antibiotique, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(2, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_3_LessThan4Hours() {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage>()
            }
        };
        MedicationAntibiotique medicationAntibiotique = new MedicationAntibiotique(new MedicationAllDrug());
        MedicationState medicationState = medicationAntibiotique.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Antibiotique, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime7.AddHours(20), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime7.AddHours(20), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(3, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_3_GreaterThan4Hours() {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage>()
            }
        };
        MedicationAntibiotique medicationAntibiotique = new MedicationAntibiotique(new MedicationAllDrug());
        MedicationState medicationState = medicationAntibiotique.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Antibiotique, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddHours(20), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddHours(20), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(3, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_4_LessThan4Hours() {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage>()
            }
        };
        MedicationAntibiotique medicationAntibiotique = new MedicationAntibiotique(new MedicationAllDrug());
        MedicationState medicationState = medicationAntibiotique.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Antibiotique, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime7.AddHours(20), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime7.AddHours(20), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(4, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_4_GreaterThan4Hours() {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage>()
            }
        };
        MedicationAntibiotique medicationAntibiotique = new MedicationAntibiotique(new MedicationAllDrug());
        MedicationState medicationState = medicationAntibiotique.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Antibiotique, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddHours(20), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddHours(20), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(4, medicationState.Dosage);
    }


    [Theory]
    [InlineData(-19, 1)]
    [InlineData(-21, 0)]
    public void GetMedicationState_1_20Hours(int offsetHours, int nb) {
        DateTime dateTime = DateTime.Now.AddHours(offsetHours);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime,
                Dosages = new List<MedicationDosage>()
            }
        };
        MedicationAntibiotique medicationAntibiotique = new MedicationAntibiotique(new MedicationAllDrug());
        MedicationState medicationState = medicationAntibiotique.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Antibiotique, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(nb, medicationState.Dosage);
    }

    [Theory]
    [InlineData(-19, 4)]
    [InlineData(-21, 3)]
    public void GetMedicationState_4_20Hours(int offsetHours, int nb) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime = DateTime.Now.AddHours(offsetHours);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime,
                Dosages = new List<MedicationDosage>()
            }
        };
        MedicationAntibiotique medicationAntibiotique = new MedicationAntibiotique(new MedicationAllDrug());
        MedicationState medicationState = medicationAntibiotique.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Antibiotique, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddHours(20), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddHours(20), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(nb, medicationState.Dosage);
    }

    [Theory]
    [InlineData(-19, 3)]
    [InlineData(-21, 2)]
    public void GetMedicationState_3_20Hours(int offsetHours, int nb) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime = DateTime.Now.AddHours(offsetHours);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Antibiotique,
                DateTime = dateTime,
                Dosages = new List<MedicationDosage>()
            }
        };
        MedicationAntibiotique medicationAntibiotique = new MedicationAntibiotique(new MedicationAllDrug());
        MedicationState medicationState = medicationAntibiotique.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Antibiotique, medicationState.DrugId);
        if (offsetHours > -20) {
            Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
            Assert.Equal(dateTime5, medicationState.LastMedicationNo);
            Assert.Equal(dateTime.AddHours(20), medicationState.NextMedicationPossible);
            Assert.Equal(dateTime.AddHours(20), medicationState.NextMedicationYes);
        }
        else {
            Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
            Assert.Null(medicationState.LastMedicationNo);
            Assert.Null(medicationState.NextMedicationPossible);
            Assert.Null(medicationState.NextMedicationYes);
        }
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(nb, medicationState.Dosage);
    }
}
