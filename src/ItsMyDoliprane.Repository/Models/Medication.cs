namespace ItsMyDoliprane.Repository.Models;

public class Medication
{
    public int DrugId { get; init; }
    public string Date { get; set; } = null!; // TODO
    public string Hour { get; set; } = null!; // TODO
    public DateTime DateTime { get; set; }
    public List<MedicationDosage> Dosages { get; set; }
}

public class MedicationDosage
{
    public int DrugCompositionId { get; set; }
    public int Quantity { get; set; }
}
