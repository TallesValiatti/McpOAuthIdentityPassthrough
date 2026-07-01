using System.ComponentModel;
using Cars.Mcp.Services;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;

namespace Cars.Mcp.Tools;

public static partial class CarsMcpTools
{
    [McpServerTool(Name = "cars_delete", ReadOnly = false, Destructive = true)]
    [Authorize(Policy = "CarsDelete")]
    [Description("Deletes a car. Requires the Cars.Delete scope.")]
    public static object DeleteCar(
        CarStore store,
        [Description("The car id.")] Guid id)
    {
        var deleted = store.Delete(id);

        return new
        {
            deleted
        };
    }
}
