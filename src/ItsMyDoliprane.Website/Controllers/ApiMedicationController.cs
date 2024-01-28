using ItsMyDoliprane.Business;
using ItsMyDoliprane.Repository.Models;
using Microsoft.AspNetCore.Mvc;

namespace ItsMyDoliprane.Controllers;

[Route("api/medication")]
public class ApiMedicationController : ApiController
{
    private readonly UseMedications _useMedications;

    public ApiMedicationController(ILogger<ApiMedicationController> logger, UseMedications useMedications) : base(logger) {
        _useMedications = useMedications;
    }

    [HttpPost]
    public IActionResult Add([FromBody] Medication medication) {
        return Execute(() => {
            _useMedications.Add(medication);
        });
    }
}
