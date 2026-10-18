using ItsMyDoliprane.Business;
using Microsoft.AspNetCore.Mvc;

namespace ItsMyDoliprane.Controllers;

[Route("api/drug")]
public class ApiDrugController : ApiController
{
    private readonly UseDrugs _useDrugs;

    public ApiDrugController(ILogger<ApiDrugController> logger, UseDrugs useDrugs) : base(logger) {
        _useDrugs = useDrugs;
    }

    [HttpPut]
    public IActionResult Update([FromBody] UpdateDrugRequest request) {
        return Execute(() => {
            _useDrugs.UpdateDrug(request.Id, request.ForAdult, request.ForChild);
        });
    }
}

public record UpdateDrugRequest(int Id, bool ForAdult, bool ForChild);

