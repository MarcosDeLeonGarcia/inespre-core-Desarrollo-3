using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;
using INESPRE.Core.Services;

namespace INESPRE.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IDbConnectionFactory _factory;
    public ReportsController(IDbConnectionFactory factory) => _factory = factory;

    [HttpGet("sales/daily")]
    public async Task<IActionResult> SalesDaily([FromQuery] DateOnly date, int? eventId, int? userId)
    {
        using var conn = _factory.CreateConnection();
        var rows = await conn.QueryAsync("dbo.spRpt_SalesDaily",
            new { Date = date.ToDateTime(TimeOnly.MinValue), EventId = eventId, UserId = userId },
            commandType: CommandType.StoredProcedure);
        return Ok(rows);
    }
}
