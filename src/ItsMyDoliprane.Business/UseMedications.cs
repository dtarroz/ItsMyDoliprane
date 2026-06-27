using ItsMyDoliprane.Business.Medications;
using ItsMyDoliprane.Business.Models;
using ItsMyDoliprane.Repository;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Business;

public class UseMedications
{
    private readonly MedicationRepository _medicationRepository;
    private readonly MedicationDoliprane _medicationDoliprane;
    private readonly MedicationHumex _medicationHumex;
    private readonly MedicationAntibiotique _medicationAntibiotique;
    private readonly MedicationSmecta _medicationSmecta;
    private readonly MedicationAllDrug _medicationAllDrug;
    private readonly MedicationIbuprofene _medicationIbuprofene;
    private readonly MedicationTopalgic _medicationTopalgic;
    private readonly MedicationProbiotique _medicationProbiotique;
    private readonly MedicationImmodiumCaps _medicationImmodiumCaps;
    private readonly MedicationProvames _medicationProvames;
    private readonly MedicationAspegicNourrissons _medicationAspegicNourrissons;
    private readonly MedicationProgestan _medicationProgestan;
    private readonly MedicationGavisconNourrissons _medicationGavisconNourrissons;

    public UseMedications(MedicationRepository medicationRepository, MedicationDoliprane medicationDoliprane,
                          MedicationHumex medicationHumex, MedicationAntibiotique medicationAntibiotique, MedicationSmecta medicationSmecta,
                          MedicationAllDrug medicationAllDrug, MedicationIbuprofene medicationIbuprofene,
                          MedicationTopalgic medicationTopalgic, MedicationProbiotique medicationProbiotique,
                          MedicationImmodiumCaps medicationImmodiumCaps, MedicationProvames medicationProvames,
                          MedicationAspegicNourrissons medicationAspegicNourrissons, MedicationProgestan medicationProgestan,
                          MedicationGavisconNourrissons medicationGavisconNourrissons) {
        _medicationRepository = medicationRepository;
        _medicationDoliprane = medicationDoliprane;
        _medicationHumex = medicationHumex;
        _medicationAntibiotique = medicationAntibiotique;
        _medicationSmecta = medicationSmecta;
        _medicationAllDrug = medicationAllDrug;
        _medicationIbuprofene = medicationIbuprofene;
        _medicationTopalgic = medicationTopalgic;
        _medicationProbiotique = medicationProbiotique;
        _medicationImmodiumCaps = medicationImmodiumCaps;
        _medicationProvames = medicationProvames;
        _medicationAspegicNourrissons = medicationAspegicNourrissons;
        _medicationProgestan = medicationProgestan;
        _medicationGavisconNourrissons = medicationGavisconNourrissons;
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

    public List<MedicationState> GetMedicationsStates(int personId, bool isAdult) {
        List<Medication> medications = GetMedicationsSinceDate(personId, DateTime.Now.AddDays(-1));
        return new List<MedicationState> {
            _medicationAllDrug.GetMedicationState(medications, isAdult),
            _medicationDoliprane.GetMedicationState(medications, isAdult),
            _medicationIbuprofene.GetMedicationState(medications, isAdult),
            _medicationHumex.GetMedicationState(medications, isAdult),
            _medicationAntibiotique.GetMedicationState(medications, isAdult),
            _medicationSmecta.GetMedicationState(medications, isAdult),
            _medicationTopalgic.GetMedicationState(medications, isAdult),
            _medicationProbiotique.GetMedicationState(medications, isAdult),
            _medicationImmodiumCaps.GetMedicationState(medications, isAdult),
            _medicationProvames.GetMedicationState(medications, isAdult),
            _medicationAspegicNourrissons.GetMedicationState(medications, isAdult),
            _medicationProgestan.GetMedicationState(medications, isAdult),
            _medicationGavisconNourrissons.GetMedicationState(medications, isAdult)
        };
    }

    public void Delete(int medicationId) {
        _medicationRepository.Delete(medicationId);
    }
}
