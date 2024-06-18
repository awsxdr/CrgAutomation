using CrgAutomation;

public record Team(
    Guid Id,
    string Name,
    Skater[] Skaters,
    string Color
);

public static class TeamFactory {
    public static Team CreateRandom() => 
        new Team(
            Guid.NewGuid(),
            $"{NameFactory.GetRandomPlace()} Roller Derby",
            GetRandomRoster(),
            NameFactory.GetRandomColor()
        );

    private static Skater[] GetRandomRoster() {
        var roster = new List<Skater>();
        var targetCount = 8 + Random.Shared.Next(8);

        for(var i = 0; i < targetCount; ++i) {
            Skater skater;

            do {
                skater = SkaterFactory.CreateRandom();
            } while(roster.Any(s => s.Number == skater.Number));

            roster.Add(skater);
        }

        return roster.OrderBy(s => s.Number).ToArray();
    }
}