namespace ItsMyDoliprane.Models;

public class HomeViewModel
{
    public int PersonId { get; set; }
    public Dictionary<int, string> Persons { get; set; }
    public Dictionary<int, string> Drugs { get; set; }
    public string? Date { get; set; }
    public string? Hour { get; set; }
    public List<MedicationViewModel> Medications { get; set; }
}

public class MedicationViewModel
{
    public string Date { get; set; }
    public List<MedicationDetailViewModel> Details { get; set; }
}

public class MedicationDetailViewModel
{
    public string Hour { get; set; }
    public string Drug { get; set; }
}
