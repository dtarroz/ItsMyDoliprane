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

public class MedicationHumex_Tests
{
    [Fact]
    public void GetMedicationState_Empty() {
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(new List<Medication>(), true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
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
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(0, medicationState.Dosage);
        Assert.Equal(0, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(-19, DrugId.HumexJour)]
    [InlineData(-21, null)]
    public void GetMedicationState_1Jour_LessThan4Hours_LessThanDosage3000(double? humexNuitHour, DrugId? nextDrug) {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = DateTime.Now.AddHours(humexNuitHour.Value),
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
        Assert.Equal(2500, medicationState.Dosage);
        Assert.Equal(1 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(-19, DrugId.HumexJour)]
    [InlineData(-21, null)]
    public void GetMedicationState_1Jour_GreaterThan4Hours_LessThanDosage3000(double? humexNuitHour, DrugId? nextDrug) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = DateTime.Now.AddHours(humexNuitHour.Value),
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
        Assert.Equal(2500, medicationState.Dosage);
        Assert.Equal(1 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(-19, DrugId.HumexJour)]
    [InlineData(-21, null)]
    public void GetMedicationState_1Jour_LessThan4Hours_LessThanDosage3500(double? humexNuitHour, DrugId? nextDrug) {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = DateTime.Now.AddHours(humexNuitHour.Value),
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
        Assert.Equal(3500, medicationState.Dosage);
        Assert.Equal(1 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(-19, DrugId.HumexJour)]
    [InlineData(-21, null)]
    public void GetMedicationState_1Jour_GreaterThan4Hours_LessThanDosage3500(double? humexNuitHour, DrugId? nextDrug) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = DateTime.Now.AddHours(humexNuitHour.Value),
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
        Assert.Equal(3500, medicationState.Dosage);
        Assert.Equal(1 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(-19, DrugId.HumexJour)]
    [InlineData(-21, null)]
    public void GetMedicationState_1Jour_LessThan4Hours_GreaterThanDosage4000(double? humexNuitHour, DrugId? nextDrug) {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = DateTime.Now.AddHours(humexNuitHour.Value),
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
        Assert.Equal(4500, medicationState.Dosage);
        Assert.Equal(1 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(-19, DrugId.HumexJour)]
    [InlineData(-21, null)]
    public void GetMedicationState_1Jour_GreaterThan4Hours_GreaterThanDosage4000(double? humexNuitHour, DrugId? nextDrug) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTime14 = DateTime.Now.AddHours(-14);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime14,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = DateTime.Now.AddHours(humexNuitHour.Value),
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
        Assert.Equal(4500, medicationState.Dosage);
        Assert.Equal(1 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(-19, DrugId.HumexJour)]
    [InlineData(-21, null)]
    public void GetMedicationState_2Jour_LessThan4Hours_LessThanDosage3000(double? humexNuitHour, DrugId? nextDrug) {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = DateTime.Now.AddHours(humexNuitHour.Value),
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
        Assert.Equal(2000, medicationState.Dosage);
        Assert.Equal(2 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(-19, DrugId.HumexJour)]
    [InlineData(-21, null)]
    public void GetMedicationState_2Jour_GreaterThan4Hours_LessThanDosage3000(double? humexNuitHour, DrugId? nextDrug) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = DateTime.Now.AddHours(humexNuitHour.Value),
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
        Assert.Equal(2000, medicationState.Dosage);
        Assert.Equal(2 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(-19, DrugId.HumexJour)]
    [InlineData(-21, null)]
    public void GetMedicationState_2Jour_LessThan4Hours_LessThanDosage3500(double? humexNuitHour, DrugId? nextDrug) {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = DateTime.Now.AddHours(humexNuitHour.Value),
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
        Assert.Equal(3000, medicationState.Dosage);
        Assert.Equal(2 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(-19, DrugId.HumexJour)]
    [InlineData(-21, null)]
    public void GetMedicationState_2Jour_GreaterThan4Hours_LessThanDosage3500(double? humexNuitHour, DrugId? nextDrug) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = DateTime.Now.AddHours(humexNuitHour.Value),
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
        Assert.Equal(3000, medicationState.Dosage);
        Assert.Equal(2 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(-19, DrugId.HumexJour)]
    [InlineData(-21, null)]
    public void GetMedicationState_2Jour_LessThan4Hours_GreaterThanDosage4000(double? humexNuitHour, DrugId? nextDrug) {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = DateTime.Now.AddHours(humexNuitHour.Value),
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
        Assert.Equal(4000, medicationState.Dosage);
        Assert.Equal(2 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(-19, DrugId.HumexJour)]
    [InlineData(-21, null)]
    public void GetMedicationState_2Jour_GreaterThan4Hours_GreaterThanDosage4000(double? humexNuitHour, DrugId? nextDrug) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTime14 = DateTime.Now.AddHours(-14);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime14,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = DateTime.Now.AddHours(humexNuitHour.Value),
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
        Assert.Equal(4000, medicationState.Dosage);
        Assert.Equal(2 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null, DrugId.HumexNuit)]
    [InlineData(-17, null)]
    [InlineData(-19, null)]
    [InlineData(-21, DrugId.HumexNuit)]
    public void GetMedicationState_3Jour_LessThan4Hours_LessThanDosage3000(double? humexNuitHour, DrugId? drugId) {
        DateTime dateTime2 = DateTime.Now.AddHours(-2);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTimeNuit = DateTime.Now.AddHours(humexNuitHour ?? 0);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime2,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = dateTimeNuit,
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        if (humexNuitHour > -18) {
            Assert.Equal(dateTimeNuit.AddHours(20), medicationState.NextMedicationPossible);
            Assert.Equal(dateTimeNuit.AddHours(20), medicationState.NextMedicationYes);
        }
        else {
            Assert.Equal(dateTime2.AddHours(4), medicationState.NextMedicationPossible);
            Assert.Equal(dateTime2.AddHours(4), medicationState.NextMedicationYes);
        }
        Assert.Equal(drugId, medicationState.NextDrug);
        Assert.Equal(1500, medicationState.Dosage);
        Assert.Equal(3 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(-19)]
    [InlineData(-21)]
    public void GetMedicationState_3Jour_GreaterThan4Hours_LessThanDosage3000(double? humexNuitHour) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTimeNuit = DateTime.Now.AddHours(humexNuitHour ?? 0);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = dateTimeNuit,
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        if (humexNuitHour > -20) {
            Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
            Assert.Equal(dateTime5, medicationState.LastMedicationNo);
            Assert.Equal(dateTimeNuit.AddHours(20), medicationState.NextMedicationPossible);
            Assert.Equal(dateTimeNuit.AddHours(20), medicationState.NextMedicationYes);
            Assert.Null(medicationState.NextDrug);
        }
        else {
            Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
            Assert.Null(medicationState.LastMedicationNo);
            Assert.Null(medicationState.NextMedicationPossible);
            Assert.Null(medicationState.NextMedicationYes);
            Assert.Equal(DrugId.HumexNuit, medicationState.NextDrug);
        }
        Assert.Equal(1500, medicationState.Dosage);
        Assert.Equal(3 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null, DrugId.HumexNuit)]
    [InlineData(-17, null)]
    [InlineData(-19, null)]
    [InlineData(-21, DrugId.HumexNuit)]
    public void GetMedicationState_3Jour_LessThan4Hours_LessThanDosage3500(double? humexNuitHour, DrugId? drugId) {
        DateTime dateTime2 = DateTime.Now.AddHours(-2);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTimeNuit = DateTime.Now.AddHours(humexNuitHour ?? 0);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime2,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = dateTimeNuit,
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(drugId, medicationState.NextDrug);
        Assert.Equal(3500, medicationState.Dosage);
        Assert.Equal(3 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null, DrugId.HumexNuit)]
    [InlineData(-19, null)]
    [InlineData(-21, DrugId.HumexNuit)]
    public void GetMedicationState_3Jour_GreaterThan4Hours_LessThanDosage3500(double? humexNuitHour, DrugId? drugId) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTime14 = DateTime.Now.AddHours(-14);
        DateTime dateTimeNuit = DateTime.Now.AddHours(humexNuitHour ?? 0);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime14,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = dateTimeNuit,
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        if (humexNuitHour > -20) {
            Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
            Assert.Null(medicationState.NextDrug);
        }
        else {
            Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
            Assert.Equal(drugId, medicationState.NextDrug);
        }
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(3500, medicationState.Dosage);
        Assert.Equal(3 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null, DrugId.HumexNuit)]
    [InlineData(-17, null)]
    [InlineData(-19, null)]
    [InlineData(-21, DrugId.HumexNuit)]
    public void GetMedicationState_3Jour_LessThan4Hours_GreaterThanDosage4000(double? humexNuitHour, DrugId? drugId) {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTime14 = DateTime.Now.AddHours(-14);
        DateTime dateTimeNuit = DateTime.Now.AddHours(humexNuitHour ?? 0);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime14,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = dateTimeNuit,
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(drugId, medicationState.NextDrug);
        Assert.Equal(4500, medicationState.Dosage);
        Assert.Equal(3 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null, DrugId.HumexNuit)]
    [InlineData(-19, null)]
    [InlineData(-21, DrugId.HumexNuit)]
    public void GetMedicationState_3Jour_GreaterThan4Hours_GreaterThanDosage4000(double? humexNuitHour, DrugId? drugId) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTime14 = DateTime.Now.AddHours(-14);
        DateTime dateTime16 = DateTime.Now.AddHours(-16);
        DateTime dateTimeNuit = DateTime.Now.AddHours(humexNuitHour ?? 0);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime14,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime16,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = dateTimeNuit,
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(drugId, medicationState.NextDrug);
        Assert.Equal(4500, medicationState.Dosage);
        Assert.Equal(3 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(-17)]
    [InlineData(-19)]
    [InlineData(-21)]
    public void GetMedicationState_4Jour_LessThan4Hours_LessThanDosage3000(double? humexNuitHour) {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTimeNuit = DateTime.Now.AddHours(humexNuitHour ?? 0);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = dateTimeNuit,
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddHours(20), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddHours(20), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(2000, medicationState.Dosage);
        Assert.Equal(4 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(-19)]
    [InlineData(-21)]
    public void GetMedicationState_4Jour_GreaterThan4Hours_LessThanDosage3000(double? humexNuitHour) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTimeNuit = DateTime.Now.AddHours(humexNuitHour ?? 0);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = dateTimeNuit,
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddHours(20), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddHours(20), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(2000, medicationState.Dosage);
        Assert.Equal(4 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(-19)]
    [InlineData(-21)]
    public void GetMedicationState_4Jour_GreaterThan4Hours_LessThanDosage3500(double? humexNuitHour) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTime14 = DateTime.Now.AddHours(-14);
        DateTime dateTimeNuit = DateTime.Now.AddHours(humexNuitHour ?? 0);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime14,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = dateTimeNuit,
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(3000, medicationState.Dosage);
        Assert.Equal(4 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(-17)]
    [InlineData(-19)]
    [InlineData(-21)]
    public void GetMedicationState_4Jour_LessThan4Hours_GreaterThanDosage4000(double? humexNuitHour) {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTime14 = DateTime.Now.AddHours(-14);
        DateTime dateTimeNuit = DateTime.Now.AddHours(humexNuitHour ?? 0);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime14,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = dateTimeNuit,
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(4000, medicationState.Dosage);
        Assert.Equal(4 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(-19)]
    [InlineData(-21)]
    public void GetMedicationState_4Jour_GreaterThan4Hours_GreaterThanDosage4000(double? humexNuitHour) {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTime14 = DateTime.Now.AddHours(-14);
        DateTime dateTime16 = DateTime.Now.AddHours(-16);
        DateTime dateTimeNuit = DateTime.Now.AddHours(humexNuitHour ?? 0);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime14,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime16,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        if (humexNuitHour != null)
            medications.Add(new Medication {
                                DrugId = (int)DrugId.HumexNuit,
                                DateTime = dateTimeNuit,
                                Dosages = new List<MedicationDosage> {
                                    new() {
                                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                                        Quantity = 0
                                    }
                                }
                            });
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(4000, medicationState.Dosage);
        Assert.Equal(4 + (humexNuitHour > -20 ? 1 : 0), medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_3Jour_1Jour_LessThan4Hours_LessThanDosage3000() {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime21 = DateTime.Now.AddHours(-21);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime21,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationYes);
        Assert.Equal(DrugId.HumexNuit, medicationState.NextDrug);
        Assert.Equal(2000, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_3Jour_1Jour_GreaterThan4Hours_LessThanDosage3000() {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime21 = DateTime.Now.AddHours(-21);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime21,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Equal(DrugId.HumexNuit, medicationState.NextDrug);
        Assert.Equal(2000, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_3Jour_1Jour_LessThan4Hours_LessThanDosage3500() {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTime21 = DateTime.Now.AddHours(-21);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime21,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(DrugId.HumexNuit, medicationState.NextDrug);
        Assert.Equal(4000, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_3Jour_1Jour_GreaterThan4Hours_LessThanDosage3500() {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTime21 = DateTime.Now.AddHours(-21);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime21,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime21.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime21.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(DrugId.HumexNuit, medicationState.NextDrug);
        Assert.Equal(3000, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_3Jour_1Jour_LessThan4Hours_GreaterThanDosage4000() {
        DateTime dateTime3 = DateTime.Now.AddHours(-3);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTime14 = DateTime.Now.AddHours(-14);
        DateTime dateTime21 = DateTime.Now.AddHours(-21);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime14,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime21,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(DrugId.HumexNuit, medicationState.NextDrug);
        Assert.Equal(5000, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_3Jour_1Jour_GreaterThan4Hours_GreaterThanDosage4000() {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        DateTime dateTime12 = DateTime.Now.AddHours(-12);
        DateTime dateTime14 = DateTime.Now.AddHours(-14);
        DateTime dateTime21 = DateTime.Now.AddHours(-21);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.Doliprane,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime14,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime21,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(DrugId.HumexNuit, medicationState.NextDrug);
        Assert.Equal(4000, medicationState.Dosage);
        Assert.Equal(3, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_1Nuit_LessThan6Hours() {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexNuit,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime5.AddHours(6), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime5.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(DrugId.HumexJour, medicationState.NextDrug);
        Assert.Equal(500, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_1Nuit_GreaterThan6Hours() {
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexNuit,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Equal(DrugId.HumexJour, medicationState.NextDrug);
        Assert.Equal(500, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_1Nuit_LessThan20Hours() {
        DateTime dateTime21 = DateTime.Now.AddHours(-19);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexNuit,
                DateTime = dateTime21,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Equal(DrugId.HumexJour, medicationState.NextDrug);
        Assert.Equal(500, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Fact]
    public void GetMedicationState_1Nuit_GreaterThan20Hours() {
        DateTime dateTime21 = DateTime.Now.AddHours(-21);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexNuit,
                DateTime = dateTime21,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medicationHumex = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medicationHumex.GetMedicationState(medications, true);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(500, medicationState.Dosage);
        Assert.Equal(0, medicationState.NumberMedication);
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
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTime3,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medication = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime1, medicationState.LastMedicationNo);
        Assert.Equal(dateTime1.AddHours(2), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime1.AddHours(2), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(500, medicationState.Dosage);
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
        MedicationHumex medication = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
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
    public void GetMedicationState_Ibuprofene_LessThan3Hours__Humex_LessThan6Hours(bool isAdult) {
        DateTime dateTimeIbu = DateTime.Now.AddHours(-2);
        DateTime dateTimeHum = DateTime.Now.AddHours(-5);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTimeHum,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medication = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeIbu, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(500, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Ibuprofene_LessThan3Hours__Humex_GreaterThan6Hours(bool isAdult) {
        DateTime dateTimeIbu = DateTime.Now.AddHours(-2);
        DateTime dateTimeHum = DateTime.Now.AddHours(-7);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTimeHum,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medication = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeIbu, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(500, medicationState.Dosage);
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
        MedicationHumex medication = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
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
    public void GetMedicationState_Ibuprofene_Between3And4Hours__Humex_LessThan6Hours(bool isAdult) {
        DateTime dateTimeIbu = DateTime.Now.AddHours(-4).AddMinutes(30);
        DateTime dateTimeHum = DateTime.Now.AddHours(-6).AddMinutes(1);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTimeHum,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medication = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeIbu, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(500, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Ibuprofene_Between3And4Hours__Humex_GreaterThan6Hours(bool isAdult) {
        DateTime dateTimeIbu = DateTime.Now.AddHours(-4).AddMinutes(30);
        DateTime dateTimeHum = DateTime.Now.AddHours(-7);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTimeHum,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medication = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTimeIbu, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeIbu.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(500, medicationState.Dosage);
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
        MedicationHumex medication = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
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
    public void GetMedicationState_Ibuprofene_GreaterThan4Hours__Humex_LessThan6Hours(bool isAdult) {
        DateTime dateTimeIbu = DateTime.Now.AddHours(-4).AddMinutes(-1);
        DateTime dateTimeHum = DateTime.Now.AddHours(-6).AddMinutes(1);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTimeHum,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medication = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeIbu, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeHum.AddHours(6), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeHum.AddHours(6), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(500, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Ibuprofene_GreaterThan4Hours__Humex_GreaterThan6Hours(bool isAdult) {
        DateTime dateTimeIbu = DateTime.Now.AddHours(-5);
        DateTime dateTimeHum = DateTime.Now.AddHours(-6);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.Ibuprofene,
                DateTime = dateTimeIbu,
                Dosages = new List<MedicationDosage>()
            },
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTimeHum,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
                    }
                }
            }
        };
        IDrugRepository drugRepository = new DrugRepositoryMock();
        MedicationHumex medication = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(500, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Ibuprofene_GreaterThan6Hours__Humex_LessThan4Hours(bool isAdult) {
        DateTime dateTimeHum = DateTime.Now.AddHours(-3);
        DateTime dateTimeIbu = DateTime.Now.AddHours(-20);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTimeHum,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
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
        MedicationHumex medication = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTimeHum, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeHum.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeHum.AddHours(4), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(500, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Ibuprofene_GreaterThan6Hours__Humex_LessThan6Hours(bool isAdult) {
        DateTime dateTimeHum = DateTime.Now.AddHours(-5);
        DateTime dateTimeIbu = DateTime.Now.AddHours(-20);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTimeHum,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
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
        MedicationHumex medication = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(500, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMedicationState_Ibuprofene_GreaterThan6Hours__Humex_GreaterThan6Hours(bool isAdult) {
        DateTime dateTimeHum = DateTime.Now.AddHours(-7);
        DateTime dateTimeIbu = DateTime.Now.AddHours(-20);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = (int)DrugId.HumexJour,
                DateTime = dateTimeHum,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugCompositionId.Paracetamol,
                        Quantity = 500
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
        MedicationHumex medication = new MedicationHumex(new MedicationAllDrug(drugRepository), drugRepository);
        MedicationState medicationState = medication.GetMedicationState(medications, isAdult);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        Assert.Equal(500, medicationState.Dosage);
        Assert.Equal(1, medicationState.NumberMedication);
    }
}
