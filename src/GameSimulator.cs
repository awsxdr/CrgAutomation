using CrgAutomation;
using Func;
using static Func.Option;

public class GameSimulator(Game _game)
{
    public const int MillisecondsPerTick = 10;
    public const int TicksPerSecond = 1000 / MillisecondsPerTick;
    public const int PeriodClockLengthInTicks = 30 * 60 * TicksPerSecond;
    public const int LineupLengthInTicks = 30 * TicksPerSecond;
    public const int PenaltyLengthInTicks = 30 * TicksPerSecond;
    public const int JamLengthInTicks = 2 * 60 * TicksPerSecond;
    public const int TimeoutLengthInTicks = 60 * TicksPerSecond;
    public const int IntermissionLengthInTicks = 15 * 60 * TicksPerSecond;

    public Task<GameEvent[]> Run() => Task.Run(() => {
        GameState state = new PreGameGameState(_game);

        do {
            state = Tick(state);
        } while (state is not PostGameGameState);

        return state.Events.OrderBy(e => e.Tick).ToArray();
    });

    private static GameState Tick(GameState state) =>
        state switch {
            PreGameGameState pg => TickPreGame(pg),
            LineupInProgressGameState lip => TickLineup(lip),
            JamInProgressGameState jip => TickJam(jip),
            TimeoutInProgressGameState tip => TickTimeout(tip),
            IntermissionInProgressGameState iip => TickIntermission(iip),
            _ => new PostGameGameState(state.Game, state.Tick + MillisecondsPerTick, state.Events)
        };

    private static GameState TickPreGame(PreGameGameState state) =>
        new LineupInProgressGameState(
            state.Game, 
            0, 
            [
                new LineupClockStartedEvent(
                    0,
                    new ExpectedState(
                        new Clock(PeriodClockLengthInTicks / TicksPerSecond, false),
                        new Clock(0, false),
                        new Clock(0, false),
                        new Clock(0, true),
                        new Clock(0, false),
                        1
                    )
                )
            ], 
            [], 
            PeriodClockLengthInTicks,
            false, 
            1,
            0,
            0,
            0);

    private static GameState TickLineup(LineupInProgressGameState state)
    {
        var nextTick = state.Tick + MillisecondsPerTick;
        var duration = nextTick - state.StartTick;

        if (state.Tick == state.StartTick) {
            if(Random.Shared.Next(10) == 0) {
                var team = Random.Shared.Next(2) == 0 ? TeamType.Home : TeamType.Away;

                if ((team == TeamType.Home && state.HomeTimeoutsTaken < 3) || (team == TeamType.Away && state.AwayTimeoutsTaken < 3)) {
                    var timeoutStartTick = state.Tick + Random.Shared.Next(29 * TicksPerSecond);
                    var events = state.Events.Append(
                        new TimeoutStartedEvent(
                            timeoutStartTick,
                            new ExpectedState(
                                new Clock(state.PeriodClock, false),
                                new Clock(0, false),
                                new Clock(0, true),
                                new Clock(0, false),
                                new Clock(0, false),
                                state.PeriodNumber
                            ),
                            team
                        )
                    ).ToArray();

                    return new TimeoutInProgressGameState(
                        state.Game,
                        timeoutStartTick,
                        events,
                        state.PenaltyBox,
                        state.PeriodClock,
                        state.PeriodNumber,
                        state.HomeTimeoutsTaken + team == TeamType.Home ? 1 : 0,
                        state.AwayTimeoutsTaken + team == TeamType.Away ? 1 : 0,
                        timeoutStartTick
                    );
                }
            }
        }

        var periodClock = state.PeriodClock;
        if (state.PeriodClockRunning) {
            periodClock -= 1;
            if (periodClock <= 0) {
                Console.WriteLine($"Period clock expired during lineup in period {state.PeriodNumber}");
                if (state.PeriodNumber == 1) {
                    return new IntermissionInProgressGameState(
                        state.Game,
                        nextTick,
                        state.Events,
                        state.PenaltyBox.Select(p => p with { DistanceToBox = 0 }).ToArray(),
                        state.HomeTimeoutsTaken,
                        state.AwayTimeoutsTaken,
                        nextTick
                    );
                } else {
                    return new PostGameGameState(state.Game, nextTick, state.Events);
                }
            }
        }

        if (duration >= LineupLengthInTicks) {
            var homeTeam = CreateLineup(state.Game.HomeTeam, state.PenaltyBox);
            var awayTeam = CreateLineup(state.Game.AwayTeam, state.PenaltyBox);

            IEnumerable<GameEvent> GenerateLineupEvents(OnTrackTeam team, TeamType teamType) =>
                team.Skaters.Select(s => new SkaterLinedUpEvent(
                    nextTick - Random.Shared.Next(0, 25 * TicksPerSecond),
                    new ExpectedState(
                        new Clock(state.PeriodClock, state.PeriodClockRunning),
                        new Clock(0, false),
                        new Clock(0, false),
                        new Clock(duration / TicksPerSecond, true),
                        new Clock(0, false),
                        state.PeriodNumber
                    ),
                    s.Number,
                    team.Jammer.Id == s.Id ? Position.Jammer 
                        : team.Pivot.Id == s.Id ? Position.Pivot 
                        : Position.Blocker,
                    teamType
                ));
            
            var events = state.Events
                .Concat(GenerateLineupEvents(homeTeam, TeamType.Home))
                .Concat(GenerateLineupEvents(awayTeam, TeamType.Away))
                .Append(new JamStartedEvent(
                    nextTick,
                    new ExpectedState(
                        new Clock(state.PeriodClock, true),
                        new Clock(0, true),
                        new Clock(0, false),
                        new Clock(0, false),
                        new Clock(0, false),
                        state.PeriodNumber
                    ),
                    homeTeam,
                    awayTeam
                ))
                .ToArray();

            return new JamInProgressGameState(
                state.Game,
                nextTick,
                events,
                state.PenaltyBox,
                nextTick,
                state.PeriodClock,
                state.PeriodNumber,
                state.HomeTimeoutsTaken,
                state.AwayTimeoutsTaken,
                homeTeam,
                awayTeam,
                new JammerState(0.0f, true, 0),
                new JammerState(0.0f, true, 0),
                None<TeamType>()
            );
        }

        return state with { Tick = nextTick };
    }

    private static GameState TickJam(JamInProgressGameState state)
    {
        var newBoxedSkaters = 
            state.HomeTeam.Skaters.Select(s => (Team: TeamType.Home, Skater: s))
            .Concat(state.AwayTeam.Skaters.Select(s => (Team: TeamType.Away, Skater: s)))
            .Where(s => !state.PenaltyBox.Any(p => p.Id == s.Skater.Id))
            .Where(s => Random.Shared.Next((int)(TicksPerSecond / s.Skater.PenaltyChance)) == 0)
            .Select(s => new BoxedSkater(
                s.Skater, 
                s.Team,
                s.Skater.Id == state.HomeTeam.Jammer.Id || s.Skater.Id == state.AwayTeam.Jammer.Id ? Position.Jammer
                    : s.Skater.Id == state.HomeTeam.Pivot.Id || s.Skater.Id == state.AwayTeam.Pivot.Id ? Position.Pivot
                    : Position.Blocker,
                Random.Shared.NextSingle() * 50.0f,
                PenaltyLengthInTicks))
            .ToArray();

        var events = state.Events;
        var nextTick = state.Tick + 1;
        var jamDuration = nextTick - state.StartTick;

        var expectedState = new ExpectedState(
            new Clock(state.PeriodClock, true),
            new Clock(jamDuration / TicksPerSecond, true),
            new Clock(0, false),
            new Clock(0, false),
            new Clock(0, false),
            state.PeriodNumber
        );


        events = events
            .Concat(newBoxedSkaters
            .Select(b => 
                new SkaterSentToBoxEvent(
                    nextTick,
                    expectedState,
                    GetRandomPenaltyCode(),
                    b.Team,
                    b.Number
                )))
            .ToArray();
            
        BoxedSkater[] boxedSkaters = 
            state.PenaltyBox.Select(s => {
                var speedPerTick = s.Skater.BaseSpeed / TicksPerSecond;
                var distanceToBox = s.DistanceToBox > speedPerTick ? s.DistanceToBox - speedPerTick : 0;
                var ticksRemaining = distanceToBox > 0 ? s.TicksRemaining : s.TicksRemaining - 1;

                if (distanceToBox == 0 && s.DistanceToBox > 0) {
                    events = events
                        .Append(
                            new SkaterSatInBoxEvent(
                                nextTick, 
                                expectedState,
                                s.Team, 
                                s.Number
                            ))
                        .ToArray();
                }

                var result = ticksRemaining > 0
                    ? s with {
                        DistanceToBox = distanceToBox,
                        TicksRemaining = ticksRemaining,
                    }
                    : null;

                if (result == null) {
                    events = events
                        .Append(
                            new SkaterReleasedFromBoxEvent(
                                nextTick, 
                                expectedState,
                                s.Team, 
                                s.Number
                            ))
                        .ToArray();
                }

                return result;
            })
            .Where(s => s != null)
            .Concat(newBoxedSkaters)
            .ToArray()!;
        
        float GetJammerProgress(JammerState jammer, OnTrackTeam team) =>
            boxedSkaters.Any(s => s.Id == team.Jammer.Id)
            ? 0.0f
            : (jammer.Progress + jammer.Progress switch {
                    < 20.0f => team.Jammer.BaseSpeed * 0.5f / TicksPerSecond,
                    _ => team.Jammer.BaseSpeed / TicksPerSecond
                }) % 100.0f;

        var homeJammerProgress = GetJammerProgress(state.HomeJammer, state.HomeTeam);
        var awayJammerProgress = GetJammerProgress(state.AwayJammer, state.AwayTeam);

        var periodClock = state.PeriodClock > 0 ? state.PeriodClock - 1 : 0;

        int CalculateJammerPoints(JammerState jammer) =>
            jammer.Trip == 0 || jammer.Progress > 20.0f
            ? 0
            : (int)(jammer.Progress / 20.0f * 4.0f);

        if (jamDuration > JamLengthInTicks) {
            if (state.HomeJammer.Trip > 0) {
                events
                    .Append(new PointsAwardedEvent(
                        nextTick + Random.Shared.Next(5 * TicksPerSecond, 20 * TicksPerSecond),
                        expectedState,
                        TeamType.Home,
                        CalculateJammerPoints(state.HomeJammer)))
                    .ToArray();
            }
            if (state.AwayJammer.Trip > 0) {
                events
                    .Append(new PointsAwardedEvent(
                        nextTick + Random.Shared.Next(5 * TicksPerSecond, 20 * TicksPerSecond),
                        expectedState,
                        TeamType.Away,
                        CalculateJammerPoints(state.AwayJammer)))
                    .ToArray();
            }

            if (periodClock > 0) {

                events = events
                    .Append(
                        new JamAdvancedEvent(
                            nextTick + 2 * TicksPerSecond,
                            expectedState
                        ))
                    .ToArray();

                return new LineupInProgressGameState(
                    state.Game,
                    nextTick,
                    events,
                    boxedSkaters,
                    periodClock,
                    true,
                    state.PeriodNumber,
                    state.HomeTimeoutsTaken,
                    state.AwayTimeoutsTaken,
                    nextTick
                );
            } else {
                Console.WriteLine($"Period clock expired following jam in period {state.PeriodNumber}");
                if (state.PeriodNumber == 1) {
                    return new IntermissionInProgressGameState(
                        state.Game,
                        nextTick,
                        events
                            .Append(
                                new JamAdvancedEvent(
                                    nextTick + 2 * TicksPerSecond,
                                    expectedState
                                ))
                            .ToArray(),
                        boxedSkaters,
                        state.HomeTimeoutsTaken,
                        state.AwayTimeoutsTaken,
                        nextTick
                    );
                } else {
                    return new PostGameGameState(state.Game, nextTick, state.Events);
                }
            }
        }

        var lead = state.LeadJammerTeam;

        var homeTripCompleted = state.HomeJammer.Progress <= 20.0f && homeJammerProgress > 20.0f;
        if (homeTripCompleted) {
            if (state.HomeJammer.Trip == 0) {
                if (lead is None) {
                    lead = Some(TeamType.Home);
                    events = events.Append(new LeadEarnedEvent(nextTick, expectedState, TeamType.Home)).ToArray();
                } else {
                    events = events.Append(new InitialTripCompletedEvent(nextTick, expectedState, TeamType.Home)).ToArray();
                }
            } else {
                events = events.Append(new PointsAwardedEvent(nextTick, expectedState, TeamType.Home, 4)).ToArray();
            }
        }
        var homeJammer = new JammerState(
            homeJammerProgress, 
            state.HomeJammer.EligibleForLead && !homeTripCompleted && !boxedSkaters.Any(s => s.Id == state.HomeTeam.Jammer.Id),
            state.HomeJammer.Trip + (homeTripCompleted ? 1 : 0)
        );

        var awayTripCompleted = state.AwayJammer.Progress <= 20.0f && awayJammerProgress > 20.0f;
        if (awayTripCompleted) {
            if (state.AwayJammer.Trip == 0) {
                if (lead is None) {
                    lead = Some(TeamType.Away);
                    events = events.Append(new LeadEarnedEvent(nextTick, expectedState, TeamType.Away)).ToArray();
                } else {
                    events = events.Append(new InitialTripCompletedEvent(nextTick, expectedState, TeamType.Away)).ToArray();
                }
            } else {
                events = events.Append(new PointsAwardedEvent(nextTick, expectedState, TeamType.Away, 4)).ToArray();
            }
        }
        var awayJammer = new JammerState(
            awayJammerProgress,
            state.AwayJammer.EligibleForLead && !awayTripCompleted && !boxedSkaters.Any(s => s.Id == state.AwayTeam.Jammer.Id),
            state.AwayJammer.Trip + (awayTripCompleted ? 1 : 0)
        );

        if (state.LeadJammerTeam is Some<TeamType> currentLead)
        {
            if (
                currentLead.Value == TeamType.Home && boxedSkaters.Any(s => s.Id == state.HomeTeam.Jammer.Id)
                || currentLead.Value == TeamType.Away && boxedSkaters.Any(s => s.Id == state.AwayTeam.Jammer.Id)
            ) {
                events = events.Append(new LeadLostEvent(nextTick, expectedState, currentLead.Value)).ToArray();
                lead = None<TeamType>();
            } else {
                lead = currentLead;
            }

            if (lead is Some<TeamType> s) {
                if (
                    (s.Value == TeamType.Home && homeTripCompleted)
                    || (s.Value == TeamType.Away && awayTripCompleted)) {
                        if (Random.Shared.Next(3) == 0) {
                            var lineupState = new ExpectedState(
                                new Clock(state.PeriodClock / TicksPerSecond, true),
                                new Clock(0, false),
                                new Clock(0, false),
                                new Clock(0, true),
                                new Clock(0, false),
                                state.PeriodNumber
                            );

                            int GetRandomPointsAwardedTick() => nextTick + Random.Shared.Next(2 * TicksPerSecond, 5 * TicksPerSecond);
                            var homePointsAwardedTick = GetRandomPointsAwardedTick();
                            var awayPointsAwardedTick = GetRandomPointsAwardedTick();

                            events = events
                                .Append(new JamCalledEvent(nextTick, lineupState))
                                .Append(new PointsAwardedEvent(
                                    homePointsAwardedTick,
                                    lineupState with { 
                                        PeriodClock = new Clock(Math.Max(0, state.PeriodClock - homePointsAwardedTick) / TicksPerSecond, true),
                                        LineupClock = new Clock(homePointsAwardedTick / TicksPerSecond, true),
                                    },
                                    TeamType.Home,
                                    CalculateJammerPoints(homeJammer)))
                                .Append(new PointsAwardedEvent(
                                    awayPointsAwardedTick,
                                    lineupState with { 
                                        PeriodClock = new Clock(Math.Max(0, state.PeriodClock - awayPointsAwardedTick) / TicksPerSecond, true),
                                        LineupClock = new Clock(awayPointsAwardedTick / TicksPerSecond, true),
                                    },
                                    TeamType.Away,
                                    CalculateJammerPoints(awayJammer)))
                                .Append(new JamAdvancedEvent(
                                    nextTick + 2 * TicksPerSecond,
                                    lineupState with {
                                        PeriodClock = new Clock(Math.Max(0, state.PeriodClock - (nextTick / TicksPerSecond + 2)), true),
                                        LineupClock = new Clock(2, true),
                                    }))
                                .ToArray();

                            return new LineupInProgressGameState(
                                state.Game,
                                nextTick,
                                events,
                                boxedSkaters,
                                state.PeriodClock,
                                true,
                                state.PeriodNumber,
                                state.HomeTimeoutsTaken,
                                state.AwayTimeoutsTaken,
                                nextTick
                            );
                        }
                }
            }
        }

        return state with {
            Tick = nextTick,
            Events = events,
            PenaltyBox = boxedSkaters,
            PeriodClock = periodClock,
            HomeJammer = homeJammer,
            AwayJammer = awayJammer,
            LeadJammerTeam = lead,
        };
    }

    private static GameState TickTimeout(TimeoutInProgressGameState state) {
        var nextTick = state.Tick + 1;
        var timeoutDuration = nextTick - state.StartTick;

        if (timeoutDuration == TimeoutLengthInTicks) {
            var events = state.Events
                .Append(
                    new LineupClockStartedEvent(
                        nextTick,
                        new ExpectedState(
                            new Clock(state.PeriodClock / TicksPerSecond, false),
                            new Clock(0, false),
                            new Clock(TimeoutLengthInTicks / TicksPerSecond, false),
                            new Clock(0, true),
                            new Clock(0, false),
                            state.PeriodNumber
                        )
                    ))
                .ToArray();

            return new LineupInProgressGameState(
                state.Game,
                nextTick,
                events,
                state.PenaltyBox,
                state.PeriodClock,
                false,
                state.PeriodNumber,
                state.HomeTimeoutsTaken,
                state.AwayTimeoutsTaken,
                nextTick
            );
        }

        return state with { Tick = nextTick };
    }

    private static GameState TickIntermission(IntermissionInProgressGameState state) {
        var nextTick = state.Tick + 1;
        var intermissionDuration = nextTick - state.StartTick;

        if (intermissionDuration > IntermissionLengthInTicks) {
            return new LineupInProgressGameState(
                state.Game,
                nextTick,
                state.Events
                    .Append(
                        new LineupClockStartedEvent(
                            nextTick,
                            new ExpectedState(
                                new Clock(0, false),
                                new Clock(0, false),
                                new Clock(0, false),
                                new Clock(0, true),
                                new Clock(IntermissionLengthInTicks / TicksPerSecond, false),
                                2
                            )
                        ))
                    .ToArray(),
                state.PenaltyBox,
                PeriodClockLengthInTicks,
                false,
                2,
                state.HomeTimeoutsTaken,
                state.AwayTimeoutsTaken,
                nextTick
            );
        }

        return state with {
            Tick = nextTick,
        };
    }

    private static OnTrackTeam CreateLineup(Team team, BoxedSkater[] penaltyBox) {
        var skatersInBox = penaltyBox.Where(p => team.Skaters.Any(s => s.Id == p.Id)).ToArray();
        var skatersInUse = new List<Guid>();

        Skater SkaterForPosition(Position position) =>
            skatersInBox
                .Where(s => s.Position == position)
                .Where(s => !skatersInUse.Contains(s.Id))
                .Append(GetRandomSkater(
                    team.Skaters
                        .Where(s => !skatersInBox.Any(b => b.Id == s.Id))
                        .Where(s => !skatersInUse.Contains(s.Id)),
                        position))
                .First()
                .Tee(s => skatersInUse.Add(s.Id));
        
        var jammer = SkaterForPosition(Position.Jammer);
        var pivot = SkaterForPosition(Position.Pivot);
        Skater[] blockers = [ SkaterForPosition(Position.Blocker), SkaterForPosition(Position.Blocker), SkaterForPosition(Position.Blocker) ];

        if(skatersInUse.Distinct().Count() != 5) throw new Exception();

        return new OnTrackTeam(jammer, pivot, blockers);
    }

    private static Skater GetRandomSkater(IEnumerable<Skater> skaters, Position position) =>
        skaters.OrderBy(s => s.FavoredPosition == position ? 1 : 0).ToArray().RandomFavorStart()!;

    private static string GetRandomPenaltyCode() =>
        new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "L", "M", "N", "P", "X" }.Random()!;
}