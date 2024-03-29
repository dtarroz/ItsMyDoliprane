namespace ItsMyDoliprane.Models;

public class HomeViewModel
{
    public int PersonId { get; set; }
    public Dictionary<int, string> Persons { get; set; }
    public Dictionary<int, string> Drugs { get; set; }
    public float DosageParacetamol { get; set; }
    public MedicationTime Medication4 { get; set; }
    public MedicationTime Medication6 { get; set; }
    public List<MedicationViewModel> Medications { get; set; }
}

public class MedicationTime
{
    public bool Ok { get; set; }
    public string? NextHour { get; set; }
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
