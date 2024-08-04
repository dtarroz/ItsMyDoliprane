namespace ItsMyDoliprane.Repository.Models;

public class Medication
{
    public int DrugId { get; init; }
    public DateTime DateTime { get; set; }
    public List<MedicationDosage> Dosages { get; set; } = null!;
}

public class MedicationDosage
{
    public int DrugCompositionId { get; set; }
    public int Quantity { get; set; }
}
