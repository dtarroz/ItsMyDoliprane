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
    public int Dosage { get; init; }
    public int NumberMedication { get; init; }
}

public enum MedicationOpinion
{
    Yes,
    Possible,
    Warning,
    No
}
