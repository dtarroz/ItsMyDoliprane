namespace ItsMyDoliprane.Repository.Models;

public class Drug
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public bool Visible { get; init; }
    public bool ForAdult { get; init; }
    public bool ForChild { get; init; }
}
