using System.Diagnostics;
using ItsMyDoliprane.Business;
using Microsoft.AspNetCore.Mvc;
using ItsMyDoliprane.Models;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Controllers;

public class HomeController : Controller
{
    private readonly UsePersons _usePersons;
    private readonly UseDrugs _useDrugs;
    private readonly UseMedications _useMedications;
    private readonly UseDosages _useDosages;

    public HomeController(UsePersons usePersons, UseDrugs useDrugs, UseMedications useMedications, UseDosages useDosages) {
        _usePersons = usePersons;
        _useDrugs = useDrugs;
        _useMedications = useMedications;
        _useDosages = useDosages;
    }

    public IActionResult Index(int personId = 1) {
        List<Medication> medications = _useMedications.GetMedicationsSinceDate(personId, DateTime.Now.AddDays(-2));
        HomeViewModel model = new HomeViewModel {
            PersonId = personId,
            Persons = _usePersons.GetPersons().ToDictionary(p => p.Id, p => p.Name),
            Drugs = _useDrugs.GetDrugs().ToDictionary(p => p.Id, p => p.Name),
            DosageParacetamol = _useDosages.GetDosageSinceDate(personId, 1, DateTime.Now.AddDays(-1)) / 1000f,
            Medication4 = GetMedication(medications, 4),
            Medication6 = GetMedication(medications, 6),
            Medications = GetMedications(medications)
        };
        return View(model);
    }

    private MedicationTime GetMedication(List<Medication> medications, int limitHour) {
        Medication? last = medications.FirstOrDefault();
        DateTime? lastDateTime = last != null ? GetDateTime(last) : null;
        return new MedicationTime {
            Ok = lastDateTime == null || (DateTime.Now - lastDateTime.Value).TotalHours > limitHour,
            NextHour = lastDateTime?.AddHours(limitHour).ToString("HH:mm")
        };
    }

    private DateTime GetDateTime(Medication medication) {
        return DateTime.Parse($"{medication.Date} {medication.Hour}:00");
    }

    private List<MedicationViewModel> GetMedications(List<Medication> medications) {
        List<Drug> drugs = _useDrugs.GetDrugs();
        return medications.GroupBy(m => m.Date)
                          .Select(group => new MedicationViewModel {
                                      Date = DateTime.Parse(group.Key).ToString("dd/MM"),
                                      Details = group.Select(d => new MedicationDetailViewModel {
                                                         Drug = drugs.Find(dr => dr.Id == d.DrugId)?.Name ?? "",
                                                         Hour = d.Hour
                                                     })
                                                     .ToList()
                                  })
                          .ToList();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
