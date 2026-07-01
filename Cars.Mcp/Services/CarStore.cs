using System.Collections.Concurrent;
using Cars.Mcp.Models;

namespace Cars.Mcp.Services;

public sealed class CarStore
{
    private readonly ConcurrentDictionary<Guid, Car> _cars = new();

    public CarStore()
    {
        var seedData = new (string Brand, string Model, int Year)[]
        {
            ("Toyota", "Corolla", 2024),
            ("Honda", "Civic", 2023),
            ("Ford", "Mustang", 2022),
            ("Chevrolet", "Camaro", 2021),
            ("Volkswagen", "Golf", 2023),
            ("BMW", "M3", 2024),
            ("Mercedes-Benz", "C-Class", 2022),
            ("Audi", "A4", 2023),
            ("Tesla", "Model 3", 2024),
            ("Hyundai", "Elantra", 2021),
        };

        foreach (var (brand, model, year) in seedData)
        {
            var car = new Car(
                Guid.NewGuid(),
                brand,
                model,
                year,
                DateTimeOffset.UtcNow,
                null);

            _cars[car.Id] = car;
        }
    }

    public IReadOnlyCollection<Car> List()
    {
        return _cars.Values
            .OrderBy(car => car.Brand)
            .ThenBy(car => car.Model)
            .ThenByDescending(car => car.Year)
            .ToArray();
    }

    public Car? Get(Guid id)
    {
        return _cars.GetValueOrDefault(id);
    }

    public Car Create(string brand, string model, int year)
    {
        var car = new Car(
            Guid.NewGuid(),
            brand.Trim(),
            model.Trim(),
            year,
            DateTimeOffset.UtcNow,
            null);

        _cars[car.Id] = car;

        return car;
    }

    public Car? Update(Guid id, string brand, string model, int year)
    {
        if (!_cars.TryGetValue(id, out var existing))
            return null;

        var updated = new Car(
            id,
            brand.Trim(),
            model.Trim(),
            year,
            existing.CreatedAt,
            DateTimeOffset.UtcNow);

        _cars[id] = updated;

        return updated;
    }

    public bool Delete(Guid id)
    {
        return _cars.TryRemove(id, out _);
    }
}
