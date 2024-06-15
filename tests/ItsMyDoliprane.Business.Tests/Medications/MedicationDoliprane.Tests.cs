using System;
using System.Collections.Generic;
using ItsMyDoliprane.Business.Enums;
using Xunit;
using ItsMyDoliprane.Business.Medications;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business.Tests.Medications;

public class MedicationDoliprane_Tests
{
    [Fact]
    public void GetMedicationState_Empty() {
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(new List<Medication>());

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Equal(0, medicationState.Dosage);
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Equal(0, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_LessThan4Hours_Dosage2000() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime3, medicationState.LastMedicationNo);
        Assert.Equal(dateTime3.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime3.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(2000, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_LessThan4Hours_Dosage2500_Last_LessThan4Hours() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(dateTime2.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime2.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(2500, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_LessThan4Hours_Dosage2500_Last_Between4And6Hours() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime2.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(2500, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_LessThan4Hours_Dosage2500_Last_LaterThan6Hours() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(2500, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_LessThan4Hours_Dosage3000_Last_LessThan4Hours() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(dateTime2.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime2.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(3000, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_LessThan4Hours_Dosage3000_Last_Between4And6Hours() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime2.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(3000, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_LessThan4Hours_Dosage3000_Last_LaterThan6Hours() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(3000, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_LessThan4Hours_Dosage4000_Last_LessThan4Hours() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(dateTime2.AddHours(4), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime2.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(4000, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_LessThan4Hours_Dosage4000_Last_Between4And6Hours() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeBeforeLast.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime2.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(4000, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_LessThan4Hours_Dosage4000_Last_LaterThan6Hours() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime2, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeBeforeLast.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeBeforeLast.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(4000, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_Between4And6Hours_Dosage2000() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Possible, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Equal(dateTime5.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(2000, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_Between4And6Hours_Dosage2500_Last_Between4And6Hours() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime5.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(2500, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_Between4And6Hours_Dosage2500_Last_LaterThan6Hours() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(2500, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_Between4And6Hours_Dosage3000_Last_Between4And6Hours() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime5.AddHours(6), medicationState.NextMedicationYes);
        Assert.Equal(3000, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_Between4And6Hours_Dosage3000_Last_LaterThan6Hours() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTimeLast.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(3000, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_Between4And6Hours_Dosage4000() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(4000, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_LaterThan6Hours_Dosage2000() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationPossible);
        Assert.Null(medicationState.NextMedicationYes);
        Assert.Equal(2000, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_LaterThan6Hours_Dosage2500() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTime9, medicationState.LastMedicationNo);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(2500, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_LaterThan6Hours_Dosage3000() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.Warning, medicationState.Opinion);
        Assert.Equal(dateTime9, medicationState.LastMedicationNo);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(3000, medicationState.Dosage);
    }

    [Fact]
    public void GetMedicationState_LaterThan6Hours_Dosage4000() {
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
        MedicationDoliprane medicationDoliprane = new MedicationDoliprane();
        MedicationState medicationState = medicationDoliprane.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(DrugId.Doliprane, medicationState.DrugId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime9, medicationState.LastMedicationNo);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationPossible);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationYes);
        Assert.Equal(4000, medicationState.Dosage);
    }
}
