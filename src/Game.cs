namespace CrgAutomation;

public record Game(
    Team HomeTeam,
    Team AwayTeam,
    Official[] Officials    
);

public static class GameFactory {
    public static Game CreateRandom() {
        var homeTeam = TeamFactory.CreateRandom();

        Team awayTeam;

        do {
            awayTeam = TeamFactory.CreateRandom();
        } while(awayTeam.Color == homeTeam.Color);

        return new Game(
            homeTeam,
            awayTeam,
            OfficialFactory.CreateRandomCrew()
        );
    }
}