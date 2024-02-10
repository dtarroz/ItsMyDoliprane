using ItsMyDoliprane.Repository;

namespace ItsMyDoliprane.Business;

public class UseDosages
{
    private readonly DosageRepository _dosageRepository;
    
    public UseDosages(DosageRepository dosageRepository) {
        _dosageRepository = dosageRepository;
    }

    public int GetDosageSinceDate(int personId, int compositionId, DateTime date) {
        return _dosageRepository.GetDosageSinceDate(personId, compositionId, date);
    }
}
