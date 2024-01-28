using ItsMyDoliprane.Repository;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business;

public class UseDrugs
{
    private readonly DrugRepository _drugRepository;

    public UseDrugs(DrugRepository drugRepository) {
        _drugRepository = drugRepository;
    }

    public List<Drug> GetDrugs() {
        return _drugRepository.GetGrugs();
    }
}
