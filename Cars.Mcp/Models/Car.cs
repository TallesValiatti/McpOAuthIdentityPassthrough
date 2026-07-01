namespace Cars.Mcp.Models;

public sealed record Car(
    Guid Id,
    string Brand,
    string Model,
    int Year,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);
