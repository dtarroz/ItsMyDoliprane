using ItsMyDoliprane.Business.Medications;
using ItsMyDoliprane.Business.Models;
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

    public void Add(NewMedication newMedication) {
        if (string.IsNullOrEmpty(newMedication.Date))
            newMedication.Date = DateTime.Now.ToString("yyyy-MM-dd");
        if (string.IsNullOrEmpty(newMedication.Hour))
            newMedication.Hour = DateTime.Now.ToString("HH:mm");
        _medicationRepository.Add(newMedication);
    }

    public List<MedicationState> GetMedicationsStates(int personId) {
        List<Medication> medications = GetMedicationsSinceDate(personId, DateTime.Now.AddDays(-1));
        return new List<MedicationState> {
            MedicationDoliprane.GetMedicationState(medications)
        };
    }
}
