using Microsoft.AspNetCore.Mvc;
using VietPropEstate.Application.Features.Properties.Queries.GetPropertyTypes;
using VietPropEstate.Application.Features.Properties.Queries.GetTransactionTypes;

namespace VietPropEstate.WebAPI.Controllers;

/// <summary>Property and transaction type lookups.</summary>
public class PropertyTypesController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPropertyTypes(CancellationToken cancellationToken)
        => Ok(await Mediator.Send(new GetPropertyTypesQuery(), cancellationToken));

    [HttpGet("transaction-types")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactionTypes(CancellationToken cancellationToken)
        => Ok(await Mediator.Send(new GetTransactionTypesQuery(), cancellationToken));
}
