using Func;

namespace CrgAutomation;

public abstract record GameState(
    Game Game,
    int Tick,
    GameEvent[] Events,
    BoxedSkater[] PenaltyBox,
    int PeriodClock
);

public record PreGameGameState(
    Game Game    
) : GameState(Game, 0, [], [], 30 * 60);

public record LineupInProgressGameState(
    Game Game,
    int Tick,
    GameEvent[] Events,
    BoxedSkater[] PenaltyBox,
    int PeriodClock,
    bool PeriodClockRunning,
    int PeriodNumber,
    int HomeTimeoutsTaken,
    int AwayTimeoutsTaken,
    int StartTick
) : GameState(Game, Tick, Events, PenaltyBox, PeriodClock);

public record TimeoutInProgressGameState(
    Game Game,
    int Tick,
    GameEvent[] Events,
    BoxedSkater[] PenaltyBox,
    int PeriodClock,
    int PeriodNumber,
    int HomeTimeoutsTaken,
    int AwayTimeoutsTaken,
    int StartTick
) : GameState(Game, Tick, Events, PenaltyBox, PeriodClock);

public record IntermissionInProgressGameState(
    Game Game,
    int Tick,
    GameEvent[] Events,
    BoxedSkater[] PenaltyBox,
    int HomeTimeoutsTaken,
    int AwayTimeoutsTaken,
    int StartTick
) : GameState(Game, Tick, Events, PenaltyBox, 30 * 60);

public record JamInProgressGameState(
    Game Game,
    int Tick,
    GameEvent[] Events,
    BoxedSkater[] PenaltyBox,
    int StartTick,
    int PeriodClock,
    int PeriodNumber,
    int HomeTimeoutsTaken,
    int AwayTimeoutsTaken,
    OnTrackTeam HomeTeam,
    OnTrackTeam AwayTeam,
    JammerState HomeJammer,
    JammerState AwayJammer,
    Option<TeamType> LeadJammerTeam
) : GameState(Game, Tick, Events, PenaltyBox, PeriodClock);

public record PostGameGameState(
    Game Game,
    int Tick,
    GameEvent[] Events
) : GameState(Game, Tick, Events, [], 0);

public record OnTrackTeam(
    Skater Jammer,
    Skater Pivot,
    Skater[] Blockers
) {
    public Skater[] Skaters => [
        Jammer,
        Pivot,
        ..Blockers
    ];
}

public record JammerState(
    float Progress,
    bool EligibleForLead,
    int Trip
);

public enum TeamType {
    Home,
    Away,
}

public record BoxedSkater(
    Skater Skater,
    TeamType Team,
    Position Position,
    float DistanceToBox,
    int TicksRemaining
) : Skater(Skater);