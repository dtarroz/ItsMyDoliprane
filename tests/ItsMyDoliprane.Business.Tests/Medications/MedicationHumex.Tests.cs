using System;
using System.Collections.Generic;
using ItsMyDoliprane.Business.Enums;
using ItsMyDoliprane.Business.Medications;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Repository.Models;
using Xunit;

namespace ItsMyDoliprane.Business.Tests.Medications;

public class MedicationHumex_Tests
{
    [Fact]
    public void GetMedicationState_Empty() {
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(new List<Medication>());

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
        // TODO Dosage
    }

    [Fact]
    public void GetMedicationState_EmptyAndOtherDrugComposition() {
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = DateTime.Now.AddHours(-2),
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = 2,
                        Quantity = 6000
                    }
                }
            }
        };
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(nextDrug, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(drugId, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(drugId, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(drugId, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddHours(20), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddHours(20), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddHours(20), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddHours(20), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationYes);
        Assert.Equal(DrugId.HumexNuit, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Equal(DrugId.HumexNuit, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(DrugId.HumexNuit, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime21.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime21.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(DrugId.HumexNuit, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(DrugId.HumexNuit, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime12.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(DrugId.HumexNuit, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime5.AddHours(6), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime5.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(DrugId.HumexJour, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Equal(DrugId.HumexJour, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Equal(DrugId.HumexJour, medicationState.NextDrug);
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
        MedicationHumex medicationHumex = new MedicationHumex();
        MedicationState medicationState = medicationHumex.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Humex, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Null(medicationState.NextDrug);
    }
}
