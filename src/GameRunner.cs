using System.Diagnostics;

namespace CrgAutomation;

public class GameRunner {
    private readonly ScoreboardAutomator _automator;
    private readonly GameEvent[] _events;

    public GameRunner(ScoreboardAutomator automator, GameEvent[] events) {
        _automator = automator;
        _events = events;
    }

    public async Task Run() {
        var events = _events.ToList();
        var startTick = DateTime.UtcNow.Ticks;
        int GetCurrentTick() => (int)((DateTime.UtcNow.Ticks - startTick) / (float)TimeSpan.TicksPerSecond * GameSimulator.TicksPerSecond);
        
        while(events.Any()) {
            var @event = events[0];
            
            if(GetCurrentTick() < @event.Tick) {
                await Task.Delay(1);
                continue;
            }

            Console.WriteLine($"Processing event {@event.GetType().Name}. Expected state: {@event.ExpectedState}");
            await ProcessEvent(@event);
            events.RemoveAt(0);
        }
    }

    private Task ProcessEvent(GameEvent @event) =>
        @event switch {
            LineupClockStartedEvent lse => ProcessLineupStartedEvent(lse),
            JamStartedEvent jse => ProcessJamStartedEvent(jse),
            JamCalledEvent jce => ProcessJamCalledEvent(jce),
            LeadEarnedEvent lee => ProcessLeadEarnedEvent(lee),
            InitialTripCompletedEvent itce => ProcessInitialTripCompletedEvent(itce),
            SkaterLinedUpEvent slue => ProcessSkaterLinedUpEvent(slue),
            JamAdvancedEvent jae => ProcessJamAdvancedEvent(jae),
            PointsAwardedEvent pae => ProcessPointsAwardedEvent(pae),
            SkaterSentToBoxEvent sstbe => ProcessSkaterSentToBoxEvent(sstbe),
            SkaterSatInBoxEvent ssibe => ProcessSkaterSatInBoxEvent(ssibe),
            SkaterReleasedFromBoxEvent srfbe => ProcessSkaterReleasedFromBoxEvent(srfbe),
            TimeoutStartedEvent tse => ProcessTimeoutStartedEvent(tse),
            _ => Task.CompletedTask,
        };

    private async Task ProcessLineupStartedEvent(LineupClockStartedEvent @event) {
        await _automator.StartLineup();
    }

    private async Task ProcessJamStartedEvent(JamStartedEvent @event) {
        await _automator.StartJam();
    }

    private async Task ProcessJamCalledEvent(JamCalledEvent @event) {
        await _automator.EndJam();
    }

    private async Task ProcessLeadEarnedEvent(LeadEarnedEvent @event) {
        await _automator.SetLead(@event.Team);
    }

    private async Task ProcessInitialTripCompletedEvent(InitialTripCompletedEvent @event) {
        await _automator.AdvanceTrip(@event.Team);
    }

    private async Task ProcessSkaterLinedUpEvent(SkaterLinedUpEvent @event) {
        await _automator.AddToLineup(@event.Number, @event.Position, @event.Team);
    }

    private async Task ProcessJamAdvancedEvent(JamAdvancedEvent @event) {
        await _automator.AdvancePltJam();
    }

    private async Task ProcessPointsAwardedEvent(PointsAwardedEvent @event) {
        await _automator.SetTripPoints(@event.Team, @event.Points);
    }

    private async Task ProcessSkaterSentToBoxEvent(SkaterSentToBoxEvent @event) {
        await _automator.AddPenalty(@event.SkaterNumber, @event.Team, @event.PenaltyCode);
    }

    private async Task ProcessSkaterSatInBoxEvent(SkaterSatInBoxEvent @event) {
        await _automator.ToggleBox(@event.SkaterNumber, @event.Team);
    }

    private async Task ProcessSkaterReleasedFromBoxEvent(SkaterReleasedFromBoxEvent @event) {
        await _automator.ToggleBox(@event.SkaterNumber, @event.Team);
    }

    private async Task ProcessTimeoutStartedEvent(TimeoutStartedEvent @event) {
        await _automator.TeamTimeout(@event.Team);
    }
}