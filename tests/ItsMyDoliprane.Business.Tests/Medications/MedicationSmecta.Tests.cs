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

public class MedicationSmecta_Tests
{
    [Fact]
    public void GetMedicationState_Empty() {
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(new List<Medication>(), true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(0, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(0, medicationState.Dosages[0].Number);
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
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(0, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(0, medicationState.Dosages[0].Number);
        Assert.Equal(0, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_1_LessThan4Hours() {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime10 = DateTime.Now.AddHours(-10);
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
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime10,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(3000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(1, medicationState.Dosages[0].Number);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_1_LessThan4Hours_OtherLessThan2Hours() {
        DateTime dateTime1 = DateTime.Now.AddHours(-1);
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime1,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
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
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime1, medicationState.LastMedicationNo);
        Assert.Equal(dateTime1.AddHours(2), medicationState.NextMedicationPossible ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Equal(dateTime1.AddHours(2), medicationState.NextMedicationYes ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(3000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(1, medicationState.Dosages[0].Number);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_1_GreaterThan4Hours() {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime10 = DateTime.Now.AddHours(-10);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime10,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(3000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(1, medicationState.Dosages[0].Number);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_1_GreaterThan4Hours_OtherLessThan2Hours() {
        DateTime dateTime1 = DateTime.Now.AddHours(-1);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime1,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime1, medicationState.LastMedicationNo);
        Assert.Equal(dateTime1.AddHours(2), medicationState.NextMedicationPossible ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Equal(dateTime1.AddHours(2), medicationState.NextMedicationYes ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(3000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(1, medicationState.Dosages[0].Number);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_2_LessThan4Hours() {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime10 = DateTime.Now.AddHours(-10);
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
            },
            new() {
                DrugId = (int)DrugId.Diosmectite,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime10,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(6000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(2, medicationState.Dosages[0].Number);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_2_LessThan4Hours_OtherLessThan2Hours() {
        DateTime dateTime1 = DateTime.Now.AddHours(-1);
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime1,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Diosmectite,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime1, medicationState.LastMedicationNo);
        Assert.Equal(dateTime1.AddHours(2), medicationState.NextMedicationPossible ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Equal(dateTime1.AddHours(2), medicationState.NextMedicationYes ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(6000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(2, medicationState.Dosages[0].Number);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_2_GreaterThan4Hours() {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime10 = DateTime.Now.AddHours(-10);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Diosmectite,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime10,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(6000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(2, medicationState.Dosages[0].Number);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_2_GreaterThan4Hours_OtherLessThan2Hours() {
        DateTime dateTime1 = DateTime.Now.AddHours(-1);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime1,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Diosmectite,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime1, medicationState.LastMedicationNo);
        Assert.Equal(dateTime1.AddHours(2), medicationState.NextMedicationPossible ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Equal(dateTime1.AddHours(2), medicationState.NextMedicationYes ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(6000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(2, medicationState.Dosages[0].Number);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_3_LessThan4Hours() {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime10 = DateTime.Now.AddHours(-10);
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
            },
            new() {
                DrugId = (int)DrugId.Diosmectite,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime10,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(9000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(3, medicationState.Dosages[0].Number);
        Assert.Equal(2, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_3_LessThan4Hours_OtherLessThan2Hours() {
        DateTime dateTime1 = DateTime.Now.AddHours(-1);
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime1,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Diosmectite,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime1, medicationState.LastMedicationNo);
        Assert.Equal(dateTime1.AddHours(2), medicationState.NextMedicationPossible ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Equal(dateTime1.AddHours(2), medicationState.NextMedicationYes ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(9000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(3, medicationState.Dosages[0].Number);
        Assert.Equal(2, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_3_GreaterThan4Hours() {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime10 = DateTime.Now.AddHours(-10);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Diosmectite,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime10,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(9000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(3, medicationState.Dosages[0].Number);
        Assert.Equal(2, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_3_GreaterThan4Hours_OtherLessThan2Hours() {
        DateTime dateTime1 = DateTime.Now.AddHours(-1);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime1,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Diosmectite,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime1, medicationState.LastMedicationNo);
        Assert.Equal(dateTime1.AddHours(2), medicationState.NextMedicationPossible ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Equal(dateTime1.AddHours(2), medicationState.NextMedicationYes ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(9000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(3, medicationState.Dosages[0].Number);
        Assert.Equal(2, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_4_LessThan4Hours() {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime10 = DateTime.Now.AddHours(-10);
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
            },
            new() {
                DrugId = (int)DrugId.Diosmectite,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime10,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddHours(20), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddHours(20), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(12000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(4, medicationState.Dosages[0].Number);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_4_LessThan4Hours_OtherLessThan2Hours() {
        DateTime dateTime1 = DateTime.Now.AddHours(-1);
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime1,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Diosmectite,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime1, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddHours(20), medicationState.NextMedicationPossible ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Equal(dateTime9.AddHours(20), medicationState.NextMedicationYes ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(12000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(4, medicationState.Dosages[0].Number);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_4_GreaterThan4Hours() {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime10 = DateTime.Now.AddHours(-10);
        DateTime dateTime11 = DateTime.Now.AddHours(-11);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Diosmectite,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime10,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime11,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime11.AddHours(20), medicationState.NextMedicationPossible ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Equal(dateTime11.AddHours(20), medicationState.NextMedicationYes ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(12000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(4, medicationState.Dosages[0].Number);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_4_GreaterThan4Hours_OtherLessThan2Hours() {
        DateTime dateTime1 = DateTime.Now.AddHours(-1);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime11 = DateTime.Now.AddHours(-11);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime1,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Diosmectite,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime11,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime1, medicationState.LastMedicationNo);
        Assert.Equal(dateTime11.AddHours(20), medicationState.NextMedicationPossible ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Equal(dateTime11.AddHours(20), medicationState.NextMedicationYes ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(12000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(4, medicationState.Dosages[0].Number);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_5_LessThan4Hours() {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime10 = DateTime.Now.AddHours(-10);
        DateTime dateTime11 = DateTime.Now.AddHours(-11);
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
            },
            new() {
                DrugId = (int)DrugId.Diosmectite,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime10,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime11,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddHours(20), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddHours(20), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(15000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(5, medicationState.Dosages[0].Number);
        Assert.Equal(4, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_5_LessThan4Hours_OtherLessThan2Hours() {
        DateTime dateTime1 = DateTime.Now.AddHours(-1);
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime11 = DateTime.Now.AddHours(-11);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime1,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Diosmectite,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime11,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime1, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddHours(20), medicationState.NextMedicationPossible ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Equal(dateTime9.AddHours(20), medicationState.NextMedicationYes ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(15000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(5, medicationState.Dosages[0].Number);
        Assert.Equal(4, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_5_GreaterThan4Hours() {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime10 = DateTime.Now.AddHours(-10);
        DateTime dateTime11 = DateTime.Now.AddHours(-11);
        DateTime dateTime13 = DateTime.Now.AddHours(-13);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Diosmectite,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime10,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime11,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime13,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime11.AddHours(20), medicationState.NextMedicationPossible ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Equal(dateTime11.AddHours(20), medicationState.NextMedicationYes ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(15000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(5, medicationState.Dosages[0].Number);
        Assert.Equal(4, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_5_GreaterThan4Hours_OtherLessThan2Hours() {
        DateTime dateTime1 = DateTime.Now.AddHours(-1);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime11 = DateTime.Now.AddHours(-11);
        DateTime dateTime13 = DateTime.Now.AddHours(-13);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime1,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Diosmectite,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime11,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime13,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime1, medicationState.LastMedicationNo);
        Assert.Equal(dateTime11.AddHours(20), medicationState.NextMedicationPossible ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Equal(dateTime11.AddHours(20), medicationState.NextMedicationYes ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(15000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(5, medicationState.Dosages[0].Number);
        Assert.Equal(4, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(-19, 1)]
    [InlineData(-21, 0)]
    public void GetMedicationState_1_20Hours(int offsetHours, int nb) {
        DateTime dateTime = DateTime.Now.AddHours(offsetHours);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(nb * 3000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(nb, medicationState.Dosages[0].Number);
        Assert.Equal(nb, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(-19, 5)]
    [InlineData(-21, 4)]
    public void GetMedicationState_5_20Hours(int offsetHours, int nb) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime11 = DateTime.Now.AddHours(-11);
        DateTime dateTime = DateTime.Now.AddHours(offsetHours);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime11,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime11.AddHours(20), medicationState.NextMedicationPossible ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Equal(dateTime11.AddHours(20), medicationState.NextMedicationYes ?? DateTime.MinValue, TimeSpan.FromMilliseconds(1));
        Assert.Null(medicationState.NextDrug);
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(nb * 3000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(nb, medicationState.Dosages[0].Number);
        Assert.Equal(nb, medicationState.NumberMedication);
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
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Smecta,
                DateTime = dateTime,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Diosmectite,
                        Quantity = 3000
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationSmecta medication = new MedicationSmecta(drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Smecta, medicationState.DrugId);
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
        Assert.NotNull(medicationState.Dosages);
        Assert.Single(medicationState.Dosages);
        Assert.Equal(DrugCompositionId.Diosmectite, medicationState.Dosages[0].DrugCompositionId);
        Assert.Equal(nb * 3000, medicationState.Dosages[0].TotalQuantity);
        Assert.Equal(nb, medicationState.Dosages[0].Number);
        Assert.Equal(nb, medicationState.NumberMedication);
    }
}
