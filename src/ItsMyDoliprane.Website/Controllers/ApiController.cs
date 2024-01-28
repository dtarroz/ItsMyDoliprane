using ItsMyDoliprane.Enums;
using ItsMyDoliprane.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ItsMyDoliprane.Controllers;

public class ApiController : Controller
{
    private readonly ILogger<ApiController> _logger;

    protected ApiController(ILogger<ApiController> logger) {
        _logger = logger;
    }

    protected JsonResult Execute(Action callback) {
        JsonErrorCode code = JsonErrorCode.Ok;
        object? result = null;
        try {
            callback();
        }
        catch (JsonErrorCodeException ex) {
            code = ex.ErrorCode;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Api Controller Execute");
            code = JsonErrorCode.UnknownError;
        }
        return Json(new {
                        code,
                        result
                    });
    }
}
