using Microsoft.AspNetCore.Mvc;
using OrderCompletion.Api.Core.Ports;
using OrderCompletion.Api.Shared.Errors;

namespace OrderCompletion.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderCompletionUseCase _usecase;

    public OrdersController(IOrderCompletionUseCase usecase)
    {
        _usecase = usecase;
    }
    
    [HttpPatch(Name = "Complete")]
    public async Task<ActionResult> Complete(List<int> orderIds, CancellationToken cancellationToken)
    {
        var res = await _usecase.CompleteOrders(orderIds, cancellationToken);
        if (res.IsSuccess)
            return Ok(res.Successes);

        var error = res.Errors.First();
        return error switch
        {
            ValidationError e => Problem(e.Message, statusCode: StatusCodes.Status400BadRequest),
            NotFoundError e => Problem(e.Message, statusCode: StatusCodes.Status404NotFound),
            ConflictError e => Problem(e.Message, statusCode: StatusCodes.Status409Conflict),
            TimedOutError e => Problem(e.Message, statusCode: StatusCodes.Status408RequestTimeout),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { error = error.Message })
        };
    }
}