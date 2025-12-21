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
    private readonly UseMedications _useMedications;
    private readonly List<Drug> _drugs;
    private readonly List<Person> _persons;

    public HomeController(UsePersons usePersons, UseDrugs useDrugs, UseMedications useMedications) {
        _useMedications = useMedications;
        _drugs = useDrugs.GetDrugs();
        _persons = usePersons.GetPersons();
    }

    public IActionResult Index(int personId = 1) {
        if (!IsPersonExists(personId))
            return RedirectToAction("Index");
        bool isAdult = PersonIsAdult(personId);
        List<Medication> medications = _useMedications.GetMedicationsSinceDate(personId, DateTime.Now.AddMonths(-1));
        var states = _useMedications.GetMedicationsStates(personId, isAdult).Where(s => IsDrugAllowForPerson(s.DrugId, personId)).ToList();
        HomeViewModel model = new HomeViewModel {
            PersonId = personId,
            Persons = _persons.ToDictionary(p => p.Id, p => p.Name),
            Drugs = GetListDrugs(personId),
            TimeProgressBars = GetTimeProgressBars(states, isAdult),
            Medications = GetMedications(medications)
        };
        return View(model);
    }

    private bool IsPersonExists(int personId) {
        return _persons.FirstOrDefault(p => p.Id == personId) != null;
    }

    private Dictionary<int, string> GetListDrugs(int personId) {
        return _drugs.Where(drug => drug.Visible && IsDrugAllowForPerson(drug, personId)).ToDictionary(p => p.Id, p => p.Name);
    }

    private bool IsDrugAllowForPerson(DrugId? drugId, int personId) {
        if (drugId == null)
            return true;
        Drug drug = _drugs.Single(p => p.Id == (int)drugId);
        return IsDrugAllowForPerson(drug, personId);
    }

    private bool IsDrugAllowForPerson(Drug drug, int personId) {
        switch (PersonIsAdult(personId)) {
            case true when drug.ForAdult:
            case false when drug.ForChild: return true;
            default: return false;
        }
    }

    private bool PersonIsAdult(int personId) {
        Person person = _persons.Single(p => p.Id == personId);
        return person.IsAdult;
    }

    private static List<TimeProgressBar> GetTimeProgressBars(List<MedicationState> states, bool isAdult) {
        var timeProgressBar = new List<TimeProgressBar> {
            GetProgressBarAllDrug(states),
            GetProgressBarDoliprane(states, isAdult),
            GetProgressBarIbuprofene(states),
            GetProgressBarHumex(states),
            GetProgressBarAntibiotique(states),
            GetProgressBarProbiotique(states),
            GetProgressBarTopalgic(states),
            GetProgressBarSmecta(states),
            GetProgressBarImmodiumCaps(states)
        };
        FixTimeProgressBar(timeProgressBar);
        return timeProgressBar;
    }

    private static void FixTimeProgressBar(List<TimeProgressBar> timeProgressBars) {
        foreach (var timeProgressBar in timeProgressBars) {
            if (timeProgressBar is { NumberMedication: 0, MaxValue: >= 10 })
                timeProgressBar.MaxValue = 0;
        }
        int maxValue = timeProgressBars.Select(s => s.MaxValue).Max();
        timeProgressBars.ForEach(s => s.MaxWidthValue = Math.Max(6, maxValue));
    }

    private static TimeProgressBar GetProgressBarAllDrug(List<MedicationState> states) {
        MedicationState? medicationState = states.Find(s => s.DrugId == null);
        return new TimeProgressBar {
            Visible = medicationState?.Opinion == MedicationOpinion.No,
            Caption = "⚠️ Tous les médicaments",
            Tooltip = GetToolTipAllDrug(medicationState),
            Opinion = medicationState?.Opinion.ToString().ToLower() ?? MedicationOpinion.Yes.ToString().ToLower(),
            CurrentValue = GetDuration(medicationState?.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(medicationState?.LastMedicationNo, medicationState?.NextMedicationYes)),
            NumberMedication = medicationState?.NumberMedication ?? 0
        };
    }

    private static string GetToolTipAllDrug(MedicationState? medicationState) {
        string tooltip = "";
        string? nextMedicationYes = medicationState?.NextMedicationYes?.ToString("HH:mm");
        if (medicationState?.NextMedicationYes != null)
            tooltip = $"Prise conseillée à partir de {nextMedicationYes}";
        return tooltip;
    }

    private static TimeProgressBar GetProgressBarDoliprane(List<MedicationState> states, bool isAdult) {
        MedicationState? state = states.Find(s => s.DrugId == DrugId.Doliprane);
        return new TimeProgressBar {
            Visible = GetMedicationStateVisible(state, states),
            Caption = "Doliprane",
            Tooltip = GetToolTipParacetamol(state, isAdult),
            Opinion = state?.Opinion.ToString().ToLower() ?? MedicationOpinion.Yes.ToString().ToLower(),
            CurrentValue = GetDuration(state?.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(state?.LastMedicationNo, state?.NextMedicationYes)),
            NumberMedication = state?.NumberMedication ?? 0
        };
    }

    private static string GetToolTipParacetamol(MedicationState? state, bool isAdult) {
        string tooltip = "";
        string? nextMedicationYes = state?.NextMedicationYes?.ToString("HH:mm");
        string? nextMedicationPossible = state?.NextMedicationPossible?.ToString("HH:mm");
        int dosage = state?.Dosages.FirstOrDefault(d => d.DrugCompositionId == DrugCompositionId.Paracetamol)?.TotalQuantity ?? 0;
        if (state?.NextMedicationYes != null)
            tooltip = $"Prise conseillée à partir de {nextMedicationYes}";
        if (state?.NextMedicationPossible != null && nextMedicationYes != nextMedicationPossible)
            tooltip += $"<br/>mais possible à partir de {nextMedicationPossible}";
        if (dosage > 0 && isAdult)
            tooltip += $"{(tooltip != "" ? "<br/><br/>" : "")}{dosage / 1000.0}g de paracétamol en 24h";
        if ((state?.NumberMedication ?? 0) > 0 && !isAdult)
            tooltip +=
                $"{(tooltip != "" ? "<br/><br/>" : "")}{state!.NumberMedication} prise{(state.NumberMedication > 1 ? "s" : "")} de doliprane en 24h";
        return tooltip;
    }

    private static TimeProgressBar GetProgressBarIbuprofene(List<MedicationState> states) {
        MedicationState? state = states.Find(s => s.DrugId == DrugId.Ibuprofene);
        return new TimeProgressBar {
            Visible = GetMedicationStateVisible(state, states),
            Caption = "Ibuprofène",
            Tooltip = GetToolTipIbuprofene(state),
            Opinion = state?.Opinion.ToString().ToLower() ?? MedicationOpinion.Yes.ToString().ToLower(),
            CurrentValue = GetDuration(state?.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(state?.LastMedicationNo, state?.NextMedicationYes)),
            NumberMedication = state?.NumberMedication ?? 0
        };
    }

    private static string GetToolTipIbuprofene(MedicationState? state) {
        string tooltip = "";
        string? nextMedicationYes = state?.NextMedicationYes?.ToString("HH:mm");
        if (state?.NextMedicationYes != null)
            tooltip = $"Prise possible à partir de {nextMedicationYes}";
        if ((state?.NumberMedication ?? 0) > 0)
            tooltip +=
                $"{(tooltip != "" ? "<br/><br/>" : "")}{state!.NumberMedication} prise{(state.NumberMedication > 1 ? "s" : "")} d'ibuprofène en 24h";
        return tooltip;
    }

    private static double GetDuration(DateTime? dateTime1, DateTime? dateTime2) {
        if (dateTime1 == null || dateTime2 == null)
            return 0;
        return (dateTime2.Value - dateTime1.Value).TotalHours;
    }

    private static TimeProgressBar GetProgressBarHumex(List<MedicationState> states) {
        MedicationState? state = states.Find(s => s.DrugId == DrugId.Humex);
        return new TimeProgressBar {
            Visible = GetMedicationStateVisible(state, states),
            Caption = state?.NextDrug switch {
                DrugId.HumexJour => "Humex jour",
                DrugId.HumexNuit => "Humex nuit",
                _                => "Humex"
            },
            Tooltip = GetToolTipParacetamol(state, true),
            Opinion = state?.Opinion.ToString().ToLower() ?? MedicationOpinion.Yes.ToString().ToLower(),
            CurrentValue = GetDuration(state?.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(state?.LastMedicationNo, state?.NextMedicationYes)),
            NumberMedication = state?.NumberMedication ?? 0
        };
    }

    private static TimeProgressBar GetProgressBarAntibiotique(List<MedicationState> states) {
        MedicationState? state = states.Find(s => s.DrugId == DrugId.Antibiotique);
        return new TimeProgressBar {
            Visible = GetMedicationStateVisible(state, states),
            Caption = "Antibiotique",
            Tooltip = GetToolTipAntibiotique(state),
            Opinion = state?.Opinion.ToString().ToLower() ?? MedicationOpinion.Yes.ToString().ToLower(),
            CurrentValue = GetDuration(state?.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(state?.LastMedicationNo, state?.NextMedicationYes)),
            NumberMedication = state?.NumberMedication ?? 0
        };
    }

    private static string GetToolTipAntibiotique(MedicationState? state) {
        string tooltip = "";
        string? nextMedicationYes = state?.NextMedicationYes?.ToString("HH:mm");
        string? nextMedicationPossible = state?.NextMedicationPossible?.ToString("HH:mm");
        if (state?.NextMedicationYes != null)
            tooltip = $"Prise conseillée à partir de {nextMedicationYes}";
        if (state?.NextMedicationPossible != null && nextMedicationYes != nextMedicationPossible)
            tooltip += $"<br/>mais possible à partir de {nextMedicationPossible}";
        if ((state?.NumberMedication ?? 0) > 0)
            tooltip +=
                $"{(tooltip != "" ? "<br/><br/>" : "")}{state!.NumberMedication} prise{(state.NumberMedication > 1 ? "s" : "")} d'antibiotique en 24h";
        return tooltip;
    }

    private static TimeProgressBar GetProgressBarProbiotique(List<MedicationState> states) {
        MedicationState? state = states.Find(s => s.DrugId == DrugId.Probiotique);
        return new TimeProgressBar {
            Visible = GetMedicationStateVisible(state, states),
            Caption = "Probiotique",
            Tooltip = GetToolTipProbiotique(state),
            Opinion = state?.Opinion.ToString().ToLower() ?? MedicationOpinion.Yes.ToString().ToLower(),
            CurrentValue = GetDuration(state?.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(state?.LastMedicationNo, state?.NextMedicationYes)),
            NumberMedication = state?.NumberMedication ?? 0
        };
    }

    private static string GetToolTipProbiotique(MedicationState? state) {
        string tooltip = "";
        string? nextMedicationYes = state?.NextMedicationYes?.ToString("HH:mm");
        string? nextMedicationPossible = state?.NextMedicationPossible?.ToString("HH:mm");
        if (state?.NextMedicationYes != null)
            tooltip = $"Prise conseillée à partir de {nextMedicationYes}";
        if (state?.NextMedicationPossible != null && nextMedicationYes != nextMedicationPossible)
            tooltip += $"<br/>mais possible à partir de {nextMedicationPossible}";
        if ((state?.NumberMedication ?? 0) > 0)
            tooltip +=
                $"{(tooltip != "" ? "<br/><br/>" : "")}{state!.NumberMedication} prise{(state.NumberMedication > 1 ? "s" : "")} de probiotique en 24h";
        return tooltip;
    }

    private static TimeProgressBar GetProgressBarSmecta(List<MedicationState> states) {
        MedicationState? state = states.Find(s => s.DrugId == DrugId.Smecta);
        int count = state?.Dosages.FirstOrDefault(d => d.DrugCompositionId == DrugCompositionId.Diosmectite)?.Number ?? 0;
        return new TimeProgressBar {
            Visible = GetMedicationStateVisible(state, states),
            Caption = "Smecta",
            Tooltip = GetToolTipSmecta(state),
            Opinion = state?.Opinion.ToString().ToLower() ?? MedicationOpinion.Yes.ToString().ToLower(),
            CurrentValue = GetDuration(state?.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(state?.LastMedicationNo, state?.NextMedicationYes)),
            NumberMedication = count
        };
    }

    private static string GetToolTipSmecta(MedicationState? state) {
        string tooltip = "";
        int count = state?.Dosages.FirstOrDefault(d => d.DrugCompositionId == DrugCompositionId.Diosmectite)?.Number ?? 0;
        if (state?.NextMedicationYes != null)
            tooltip = $"Prise possible à partir de {state.NextMedicationYes.Value:HH:mm}";
        if (count > 0)
            tooltip += $"{(tooltip != "" ? "<br/><br/>" : "")}{count} prise{(count > 1 ? "s" : "")} de smecta en 24h";
        return tooltip;
    }

    private static TimeProgressBar GetProgressBarTopalgic(List<MedicationState> states) {
        MedicationState? state = states.Find(s => s.DrugId == DrugId.Topalgic);
        return new TimeProgressBar {
            Visible = GetMedicationStateVisible(state, states),
            Caption = "Topalgic",
            Tooltip = GetToolTipTopalgic(state),
            Opinion = state?.Opinion.ToString().ToLower() ?? MedicationOpinion.Yes.ToString().ToLower(),
            CurrentValue = GetDuration(state?.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(state?.LastMedicationNo, state?.NextMedicationYes)),
            NumberMedication = state?.NumberMedication ?? 0
        };
    }

    private static string GetToolTipTopalgic(MedicationState? state) {
        string tooltip = "";
        string? nextMedicationYes = state?.NextMedicationYes?.ToString("HH:mm");
        string? nextMedicationPossible = state?.NextMedicationPossible?.ToString("HH:mm");
        if (state?.NextMedicationYes != null)
            tooltip = $"Prise conseillée à partir de {nextMedicationYes}";
        if (state?.NextMedicationPossible != null && nextMedicationYes != nextMedicationPossible)
            tooltip += $"<br/>mais possible à partir de {nextMedicationPossible}";
        if ((state?.NumberMedication ?? 0) > 0)
            tooltip +=
                $"{(tooltip != "" ? "<br/><br/>" : "")}{state!.NumberMedication} prise{(state.NumberMedication > 1 ? "s" : "")} de topalgic en 24h";
        return tooltip;
    }

    private static TimeProgressBar GetProgressBarImmodiumCaps(List<MedicationState> states) {
        MedicationState? state = states.Find(s => s.DrugId == DrugId.ImmodiumCaps);
        return new TimeProgressBar {
            Visible = GetMedicationStateVisible(state, states),
            Caption = "Immodium Caps",
            Tooltip = GetToolTipImmodiumCaps(state),
            Opinion = state?.Opinion.ToString().ToLower() ?? MedicationOpinion.Yes.ToString().ToLower(),
            CurrentValue = GetDuration(state?.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(state?.LastMedicationNo, state?.NextMedicationYes)),
            NumberMedication = state?.NumberMedication ?? 0
        };
    }

    private static string GetToolTipImmodiumCaps(MedicationState? state) {
        string tooltip = "";
        string? nextMedicationYes = state?.NextMedicationYes?.ToString("HH:mm");
        if (state?.NextMedicationYes != null)
            tooltip = $"Prise possible à partir de {nextMedicationYes}";
        if ((state?.NumberMedication ?? 0) > 0)
            tooltip +=
                $"{(tooltip != "" ? "<br/><br/>" : "")}{state!.NumberMedication} prise{(state.NumberMedication > 1 ? "s" : "")} d'immodium caps en 24h";
        return tooltip;
    }

    private List<MedicationViewModel> GetMedications(IEnumerable<Medication> medications) {
        return medications.GroupBy(m => m.DateTime.Date)
                          .Select(group => new MedicationViewModel {
                                      Date = group.Key.ToString("dd/MM"),
                                      Details = group.Select(d => new MedicationDetailViewModel {
                                                                 Id = d.Id,
                                                                 Drug = _drugs.Find(dr => dr.Id == d.DrugId)?.Name
                                                                        ?? "",
                                                                 Hour = d.DateTime.ToString("HH:mm")
                                                             })
                                                     .ToList()
                                  })
                          .ToList();
    }

    private static bool GetMedicationStateVisible(MedicationState? state, List<MedicationState> states) {
        MedicationState allState = states.Single(s => s.DrugId == null);
        if ((state?.NumberMedication ?? 0) > 0)
            return true;
        if ((state?.Opinion ?? MedicationOpinion.Yes) == MedicationOpinion.Yes)
            return false;
        if (allState.Opinion == MedicationOpinion.Yes)
            return true;
        return (state!.NextMedicationYes!.Value - allState.NextMedicationYes!.Value).TotalMinutes != 0;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
