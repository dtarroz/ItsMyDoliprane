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

public class MedicationIbuprofene_Tests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Empty(bool isAdult) {
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(new List<Medication>(), isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(0, medicationState.Dosage);
        Assert.Equal(0, medicationState.NumberMedication);
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
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(0, medicationState.Dosage);
        Assert.Equal(0, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_1_LessThan8Hours() {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime5.AddHours(8), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime5.AddHours(8), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_1_GreaterThan4Hours() {
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_2_LessThan8Hours() {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime5.AddHours(8), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime5.AddHours(8), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(2, medicationState.Dosage);
        Assert.Equal(2, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_2_GreaterThan8Hours() {
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(2, medicationState.Dosage);
        Assert.Equal(2, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_3_LessThan8Hours() {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(3, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_3_GreaterThan8Hours() {
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTime15 = DateTime.Now.AddHours(-15);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime15,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime9, medicationState.LastMedicationNo);
        Assert.Equal(dateTime15.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime15.AddDays(1), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(3, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_4_LessThan8Hours() {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTime15 = DateTime.Now.AddHours(-15);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime15,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(4, medicationState.Dosage);
        Assert.Equal(4, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_4_GreaterThan8Hours() {
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTime15 = DateTime.Now.AddHours(-15);
        DateTime dateTime17 = DateTime.Now.AddHours(-17);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime15,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime17,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime9, medicationState.LastMedicationNo);
        Assert.Equal(dateTime15.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime15.AddDays(1), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(4, medicationState.Dosage);
        Assert.Equal(4, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_LessThan8Hours_Smecta() {
        DateTime dateTime1 = DateTime.Now.AddHours(-1).AddMinutes(1);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
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
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime1, medicationState.LastMedicationNo);
        Assert.Equal(dateTime1.AddHours(2), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime1.AddHours(2), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_Doliprane_LessThan3Hours() {
        DateTime dateTimeDoli = DateTime.Now.AddHours(-2);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTimeDoli,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeDoli, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeDoli.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeDoli.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(0, medicationState.Dosage);
        Assert.Equal(0, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_Doliprane_LessThan3Hours__Ibuprofene_LessThan6Hours() {
        DateTime dateTimeDoli = DateTime.Now.AddHours(-2);
        DateTime dateTimeIbu = DateTime.Now.AddHours(-5);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTimeDoli,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeDoli, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeIbu.AddHours(8), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeIbu.AddHours(8), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_Doliprane_LessThan3Hours__Ibuprofene_Between6And8Hours() {
        DateTime dateTimeDoli = DateTime.Now.AddHours(-2);
        DateTime dateTimeIbu = DateTime.Now.AddHours(-7);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTimeDoli,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeDoli, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeDoli.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeDoli.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_Doliprane_LessThan3Hours__Ibuprofene_GreaterThan8Hours() {
        DateTime dateTimeDoli = DateTime.Now.AddHours(-2);
        DateTime dateTimeIbu = DateTime.Now.AddHours(-9);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTimeDoli,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeDoli, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeDoli.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeDoli.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_Doliprane_Between3And4Hours() {
        DateTime dateTimeDoli = DateTime.Now.AddHours(-4).AddMinutes(30);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTimeDoli,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTimeDoli, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeDoli.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeDoli.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(0, medicationState.Dosage);
        Assert.Equal(0, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_Doliprane_Between3And4Hours__Ibuprofene_LessThan6Hours() {
        DateTime dateTimeDoli = DateTime.Now.AddHours(-4).AddMinutes(30);
        DateTime dateTimeIbu = DateTime.Now.AddHours(-5);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTimeDoli,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeDoli, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeIbu.AddHours(8), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeIbu.AddHours(8), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_Doliprane_Between3And4Hours__Ibuprofene_Between6And8Hours() {
        DateTime dateTimeDoli = DateTime.Now.AddHours(-4).AddMinutes(30);
        DateTime dateTimeIbu = DateTime.Now.AddHours(-7);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTimeDoli,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTimeDoli, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeIbu.AddHours(8), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeIbu.AddHours(8), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_Doliprane_Between3And4Hours__Ibuprofene_GreaterThan8Hours() {
        DateTime dateTimeDoli = DateTime.Now.AddHours(-4).AddMinutes(30);
        DateTime dateTimeIbu = DateTime.Now.AddHours(-9);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTimeDoli,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTimeDoli, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeDoli.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeDoli.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_Doliprane_GreaterThan4Hours() {
        DateTime dateTimeDoli = DateTime.Now.AddHours(-5);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTimeDoli,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(0, medicationState.Dosage);
        Assert.Equal(0, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_Doliprane_GreaterThan4Hours__Ibuprofene_LessThan6Hours() {
        DateTime dateTimeDoli = DateTime.Now.AddHours(-5).AddMinutes(1);
        DateTime dateTimeIbu = DateTime.Now.AddHours(-5);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTimeDoli,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeDoli, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeIbu.AddHours(8), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeIbu.AddHours(8), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_Doliprane_GreaterThan4Hours__Ibuprofene_Between6And8Hours() {
        DateTime dateTimeDoli = DateTime.Now.AddHours(-5);
        DateTime dateTimeIbu = DateTime.Now.AddHours(-7);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTimeDoli,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTimeDoli, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeIbu.AddHours(8), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeIbu.AddHours(8), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_Doliprane_GreaterThan4Hours__Ibuprofene_GreaterThan8Hours() {
        DateTime dateTimeDoli = DateTime.Now.AddHours(-5);
        DateTime dateTimeIbu = DateTime.Now.AddHours(-9);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTimeDoli,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Doliprane_GreaterThan6Hours__Ibuprofene_LessThan6Hours(bool isAdult) {
        DateTime dateTimeIbu = DateTime.Now.AddHours(-5);
        DateTime dateTimeDoli = DateTime.Now.AddHours(-20);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTimeDoli,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeIbu, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeIbu.AddHours(8), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeIbu.AddHours(8), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Doliprane_GreaterThan6Hours__Ibuprofene_LessThan8Hours(bool isAdult) {
        DateTime dateTimeIbu = DateTime.Now.AddHours(-7);
        DateTime dateTimeDoli = DateTime.Now.AddHours(-20);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTimeDoli,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeIbu, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeIbu.AddHours(8), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeIbu.AddHours(8), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Doliprane_GreaterThan6Hours__Ibuprofene_GreaterThan8Hours(bool isAdult) {
        DateTime dateTimeIbu = DateTime.Now.AddHours(-9);
        DateTime dateTimeDoli = DateTime.Now.AddHours(-20);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTimeDoli,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationIbuprofene medication = new MedicationIbuprofene(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Ibuprofene, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }
}
