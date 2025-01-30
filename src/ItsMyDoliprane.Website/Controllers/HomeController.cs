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
        List<Medication> medications = _useMedications.GetMedicationsSinceDate(personId, DateTime.Now.AddMonths(-1));
        var states = _useMedications.GetMedicationsStates(personId, PersonIsAdult(personId))
                                    .Where(s => IsDrugAllowForPerson(s.DrugId, personId))
                                    .ToList();
        HomeViewModel model = new HomeViewModel {
            PersonId = personId,
            Persons = _persons.ToDictionary(p => p.Id, p => p.Name),
            Drugs = GetListDrugs(personId),
            TimeProgressBars = GetTimeProgressBars(states),
            Medications = GetMedications(medications)
        };
        return View(model);
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

    private static List<TimeProgressBar> GetTimeProgressBars(List<MedicationState> states) {
        return new List<TimeProgressBar> {
            GetProgressBarAllDrug(states),
            GetProgressBarDoliprane(states),
            GetProgressBarIbuprofene(states),
            GetProgressBarHumex(states),
            GetProgressBarAntibiotique(states),
            GetProgressBarSmecta(states)
        };
    }

    private static TimeProgressBar GetProgressBarAllDrug(List<MedicationState> states) {
        MedicationState? medicationState = states.Find(s => s.DrugId == null);
        DateTime? maxDateTime = states.Max(s => s.NextMedicationYes);
        return new TimeProgressBar {
            Visible = medicationState?.Opinion == MedicationOpinion.No,
            Caption = "⚠️ Tous les médicaments",
            Tooltip = GetToolTipAllDrug(medicationState),
            Opinion = medicationState?.Opinion.ToString().ToLower() ?? MedicationOpinion.Yes.ToString().ToLower(),
            CurrentValue = GetDuration(medicationState?.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(medicationState?.LastMedicationNo, medicationState?.NextMedicationYes)),
            MaxWidthValue = Math.Max((int)Math.Ceiling(GetDuration(medicationState?.LastMedicationNo, maxDateTime)), 6),
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

    private static TimeProgressBar GetProgressBarDoliprane(List<MedicationState> states) {
        MedicationState? dolipraneState = states.Find(s => s.DrugId == DrugId.Doliprane);
        MedicationState? ibuprofeneState = states.Find(s => s.DrugId == DrugId.Ibuprofene);
        DateTime? maxDateTime = states.Max(s => s.NextMedicationYes);
        return new TimeProgressBar {
            Visible = dolipraneState?.Dosage > 0 || (ibuprofeneState?.Dosage > 0 && dolipraneState?.Opinion != MedicationOpinion.Yes),
            Caption = "Doliprane",
            Tooltip = GetToolTipParacetamol(dolipraneState),
            Opinion = dolipraneState?.Opinion.ToString().ToLower() ?? MedicationOpinion.Yes.ToString().ToLower(),
            CurrentValue = GetDuration(dolipraneState?.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(dolipraneState?.LastMedicationNo, dolipraneState?.NextMedicationYes)),
            MaxWidthValue = Math.Max((int)Math.Ceiling(GetDuration(dolipraneState?.LastMedicationNo, maxDateTime)), 6),
            NumberMedication = dolipraneState?.NumberMedication ?? 0
        };
    }

    private static string GetToolTipParacetamol(MedicationState? medicationState) {
        string tooltip = "";
        string? nextMedicationYes = medicationState?.NextMedicationYes?.ToString("HH:mm");
        string? nextMedicationPossible = medicationState?.NextMedicationPossible?.ToString("HH:mm");
        if (medicationState?.NextMedicationYes != null)
            tooltip = $"Prise conseillée à partir de {nextMedicationYes}";
        if (medicationState?.NextMedicationPossible != null && nextMedicationYes != nextMedicationPossible)
            tooltip += $"<br/>mais possible à partir de {nextMedicationPossible}";
        if (medicationState?.Dosage > 0)
            tooltip += $"{(tooltip != "" ? "<br/><br/>" : "")}{medicationState.Dosage / 1000.0}g de paracétamol en 24h";
        return tooltip;
    }

    private static TimeProgressBar GetProgressBarIbuprofene(List<MedicationState> states) {
        MedicationState? ibuprofeneState = states.Find(s => s.DrugId == DrugId.Ibuprofene);
        MedicationState? dolipraneState = states.Find(s => s.DrugId == DrugId.Doliprane);
        DateTime? maxDateTime = states.Max(s => s.NextMedicationYes);
        return new TimeProgressBar {
            Visible = ibuprofeneState?.Dosage > 0 || (dolipraneState?.Dosage > 0 && ibuprofeneState?.Opinion != MedicationOpinion.Yes),
            Caption = "Ibuprofène",
            Tooltip = GetToolTipIbuprofene(ibuprofeneState),
            Opinion = ibuprofeneState?.Opinion.ToString().ToLower() ?? MedicationOpinion.Yes.ToString().ToLower(),
            CurrentValue = GetDuration(ibuprofeneState?.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(ibuprofeneState?.LastMedicationNo, ibuprofeneState?.NextMedicationYes)),
            MaxWidthValue = Math.Max((int)Math.Ceiling(GetDuration(ibuprofeneState?.LastMedicationNo, maxDateTime)), 6),
            NumberMedication = ibuprofeneState?.NumberMedication ?? 0
        };
    }

    private static string GetToolTipIbuprofene(MedicationState? state) {
        string tooltip = "";
        string? nextMedicationYes = state?.NextMedicationYes?.ToString("HH:mm");
        if (state?.NextMedicationYes != null)
            tooltip = $"Prise possible à partir de {nextMedicationYes}";
        if (state?.Dosage > 0)
            tooltip += $"{(tooltip != "" ? "<br/><br/>" : "")}{state.Dosage} prise{(state.Dosage > 1 ? "s" : "")} d'ibuprofène en 24h";
        return tooltip;
    }

    private static double GetDuration(DateTime? dateTime1, DateTime? dateTime2) {
        if (dateTime1 == null || dateTime2 == null)
            return 0;
        return (dateTime2.Value - dateTime1.Value).TotalHours;
    }

    private static TimeProgressBar GetProgressBarHumex(List<MedicationState> states) {
        MedicationState? humexState = states.Find(s => s.DrugId == DrugId.Humex);
        MedicationState? ibuprofeneState = states.Find(s => s.DrugId == DrugId.Ibuprofene);
        DateTime? maxDateTime = states.Max(s => s.NextMedicationYes);
        return new TimeProgressBar {
            Visible = humexState?.Dosage > 0 || (ibuprofeneState?.Dosage > 0 && humexState?.Opinion != MedicationOpinion.Yes),
            Caption = humexState?.NextDrug switch {
                DrugId.HumexJour => "Humex jour",
                DrugId.HumexNuit => "Humex nuit",
                _                => "Humex"
            },
            Tooltip = GetToolTipParacetamol(humexState),
            Opinion = humexState?.Opinion.ToString().ToLower() ?? MedicationOpinion.Yes.ToString().ToLower(),
            CurrentValue = GetDuration(humexState?.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(humexState?.LastMedicationNo, humexState?.NextMedicationYes)),
            MaxWidthValue = Math.Max((int)Math.Ceiling(GetDuration(humexState?.LastMedicationNo, maxDateTime)), 6),
            NumberMedication = humexState?.NumberMedication ?? 0
        };
    }

    private static TimeProgressBar GetProgressBarAntibiotique(List<MedicationState> states) {
        MedicationState? state = states.Find(s => s.DrugId == DrugId.Antibiotique);
        DateTime? maxDateTime = states.Max(s => s.NextMedicationYes);
        return new TimeProgressBar {
            Visible = state?.Dosage > 0,
            Caption = "Antibiotique",
            Tooltip = GetToolTipAntibiotique(state),
            Opinion = state?.Opinion.ToString().ToLower() ?? MedicationOpinion.Yes.ToString().ToLower(),
            CurrentValue = GetDuration(state?.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(state?.LastMedicationNo, state?.NextMedicationYes)),
            MaxWidthValue = Math.Max((int)Math.Ceiling(GetDuration(state?.LastMedicationNo, maxDateTime)), 6),
            NumberMedication = state?.NumberMedication ?? 0
        };
    }

    private static string GetToolTipAntibiotique(MedicationState? state) {
        string tooltip = "";
        if (state?.NextMedicationYes != null)
            tooltip = $"Prise possible à partir de {state.NextMedicationYes.Value:HH:mm}";
        if (state?.Dosage > 0)
            tooltip += $"{(tooltip != "" ? "<br/><br/>" : "")}{state.Dosage} prise{(state.Dosage > 1 ? "s" : "")} d'antibiotique en 24h";
        return tooltip;
    }

    private static TimeProgressBar GetProgressBarSmecta(List<MedicationState> states) {
        MedicationState? state = states.Find(s => s.DrugId == DrugId.Smecta);
        DateTime? maxDateTime = states.Max(s => s.NextMedicationYes);
        return new TimeProgressBar {
            Visible = state?.Dosage > 0 || state?.Opinion != MedicationOpinion.Yes,
            Caption = "Smecta",
            Tooltip = GetToolTipSmecta(state),
            Opinion = state?.Opinion.ToString().ToLower() ?? MedicationOpinion.Yes.ToString().ToLower(),
            CurrentValue = GetDuration(state?.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(state?.LastMedicationNo, state?.NextMedicationYes)),
            MaxWidthValue = Math.Max((int)Math.Ceiling(GetDuration(state?.LastMedicationNo, maxDateTime)), 6),
            NumberMedication = state?.NumberMedication ?? 0
        };
    }

    private static string GetToolTipSmecta(MedicationState? state) {
        string tooltip = "";
        if (state?.NextMedicationYes != null)
            tooltip = $"Prise possible à partir de {state.NextMedicationYes.Value:HH:mm}";
        if (state?.Dosage > 0)
            tooltip += $"{(tooltip != "" ? "<br/><br/>" : "")}{state.Dosage} prise{(state.Dosage > 1 ? "s" : "")} de smecta en 24h";
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
