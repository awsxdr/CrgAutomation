namespace CrgAutomation;

public abstract record GameEvent(
    int Tick,
    ExpectedState ExpectedState
);

public record LineupClockStartedEvent(
    int Tick,
    ExpectedState ExpectedState
) : GameEvent(Tick, ExpectedState);

public record JamStartedEvent(
    int Tick,
    ExpectedState ExpectedState,
    OnTrackTeam HomeTeam,
    OnTrackTeam AwayTeam
) : GameEvent(Tick, ExpectedState);

public record JamAdvancedEvent(
    int Tick,
    ExpectedState ExpectedState
) : GameEvent(Tick, ExpectedState);

public record SkaterLinedUpEvent(
    int Tick,
    ExpectedState ExpectedState,
    string Number,
    Position Position,
    TeamType Team
) : GameEvent(Tick, ExpectedState);

public record SkaterSentToBoxEvent(
    int Tick,
    ExpectedState ExpectedState,
    string PenaltyCode,
    TeamType Team,
    string SkaterNumber
) : GameEvent(Tick, ExpectedState);

public record SkaterReleasedFromBoxEvent(
    int Tick,
    ExpectedState ExpectedState,
    TeamType Team,
    string SkaterNumber
) : GameEvent(Tick, ExpectedState);

public record SkaterSatInBoxEvent(
    int Tick,
    ExpectedState ExpectedState,
    TeamType Team,
    string SkaterNumber
) : GameEvent(Tick, ExpectedState);

public record LeadEarnedEvent(
    int Tick,
    ExpectedState ExpectedState,
    TeamType Team
) : GameEvent(Tick, ExpectedState);

public record LeadLostEvent(
    int Tick,
    ExpectedState ExpectedState,
    TeamType Team
) : GameEvent(Tick, ExpectedState);

public record JamCalledEvent(
    int Tick,
    ExpectedState ExpectedState
) : GameEvent(Tick, ExpectedState);

public record InitialTripCompletedEvent(
    int Tick,
    ExpectedState ExpectedState,
    TeamType Team
) : GameEvent(Tick, ExpectedState);

public record PointsAwardedEvent(
    int Tick,
    ExpectedState ExpectedState,
    TeamType Team,
    int Points
) : GameEvent(Tick, ExpectedState);

public record TimeoutStartedEvent(
    int Tick,
    ExpectedState ExpectedState,
    TeamType Team
) : GameEvent(Tick, ExpectedState);