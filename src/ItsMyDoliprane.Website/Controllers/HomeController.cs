using System.Diagnostics;
using ItsMyDoliprane.Business;
using Microsoft.AspNetCore.Mvc;
using ItsMyDoliprane.Models;
using ItsMyDoliprane.Repository.Models;

namespace ItsMyDoliprane.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UsePersons _usePersons;
    private readonly UseDrugs _useDrugs;
    private readonly UseMedications _useMedications;

    public HomeController(ILogger<HomeController> logger, UsePersons usePersons, UseDrugs useDrugs, UseMedications useMedications) {
        _logger = logger;
        _usePersons = usePersons;
        _useDrugs = useDrugs;
        _useMedications = useMedications;
    }

    public IActionResult Index(int personId = 1) {
        HomeViewModel model = new HomeViewModel {
            PersonId = personId,
            Persons = _usePersons.GetPersons().ToDictionary(p => p.Id, p => p.Name),
            Drugs = _useDrugs.GetDrugs().ToDictionary(p => p.Id, p => p.Name),
            Date = DateTime.Now.ToString("yyyy-MM-dd"),
            Hour = DateTime.Now.ToString("HH:mm"),
            Medications = GetMedications(personId)
        };
        return View(model);
    }

    private List<MedicationViewModel> GetMedications(int personId) {
        List<Medication> medications = _useMedications.GetMedicationsSinceDate(personId, DateTime.Now.AddDays(-2));
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
