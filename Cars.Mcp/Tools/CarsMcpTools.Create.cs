using System.ComponentModel;
using Cars.Mcp.Services;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;

namespace Cars.Mcp.Tools;

public static partial class CarsMcpTools
{
    [McpServerTool(Name = "cars_create", ReadOnly = false, Destructive = false)]
    [Authorize(Policy = "CarsWrite")]
    [Description("Creates a new car. Requires the Cars.Write scope.")]
    public static object CreateCar(
        CarStore store,
        [Description("The car brand. Example: Toyota.")] string brand,
        [Description("The car model. Example: Corolla.")] string model,
        [Description("The manufacturing year.")] int year)
    {
        var validation = ValidateCarInput(brand, model, year);

        if (validation is not null)
            return validation;

        var car = store.Create(brand, model, year);

        return new
        {
            created = true,
            car
        };
    }
}
