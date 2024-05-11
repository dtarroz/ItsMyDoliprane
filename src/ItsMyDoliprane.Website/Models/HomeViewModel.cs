namespace ItsMyDoliprane.Models;

public class HomeViewModel
{
    public int PersonId { get; set; }
    public Dictionary<int, string> Persons { get; set; } = null!;
    public Dictionary<int, string> Drugs { get; set; } = null!;
    public float DosageParacetamol { get; set; }
    public MedicationTime Medication4 { get; set; } = null!;
    public MedicationTime Medication6 { get; set; } = null!;
    public List<MedicationViewModel> Medications { get; set; } = null!;
}

public class MedicationTime
{
    public bool Ok { get; init; }
    public string? NextHour { get; init; }
}

public class MedicationViewModel
{
    public string Date { get; init; } = null!;
    public List<MedicationDetailViewModel> Details { get; init; } = null!;
}

public class MedicationDetailViewModel
{
    public string Hour { get; init; } = null!;
    public string Drug { get; init; } = null!;
}
