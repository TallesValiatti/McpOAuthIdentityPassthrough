using System.ComponentModel;
using Cars.Mcp.Services;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;

namespace Cars.Mcp.Tools;

public static partial class CarsMcpTools
{
    [McpServerTool(Name = "cars_update", ReadOnly = false, Idempotent = true, Destructive = false)]
    [Authorize(Policy = "CarsWrite")]
    [Description("Updates an existing car. Requires the Cars.Write scope.")]
    public static object UpdateCar(
        CarStore store,
        [Description("The car id.")] Guid id,
        [Description("The new car brand.")] string brand,
        [Description("The new car model.")] string model,
        [Description("The new manufacturing year.")] int year)
    {
        var validation = ValidateCarInput(brand, model, year);

        if (validation is not null)
            return validation;

        var car = store.Update(id, brand, model, year);

        return car is null
            ? new { updated = false, message = "Car not found." }
            : new { updated = true, car };
    }
}
