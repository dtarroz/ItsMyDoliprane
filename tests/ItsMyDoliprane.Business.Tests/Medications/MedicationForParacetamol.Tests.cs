using System;
using System.Collections.Generic;
using ItsMyDoliprane.Business.Enums;
using Xunit;
using ItsMyDoliprane.Business.Medications;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business.Tests.Medications;

public class MedicationForParacetamol_Tests
{
    [Fact]
    public void GetMedicationState_Empty() {
        MedicationState medicationState = MedicationForParacetamol.GetMedicationState(new List<Medication>());

        Assert.NotNull(medicationState);
        Assert.Equal(1, medicationState.CompositionId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationYes);
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
        MedicationState medicationState = MedicationForParacetamol.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(1, medicationState.CompositionId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationYes);
    }

    [Fact]
    public void GetMedicationState_LessThan4Hours_Dosage2000() {
        DateTime dateTime = DateTime.Now.AddHours(-4).AddMinutes(1);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        MedicationState medicationState = MedicationForParacetamol.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(1, medicationState.CompositionId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime, medicationState.LastMedicationNo);
        Assert.Equal(dateTime.AddHours(6), medicationState.NextMedicationYes);
    }

    [Fact]
    public void GetMedicationState_LessThan4Hours_Dosage3000() {
        DateTime dateTime4 = DateTime.Now.AddHours(-4).AddMinutes(1);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime4,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        MedicationState medicationState = MedicationForParacetamol.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(1, medicationState.CompositionId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime4, medicationState.LastMedicationNo);
        Assert.Equal(dateTime7.AddDays(1), medicationState.NextMedicationYes);
    }

    [Fact]
    public void GetMedicationState_LessThan4Hours_Dosage4000() {
        DateTime dateTime4 = DateTime.Now.AddHours(-4).AddMinutes(1);
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime4,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        MedicationState medicationState = MedicationForParacetamol.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(1, medicationState.CompositionId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime4, medicationState.LastMedicationNo);
        Assert.Equal(dateTime7.AddDays(1), medicationState.NextMedicationYes);
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
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        MedicationState medicationState = MedicationForParacetamol.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(1, medicationState.CompositionId);
        Assert.Equal(MedicationOpinion.Possible, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime5.AddHours(6), medicationState.NextMedicationYes);
    }

    [Fact]
    public void GetMedicationState_Between4And6Hours_Dosage3000() {
        DateTime dateTime5 = DateTime.Now.AddHours(-5);
        DateTime dateTime7 = DateTime.Now.AddHours(-7);
        DateTime dateTime9 = DateTime.Now.AddHours(-9);
        List<Medication> medications = new List<Medication> {
            new() {
                DrugId = 1,
                DateTime = dateTime5,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        MedicationState medicationState = MedicationForParacetamol.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(1, medicationState.CompositionId);
        Assert.Equal(MedicationOpinion.Possible, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
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
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime7,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime9,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        MedicationState medicationState = MedicationForParacetamol.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(1, medicationState.CompositionId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime5, medicationState.LastMedicationNo);
        Assert.Equal(dateTime9.AddDays(1), medicationState.NextMedicationYes);
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
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        MedicationState medicationState = MedicationForParacetamol.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(1, medicationState.CompositionId);
        Assert.Equal(MedicationOpinion.Yes, medicationState.Opinion);
        Assert.Null(medicationState.LastMedicationNo);
        Assert.Null(medicationState.NextMedicationYes);
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
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime14,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        MedicationState medicationState = MedicationForParacetamol.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(1, medicationState.CompositionId);
        Assert.Equal(MedicationOpinion.Possible, medicationState.Opinion);
        Assert.Equal(dateTime9, medicationState.LastMedicationNo);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationYes);
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
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime12,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime14,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            },
            new() {
                DrugId = 1,
                DateTime = dateTime18,
                Dosages = new List<MedicationDosage> {
                    new() {
                        DrugCompositionId = (int)DrugComposition.Paracetamol,
                        Quantity = 1000
                    }
                }
            }
        };
        MedicationState medicationState = MedicationForParacetamol.GetMedicationState(medications);

        Assert.NotNull(medicationState);
        Assert.Equal(1, medicationState.CompositionId);
        Assert.Equal(MedicationOpinion.No, medicationState.Opinion);
        Assert.Equal(dateTime9, medicationState.LastMedicationNo);
        Assert.Equal(dateTime14.AddDays(1), medicationState.NextMedicationYes);
    }
}
