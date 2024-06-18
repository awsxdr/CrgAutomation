namespace CrgAutomation;

using Func;

public enum Position {
    Jammer,
    Pivot,
    Blocker,
}

public record Skater(
    Guid Id,
    string Name,
    string Number,
    Position FavoredPosition,
    float BaseSpeed,
    float PenaltyChance
);

public static class SkaterFactory
{
    private const float PenaltyChance = 1.0f / 1200.0f;

    public static Skater CreateRandom() =>
        new(
            Guid.NewGuid(),
            NameFactory.GetRandomName(),
            GetRandomNumber(),
            GetRandomPosition(),
            GetRandomSpeed(),
            GetRandomPenaltyChance()
        );

    private static string GetRandomNumber() =>
        Enumerable.Range(0, Random.Shared.Next(4) + 1)
            .Select(_ => Random.Shared.Next(10).ToString())
            .Map(string.Concat);

    private static Position GetRandomPosition() =>
        Random.Shared.Next(3) switch {
            0 => Position.Jammer,
            1 => Position.Pivot,
            _ => Position.Blocker,
        };

    private static float GetRandomSpeed() =>
        Random.Shared.NextSingle() * 2.0f + 3.0f;
    
    private static float GetRandomPenaltyChance() =>
        (Random.Shared.NextSingle() + 1.0f) * PenaltyChance;
}