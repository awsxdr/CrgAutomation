namespace CrgAutomation;

public record Official(
    Guid Id,
    string Name,
    bool IsHead,
    OfficialRole Role
);

public enum OfficialRole {
    PenaltyLineupTracker,
    PenaltyWrangler,
    InsideWhiteboard,
    JamTimer,
    Scorekeeper,
    ScoreboardOperator,
    PenaltyBoxManager,
    PenaltyBoxTimer,
    InsidePackReferee,
    OutsidePackReferee,
    JammerReferee,
}

public static class OfficialFactory {
    public static Official CreateRandom(OfficialRole role, bool isHead) =>
        new Official(
            Guid.NewGuid(),
            NameFactory.GetRandomName(),
            isHead,
            role
        );

    public static Official[] CreateRandomCrew() =>
        [
            CreateRandom(OfficialRole.PenaltyLineupTracker, true),
            CreateRandom(OfficialRole.PenaltyLineupTracker, false),
            CreateRandom(OfficialRole.PenaltyWrangler, false),
            CreateRandom(OfficialRole.InsideWhiteboard, false),
            CreateRandom(OfficialRole.JamTimer, false),
            CreateRandom(OfficialRole.Scorekeeper, false),
            CreateRandom(OfficialRole.Scorekeeper, false),
            CreateRandom(OfficialRole.ScoreboardOperator, false),
            CreateRandom(OfficialRole.PenaltyBoxManager, false),
            CreateRandom(OfficialRole.PenaltyBoxTimer, false),
            CreateRandom(OfficialRole.PenaltyBoxTimer, false),
            CreateRandom(OfficialRole.InsidePackReferee, true),
            CreateRandom(OfficialRole.InsidePackReferee, false),
            CreateRandom(OfficialRole.OutsidePackReferee, false),
            CreateRandom(OfficialRole.OutsidePackReferee, false),
            CreateRandom(OfficialRole.OutsidePackReferee, false),
            CreateRandom(OfficialRole.JammerReferee, false),
            CreateRandom(OfficialRole.JammerReferee, false),
        ];
}