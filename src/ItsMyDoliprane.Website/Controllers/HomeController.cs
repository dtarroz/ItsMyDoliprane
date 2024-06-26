﻿using System.Diagnostics;
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
    private readonly UseDosages _useDosages;
    private readonly List<Drug> _drugs;
    private readonly List<Person> _persons;

    public HomeController(UsePersons usePersons, UseDrugs useDrugs, UseMedications useMedications, UseDosages useDosages) {
        _useMedications = useMedications;
        _useDosages = useDosages;
        _drugs = useDrugs.GetDrugs();
        _persons = usePersons.GetPersons();
    }

    public IActionResult Index(int personId = 1) {
        List<Medication> medications = _useMedications.GetMedicationsSinceDate(personId, DateTime.Now.AddDays(-2));
        var states = _useMedications.GetMedicationsStates(personId).Where(s => IsDrugAllowForPerson(s.DrugId, personId)).ToList();
        HomeViewModel model = new HomeViewModel {
            PersonId = personId,
            Persons = _persons.ToDictionary(p => p.Id, p => p.Name),
            Drugs = GetListDrugs(personId),
            DosageParacetamol = _useDosages.GetDosageSinceDate(personId, 1, DateTime.Now.AddDays(-1)) / 1000f,
            Medication4 = GetMedication(medications, 4),
            Medication6 = GetMedication(medications, 6),
            ProgressBarDoliprane = GetProgressBarDoliprane(states),
            ProgressBarHumex = GetProgressBarHumex(states),
            ProgressBarAntibiotique = GetProgressBarAntibiotique(states),
            Medications = GetMedications(medications)
        };
        return View(model);
    }

    private Dictionary<int, string> GetListDrugs(int personId) {
        return _drugs.Where(drug => drug.Visible && IsDrugAllowForPerson(drug, personId)).ToDictionary(p => p.Id, p => p.Name);
    }

    private bool IsDrugAllowForPerson(DrugId drugId, int personId) {
        Drug drug = _drugs.Single(p => p.Id == (int)drugId);
        return IsDrugAllowForPerson(drug, personId);
    }

    private bool IsDrugAllowForPerson(Drug drug, int personId) {
        Person person = _persons.Single(p => p.Id == personId);
        switch (person.IsAdult) {
            case true when drug.ForAdult:
            case false when drug.ForChild: return true;
            default: return false;
        }
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
        MedicationState? dolipraneState = states.Find(s => s.DrugId == DrugId.Doliprane);
        DateTime? maxDateTime = states.Max(s => s.NextMedicationYes);
        return new TimeProgressBar {
            Visible = dolipraneState?.Dosage > 0,
            Caption = "Doliprane",
            Tooltip = GetToolTipParacetamol(dolipraneState),
            Opinion = dolipraneState?.Opinion.ToString().ToLower() ?? MedicationOpinion.Yes.ToString().ToLower(),
            CurrentValue = GetDuration(dolipraneState?.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(dolipraneState?.LastMedicationNo, dolipraneState?.NextMedicationYes)),
            MaxWidthValue = Math.Max((int)Math.Ceiling(GetDuration(dolipraneState?.LastMedicationNo, maxDateTime)), 6)
        };
    }

    private static string GetToolTipParacetamol(MedicationState? medicationState) {
        string tooltip = "";
        if (medicationState?.NextMedicationYes != null)
            tooltip = $"Prise conseillée à partir de {medicationState.NextMedicationYes.Value:HH:mm}";
        if (medicationState?.NextMedicationPossible != null)
            tooltip += $"<br/>mais possible à partir de {medicationState.NextMedicationPossible.Value:HH:mm}";
        if (medicationState?.Dosage > 0)
            tooltip += $"{(tooltip != "" ? "<br/><br/>" : "")}{medicationState.Dosage / 1000.0}g de paracétamol en 24h";
        return tooltip;
    }

    private static double GetDuration(DateTime? dateTime1, DateTime? dateTime2) {
        if (dateTime1 == null || dateTime2 == null)
            return 0;
        return (dateTime2.Value - dateTime1.Value).TotalHours;
    }

    private static TimeProgressBar GetProgressBarHumex(List<MedicationState> states) {
        MedicationState? humexState = states.Find(s => s.DrugId == DrugId.Humex);
        DateTime? maxDateTime = states.Max(s => s.NextMedicationYes);
        return new TimeProgressBar {
            Visible = humexState?.Dosage > 0,
            Caption = humexState?.NextDrug switch {
                DrugId.HumexJour => "Humex jour",
                DrugId.HumexNuit => "Humex nuit",
                _                => "Humex"
            },
            Tooltip = GetToolTipParacetamol(humexState),
            Opinion = humexState?.Opinion.ToString().ToLower() ?? MedicationOpinion.Yes.ToString().ToLower(),
            CurrentValue = GetDuration(humexState?.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(humexState?.LastMedicationNo, humexState?.NextMedicationYes)),
            MaxWidthValue = Math.Max((int)Math.Ceiling(GetDuration(humexState?.LastMedicationNo, maxDateTime)), 6)
        };
    }

    private static TimeProgressBar GetProgressBarAntibiotique(List<MedicationState> states) {
        MedicationState? state = states.Find(s => s.DrugId == DrugId.Antibiotique);
        DateTime? maxDateTime = states.Max(s => s.NextMedicationYes);
        return new TimeProgressBar {
            Visible = state?.Dosage > 0,
            Caption = "Antibiotique",
            Tooltip = GetToolTipAntibiotique(state), //$"{state?.Dosage} prise{(state?.Dosage > 1 ? "s" : "")} d'antibiotique en 24h",
            Opinion = state?.Opinion.ToString().ToLower() ?? MedicationOpinion.Yes.ToString().ToLower(),
            CurrentValue = GetDuration(state?.LastMedicationNo, DateTime.Now),
            MaxValue = (int)Math.Ceiling(GetDuration(state?.LastMedicationNo, state?.NextMedicationYes)),
            MaxWidthValue = Math.Max((int)Math.Ceiling(GetDuration(state?.LastMedicationNo, maxDateTime)), 6)
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

    private List<MedicationViewModel> GetMedications(IEnumerable<Medication> medications) {
        return medications.GroupBy(m => m.Date)
                          .Select(group => new MedicationViewModel {
                                      Date = DateTime.Parse(group.Key).ToString("dd/MM"),
                                      Details = group.Select(d => new MedicationDetailViewModel {
                                                         Drug = _drugs.Find(dr => dr.Id == d.DrugId)?.Name ?? "",
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
