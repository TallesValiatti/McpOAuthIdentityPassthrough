using System.ComponentModel;
using Cars.Mcp.Models;
using Cars.Mcp.Services;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;

namespace Cars.Mcp.Tools;

public static partial class CarsMcpTools
{
    [McpServerTool(Name = "cars_list", ReadOnly = true, Destructive = false)]
    [Authorize(Policy = "CarsRead")]
    [Description("Lists all cars. Requires the Cars.Read scope.")]
    public static IReadOnlyCollection<Car> ListCars(CarStore store)
    {
        return store.List();
    }
}
