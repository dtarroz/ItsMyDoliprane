using ItsMyDoliprane.Repository;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business;

public class UseMedications
{
    private readonly MedicationRepository _medicationRepository;

    public UseMedications(MedicationRepository medicationRepository) {
        _medicationRepository = medicationRepository;
    }

    public List<Medication> GetMedicationsSinceDate(int personId, DateTime date) {
        return _medicationRepository.GetMedicationsSinceDate(personId, date);
    }

    public void Add(Medication medication) {
        if (string.IsNullOrEmpty(medication.Date))
            medication.Date = DateTime.Now.ToString("yyyy-MM-dd");
        if (string.IsNullOrEmpty(medication.Hour))
            medication.Hour = DateTime.Now.ToString("HH:mm");
        _medicationRepository.Add(medication);
    }
}
