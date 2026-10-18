using ItsMyDoliprane.Business;
using ItsMyDoliprane.Models;
using Microsoft.AspNetCore.Mvc;

namespace ItsMyDoliprane.Controllers;

public class SettingsController : Controller
{
    private readonly UseDrugs _useDrugs;

    public SettingsController(UseDrugs useDrugs) {
        _useDrugs = useDrugs;
    }

    public IActionResult Index() {
        var drugs = _useDrugs.GetDrugs();
        SettingsViewModel model = new SettingsViewModel {
            Drugs = drugs.Select(d => new DrugSettingViewModel {
                                     Id = d.Id,
                                     Name = d.Name,
                                     ForAdult = d.ForAdult,
                                     ForChild = d.ForChild
                                 }).ToList()
        };
        return View(model);
    }
}

