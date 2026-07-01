using ModelContextProtocol.Server;

namespace Cars.Mcp.Tools;

[McpServerToolType]
public static partial class CarsMcpTools
{
    private static object? ValidateCarInput(string brand, string model, int year)
    {
        if (string.IsNullOrWhiteSpace(brand))
        {
            return new
            {
                valid = false,
                message = "Brand is required."
            };
        }

        if (string.IsNullOrWhiteSpace(model))
        {
            return new
            {
                valid = false,
                message = "Model is required."
            };
        }

        var maxYear = DateTimeOffset.UtcNow.Year + 1;

        if (year < 1886 || year > maxYear)
        {
            return new
            {
                valid = false,
                message = $"Year must be between 1886 and {maxYear}."
            };
        }

        return null;
    }
}
