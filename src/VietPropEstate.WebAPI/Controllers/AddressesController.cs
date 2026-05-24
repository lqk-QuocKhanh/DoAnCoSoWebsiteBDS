using Microsoft.AspNetCore.Mvc;
using VietPropEstate.Application.Features.Addresses.Queries.GetProvinces;
using VietPropEstate.Application.Features.Addresses.Queries.GetWardsByProvince;

namespace VietPropEstate.WebAPI.Controllers;

[Route("api/addresses")]
[Route("api/address")]
public sealed class AddressesController : BaseApiController
{
    /// <summary>Returns all active Vietnamese provinces/cities.</summary>
    [HttpGet("provinces")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProvinces(CancellationToken cancellationToken)
    {
        var provinces = await Mediator.Send(new GetProvincesQuery(), cancellationToken);
        return Ok(provinces);
    }

    /// <summary>Returns wards/communes for a province code.</summary>
    [HttpGet("wards/{provinceCode:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWards(int provinceCode, CancellationToken cancellationToken)
    {
        var wards = await Mediator.Send(new GetWardsByProvinceQuery(provinceCode), cancellationToken);
        return Ok(wards);
    }
}
