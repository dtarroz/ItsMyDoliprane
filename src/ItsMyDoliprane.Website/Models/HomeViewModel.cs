namespace ItsMyDoliprane.Models;

public class HomeViewModel
{
    public int PersonId { get; set; }
    public Dictionary<int, string> Persons { get; set; } = null!;
    public Dictionary<int, string> Drugs { get; set; } = null!;
    public float DosageParacetamol { get; set; }    // TODO
    public MedicationTime Medication4 { get; set; } = null!; // TODO
    public MedicationTime Medication6 { get; set; } = null!;    // TODO
    public TimeProgressBar ProgressBarDoliprane { get; set; } = null!;
    public TimeProgressBar ProgressBarHumex { get; set; } = null!;
    public List<MedicationViewModel> Medications { get; set; } = null!;
}

public class MedicationTime // TODO
{
    public bool Ok { get; init; }
    public string? NextHour { get; init; }
}

public class TimeProgressBar
{
    public bool Visible { get; set; }
    public string Caption { get; set; } = null!;
    public string Tooltip { get; set; } = null!;
    public string Opinion { get; set; } = null!;
    public double CurrentValue { get; set; }
    public int MaxValue { get; set; }
    public int MaxWidthValue { get; set; }
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
