using ItsMyDoliprane.Business.Enums;

namespace ItsMyDoliprane.Business.Models;

public class MedicationState
{
    public DrugId? DrugId { get; init; }
    public MedicationOpinion Opinion { get; init; }
    public DateTime? LastMedicationNo { get; init; }
    public DateTime? NextMedicationPossible { get; init; }
    public DateTime? NextMedicationYes { get; init; }
    public DrugId? NextDrug { get; init; }
    public List<MedicationStateDosage> Dosages { get; init; } = null!;
    public int NumberMedication { get; init; }
}

public enum MedicationOpinion
{
    Yes,
    Possible,
    Warning,
    No
}

public class MedicationStateDosage
{
    public DrugCompositionId DrugCompositionId { get; set; }
    public int TotalQuantity { get; set; }
    public int Number { get; set; }
}
