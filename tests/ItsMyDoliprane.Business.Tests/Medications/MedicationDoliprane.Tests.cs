using System;
using System.Collections.Generic;
using ItsMyDoliprane.Business.Enums;
using Xunit;
using ItsMyDoliprane.Business.Medications;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Business.Tests.Mocks;
using ItsMyDoliprane.Repository.Boundary;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business.Tests.Medications;

public class MedicationDoliprane_Tests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Empty(bool isAdult) {
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(new List<Medication>(), isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Equal(0, medicationState.Dosage);
        Assert.Equal(0, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_EmptyAndOtherDrugComposition(bool isAdult) {
        DateTime dateTime2 = DateTime.Now.AddHours(-2);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 2,
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Equal(0, medicationState.Dosage);
        Assert.Equal(0, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_LessThan4Hours_Dosage2000(bool isAdult) {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime3.AddHours(isAdult ? 4 : 6), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime3.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(2000, medicationState.Dosage);
        Assert.Equal(2, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_LessThan4Hours_Dosage2500_Last_LessThan4Hours(bool isAdult) {
        DateTime dateTime2 = DateTime.Now.AddHours(-2);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTimeLast = DateTime.Now.AddDays(-1).AddHours(1);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime2,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTimeLast,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(dateTime2.AddHours(isAdult ? 4 : 6), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime2.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(2500, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_LessThan4Hours_Dosage2500_Last_Between4And6Hours(bool isAdult) {
        DateTime dateTime2 = DateTime.Now.AddHours(-2);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTimeLast = DateTime.Now.AddDays(-1).AddHours(3);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime2,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTimeLast,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(isAdult ? dateTimeLast.AddDays(1) : dateTime2.AddHours(6), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime2.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(2500, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_LessThan4Hours_Dosage2500_Last_LaterThan6Hours(bool isAdult) {
        DateTime dateTime2 = DateTime.Now.AddHours(-2);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTimeLast = DateTime.Now.AddDays(-1).AddHours(7);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime2,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTimeLast,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(2500, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_LessThan4Hours_Dosage3000_Last_LessThan4Hours(bool isAdult) {
        DateTime dateTime2 = DateTime.Now.AddHours(-2);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTimeLast = DateTime.Now.AddDays(-1).AddHours(1);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime2,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTimeLast,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(dateTime2.AddHours(isAdult ? 4 : 6), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime2.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(3000, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_LessThan4Hours_Dosage3000_Last_Between4And6Hours(bool isAdult) {
        DateTime dateTime2 = DateTime.Now.AddHours(-2);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTimeLast = DateTime.Now.AddDays(-1).AddHours(3);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime2,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTimeLast,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(isAdult ? dateTimeLast.AddDays(1) : dateTime2.AddHours(6), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime2.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(3000, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_LessThan4Hours_Dosage3000_Last_LaterThan6Hours(bool isAdult) {
        DateTime dateTime2 = DateTime.Now.AddHours(-2);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTimeLast = DateTime.Now.AddDays(1).AddHours(7);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime2,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTimeLast,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(3000, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_LessThan4Hours_Dosage4000_Last_LessThan4Hours(bool isAdult) {
        DateTime dateTime2 = DateTime.Now.AddHours(-2);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTimeBeforeLast = DateTime.Now.AddDays(-1).AddHours(1);
        DateTime dateTimeLast = DateTime.Now.AddDays(-1).AddMinutes(30);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime2,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTimeBeforeLast,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTimeLast,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(dateTime2.AddHours(isAdult ? 4 : 6), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime2.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(4000, medicationState.Dosage);
        Assert.Equal(4, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_LessThan4Hours_Dosage4000_Last_Between4And6Hours(bool isAdult) {
        DateTime dateTime2 = DateTime.Now.AddHours(-2);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTimeBeforeLast = DateTime.Now.AddDays(-1).AddHours(3);
        DateTime dateTimeLast = DateTime.Now.AddDays(-1).AddHours(1);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime2,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTimeBeforeLast,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTimeLast,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(isAdult ? dateTimeBeforeLast.AddDays(1) : dateTime2.AddHours(6), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime2.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(4000, medicationState.Dosage);
        Assert.Equal(4, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_LessThan4Hours_Dosage4000_Last_LaterThan6Hours(bool isAdult) {
        DateTime dateTime2 = DateTime.Now.AddHours(-2);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTimeBeforeLast = DateTime.Now.AddDays(-1).AddHours(7);
        DateTime dateTimeLast = DateTime.Now.AddDays(-1).AddHours(3);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime2,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTimeBeforeLast,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTimeLast,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeBeforeLast.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeBeforeLast.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(4000, medicationState.Dosage);
        Assert.Equal(4, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Between4And6Hours_Dosage2000(bool isAdult) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(isAdult ? MedicationOpinion.Possible : MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        if (isAdult)
            Assert.Null(medicationState.NextMedicationPossible);
        else
            Assert.Equal(dateTime5.AddHours(6), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime5.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(2000, medicationState.Dosage);
        Assert.Equal(2, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Between4And6Hours_Dosage2500_Last_Between4And6Hours(bool isAdult) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTimeLast = DateTime.Now.AddDays(-1).AddMinutes(30);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTimeLast,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(isAdult ? MedicationOpinion.Warning : MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(isAdult ? dateTimeLast.AddDays(1) : dateTime5.AddHours(6), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime5.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(2500, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Between4And6Hours_Dosage2500_Last_LaterThan6Hours(bool isAdult) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTimeLast = DateTime.Now.AddDays(-1).AddHours(4);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTimeLast,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(isAdult ? MedicationOpinion.Warning : MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(2500, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Between4And6Hours_Dosage3000_Last_Between4And6Hours(bool isAdult) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTimeLast = DateTime.Now.AddDays(-1).AddMinutes(30);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTimeLast,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(isAdult ? MedicationOpinion.Warning : MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(isAdult ? dateTimeLast.AddDays(1) : dateTime5.AddHours(6), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime5.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(3000, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Between4And6Hours_Dosage3000_Last_LaterThan6Hours(bool isAdult) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTimeLast = DateTime.Now.AddDays(-1).AddHours(4);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTimeLast,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(isAdult ? MedicationOpinion.Warning : MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(3000, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Between4And6Hours_Dosage4000(bool isAdult) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(4000, medicationState.Dosage);
        Assert.Equal(4, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_LaterThan6Hours_Dosage2000(bool isAdult) {
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Equal(2000, medicationState.Dosage);
        Assert.Equal(2, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_LaterThan6Hours_Dosage2500(bool isAdult) {
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTime14 = DateTime.Now.AddHours(-14);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime14,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTime9, medicationState.LastMedicationNo);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(2500, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_LaterThan6Hours_Dosage3000(bool isAdult) {
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTime14 = DateTime.Now.AddHours(-14);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime14,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTime9, medicationState.LastMedicationNo);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(3000, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_LaterThan6Hours_Dosage4000(bool isAdult) {
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTime14 = DateTime.Now.AddHours(-14);
        DateTime dateTime18 = DateTime.Now.AddHours(-18);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime14,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime18,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime9, medicationState.LastMedicationNo);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(4000, medicationState.Dosage);
        Assert.Equal(4, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_LessThan4Hours_Smecta(bool isAdult) {
        DateTime dateTime1 = DateTime.Now.AddHours(-1).AddMinutes(1);
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
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
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medication = new(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime1, medicationState.LastMedicationNo);
        Assert.Equal(isAdult ? dateTime1.AddHours(2) : dateTime3.AddHours(6), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime3.AddHours(6), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1000, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Ibuprofene_LessThan3Hours(bool isAdult) {
        DateTime dateTimeIbu = DateTime.Now.AddHours(-2);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medication = new(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeIbu, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(0, medicationState.Dosage);
        Assert.Equal(0, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Ibuprofene_LessThan3Hours__Doliprane_LessThan6Hours(bool isAdult) {
        DateTime dateTimeIbu = DateTime.Now.AddHours(-2);
        DateTime dateTimeDoli = DateTime.Now.AddHours(-5);
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
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medication = new(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeIbu, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1000, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Ibuprofene_LessThan3Hours__Doliprane_GreaterThan6Hours(bool isAdult) {
        DateTime dateTimeIbu = DateTime.Now.AddHours(-2);
        DateTime dateTimeDoli = DateTime.Now.AddHours(-7);
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
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medication = new(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeIbu, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1000, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Ibuprofene_Between3And4Hours(bool isAdult) {
        DateTime dateTimeIbu = DateTime.Now.AddHours(-4).AddMinutes(30);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medication = new(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTimeIbu, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(0, medicationState.Dosage);
        Assert.Equal(0, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Ibuprofene_Between3And4Hours__Doliprane_LessThan6Hours(bool isAdult) {
        DateTime dateTimeIbu = DateTime.Now.AddHours(-4).AddMinutes(30);
        DateTime dateTimeDoli = DateTime.Now.AddHours(-6).AddMinutes(1);
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
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medication = new(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeIbu, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1000, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Ibuprofene_Between3And4Hours__Doliprane_GreaterThan6Hours(bool isAdult) {
        DateTime dateTimeIbu = DateTime.Now.AddHours(-4).AddMinutes(30);
        DateTime dateTimeDoli = DateTime.Now.AddHours(-7);
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
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medication = new(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTimeIbu, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1000, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Ibuprofene_GreaterThan4Hours(bool isAdult) {
        DateTime dateTimeIbu = DateTime.Now.AddHours(-5);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medication = new(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(0, medicationState.Dosage);
        Assert.Equal(0, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Ibuprofene_GreaterThan4Hours__Doliprane_LessThan6Hours(bool isAdult) {
        DateTime dateTimeIbu = DateTime.Now.AddHours(-4).AddMinutes(-1);
        DateTime dateTimeDoli = DateTime.Now.AddHours(-6).AddMinutes(1);
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
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medication = new(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeIbu, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeDoli.AddHours(6), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeDoli.AddHours(6), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1000, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Ibuprofene_GreaterThan4Hours__Doliprane_GreaterThan6Hours(bool isAdult) {
        DateTime dateTimeIbu = DateTime.Now.AddHours(-5);
        DateTime dateTimeDoli = DateTime.Now.AddHours(-6);
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
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationDoliprane medication = new(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1000, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Ibuprofene_GreaterThan6Hours__Doliprane_LessThan4Hours(bool isAdult) {
        DateTime dateTimeDoli = DateTime.Now.AddHours(-3);
        DateTime dateTimeIbu = DateTime.Now.AddHours(-20);
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
        MedicationDoliprane medication = new(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeDoli, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeDoli.AddHours(isAdult ? 4 : 6), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeDoli.AddHours(6), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1000, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Ibuprofene_GreaterThan6Hours__Doliprane_LessThan6Hours(bool isAdult) {
        DateTime dateTimeDoli = DateTime.Now.AddHours(-5);
        DateTime dateTimeIbu = DateTime.Now.AddHours(-20);
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
        MedicationDoliprane medication = new(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(isAdult ? MedicationOpinion.Possible : MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeDoli, medicationState.LastMedicationNo);
        if (isAdult)
            Assert.Null(medicationState.NextMedicationPossible);
        else
            Assert.Equal(dateTimeDoli.AddHours(6), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeDoli.AddHours(6), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1000, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Ibuprofene_GreaterThan6Hours__Doliprane_GreaterThan6Hours(bool isAdult) {
        DateTime dateTimeDoli = DateTime.Now.AddHours(-7);
        DateTime dateTimeIbu = DateTime.Now.AddHours(-20);
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
        MedicationDoliprane medication = new(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(1000, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }
}
