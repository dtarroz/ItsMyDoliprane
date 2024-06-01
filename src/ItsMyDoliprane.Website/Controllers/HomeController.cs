using System.Diagnostics;
using ItsMyDoliprane.Business;
using ItsMyDoliprane.Business.Enums;
using ItsMyDoliprane.Business.Models;
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
        List<MedicationState> states = _useMedications.GetMedicationsStates(personId);
        HomeViewModel model = new HomeViewModel {
            PersonId = personId,
            Persons = _usePersons.GetPersons().ToDictionary(p => p.Id, p => p.Name),
            Drugs = _useDrugs.GetDrugs().Where(d => d.Visible).ToDictionary(p => p.Id, p => p.Name),
            DosageParacetamol = _useDosages.GetDosageSinceDate(personId, 1, DateTime.Now.AddDays(-1)) / 1000f,
            Medication4 = GetMedication(medications, 4),
            Medication6 = GetMedication(medications, 6),
            ProgressBarDoliprane = GetProgressBarDoliprane(states),
            Medications = GetMedications(medications)
        };
        return View(model);
    }

    private static MedicationTime GetMedication(IEnumerable<Medication> medications, int limitHour) {
        Medication? last = medications.FirstOrDefault();
        DateTime? lastDateTime = last != null ? GetDateTime(last) : null;
        return new MedicationTime {
            Ok = lastDateTime == null || (DateTime.Now - lastDateTime.Value).TotalHours > limitHour,
            NextHour = lastDateTime?.AddHours(limitHour).ToString("HH:mm")
        };
    }

    private static DateTime GetDateTime(Medication medication) {
        return DateTime.Parse($"{medication.Date} {medication.Hour}:00");
    }

    private static TimeProgressBar GetProgressBarDoliprane(List<MedicationState> states) {
        MedicationState paracetamolState = states.Find(s => s.DrugId == DrugId.Doliprane)!;
        DateTime? maxDateTime = states.Max(s => s.NextMedicationYes);
        return new TimeProgressBar {
            Caption = "Doliprane",
            Tooltip = GetToolTip(paracetamolState),
            Opinion = paracetamolState.Opinion.ToString().ToLower(),
            CurrentValue = GetDuration(paracetamolState.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(paracetamolState.LastMedicationNo, paracetamolState.NextMedicationYes)),
            MaxWidthValue = Math.Max((int)Math.Ceiling(GetDuration(paracetamolState.LastMedicationNo, maxDateTime)), 6)
        };
    }

    private static string GetToolTip(MedicationState medicationState) {
        string tooltip = "";
        if (medicationState.NextMedicationYes != null)
            tooltip = $"Prise conseillée à partir de {medicationState.NextMedicationYes.Value:HH:mm}";
        if (medicationState.NextMedicationPossible != null)
            tooltip += $"<br/>mais possible à partir de {medicationState.NextMedicationPossible.Value:HH:mm}";
        if (medicationState.Dosage > 0)
            tooltip += $"{(tooltip != "" ? "<br/><br/>" : "")}{medicationState.Dosage / 1000.0}g de paracétamol en 24h";
        return tooltip;
    }

    private static double GetDuration(DateTime? dateTime1, DateTime? dateTime2) {
        if (dateTime1 == null || dateTime2 == null)
            return 0;
        return (dateTime2.Value - dateTime1.Value).TotalHours;
    }

    private List<MedicationViewModel> GetMedications(IEnumerable<Medication> medications) {
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
