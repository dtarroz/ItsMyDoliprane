namespace ItsMyDoliprane.Repository.Models;

public class Medication
{
    public int PersonId { get; init; }
    public int DrugId { get; init; }
    public string Date { get; set; } = null!;
    public string Hour { get; set; } = null!;
}
