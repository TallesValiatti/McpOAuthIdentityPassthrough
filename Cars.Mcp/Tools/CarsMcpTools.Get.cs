using System.ComponentModel;
using Cars.Mcp.Services;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;

namespace Cars.Mcp.Tools;

public static partial class CarsMcpTools
{
    [McpServerTool(Name = "cars_get", ReadOnly = true, Destructive = false)]
    [Authorize(Policy = "CarsRead")]
    [Description("Gets a car by id. Requires the Cars.Read scope.")]
    public static object GetCar(
        CarStore store,
        [Description("The car id.")] Guid id)
    {
        var car = store.Get(id);

        return car is null
            ? new { found = false, message = "Car not found." }
            : new { found = true, car };
    }
}
