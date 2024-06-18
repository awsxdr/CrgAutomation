public record ExpectedState(
    Clock PeriodClock,
    Clock JamClock,
    Clock TimeoutClock,
    Clock LineupClock,
    Clock IntervalClock,
    int PeriodNumber
);

public record Clock(
    int Value,
    bool IsRunning
);