using ItsMyDoliprane.Repository.Boundary;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business;

public class UseDrugs
{
    private readonly IDrugRepository _drugRepository;

    public UseDrugs(IDrugRepository drugRepository) {
        _drugRepository = drugRepository;
    }

    public List<Drug> GetDrugs() {
        return _drugRepository.GetDrugs();
    }
}
