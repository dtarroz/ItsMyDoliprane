namespace ItsMyDoliprane.Business.Models;

internal class RuleMedicationState
{
    public MedicationOpinion? Opinion { get; init; }
    public DateTime? LastMedicationNo { get; init; }
    public DateTime? NextMedicationPossible { get; init; }
    public DateTime? NextMedicationYes { get; init; }
}
