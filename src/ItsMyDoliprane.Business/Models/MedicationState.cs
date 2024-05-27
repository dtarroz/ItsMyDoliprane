namespace ItsMyDoliprane.Business.Models;

public class MedicationState
{
    public int DrugCompositionId { get; init; }
    public MedicationOpinion Opinion { get; init; }
    public DateTime? LastMedicationNo { get; init; }
    public DateTime? NextMedicationYes { get; init; }
}

public enum MedicationOpinion
{
    Yes,
    Possible,
    No
}