namespace ItsMyDoliprane.Models;

public class SettingsViewModel
{
    public List<DrugSettingViewModel> Drugs { get; init; } = new();
}

public class DrugSettingViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public bool ForAdult { get; init; }
    public bool ForChild { get; init; }
}

