namespace CrgAutomation;

using PuppeteerSharp;

public class ScoreboardAutomator {
	private readonly string _baseUrl;
	private readonly IBrowser _browser;
	private readonly Task<IPage> _dataPage;
    private readonly Task<IPage> _operatorPage;
    private readonly Task<IPage> _pltHomePage;
    private readonly Task<IPage> _pltAwayPage;
	
	public ScoreboardAutomator(string baseUrl, IBrowser browser) {
		_baseUrl = baseUrl;
		_browser = browser;

        _dataPage = _browser.NewPageAsync();
        _operatorPage = _browser.NewPageAsync();
        _pltHomePage = _browser.NewPageAsync();
        _pltAwayPage = _browser.NewPageAsync();
	}
	
	public async Task LoadStatsBook(string path) {
        var dataPage = await _dataPage;

		await dataPage.GoToAsync($"{_baseUrl}/settings/sb_data/");
		var input = await dataPage.QuerySelectorAsync("input[type='file']");
		await input.UploadFileAsync(path);
		
		var uploadStatsbookButton = await dataPage.WaitForSelectorAsync("form>button:nth-of-type(2)");

        await dataPage.BringToFrontAsync();
		await uploadStatsbookButton.ClickAsync();
	}

	public async Task StartLoadedGame()
	{
        var operatorPage = await _operatorPage;

        await operatorPage.BringToFrontAsync();

        if(!operatorPage.Url.StartsWith($"{_baseUrl}/nso/sbo/")) {
		    await operatorPage.GoToAsync($"{_baseUrl}/nso/sbo/");
        
            var dialogTitleBar = (await operatorPage.QuerySelectorAllAsync(".ui-dialog-titlebar")).Single(b => b.GetPropertyAsync("innerText").Result.RemoteObject.Value.ToString().StartsWith("Operator Login"));
            var closeButton = await dialogTitleBar.QuerySelectorAsync("button");
            await closeButton.ClickAsync();
        }
		
		var startButton = await operatorPage.QuerySelectorAsync("#GameControl");
		await startButton.ClickAsync();
		
		var gameSelect = await operatorPage.QuerySelectorAsync(".Game");
		
        var gameOption = await gameSelect.WaitForSelectorAsync("option:nth-of-type(2)");
		
		var gameValue = (await gameOption.GetPropertyAsync("value")).RemoteObject.Value.ToString();
		
		await gameSelect.SelectAsync(gameValue);
		
		var startGameButton = await operatorPage.QuerySelectorAsync(".StartGame");
		await startGameButton.ClickAsync();
		
		var advanceGameButton = await operatorPage.QuerySelectorAsync("#gameAdvance");
		await advanceGameButton.ClickAsync();

        var pltHome = await _pltHomePage;
        await pltHome.GoToAsync(_baseUrl);

        var pltLink = await pltHome.QuerySelectorAsync(".curGame[href*='nso/plt']");
        await pltHome.BringToFrontAsync();
        await pltLink.ClickAsync();

        while(!pltHome.Url.Contains("?")) {
            await Task.Delay(1);
        }
        var pltAddress = pltHome.Url;

        await pltHome.GoToAsync($"{pltAddress}&team=1");
        var enablePltButton = await pltHome.WaitForSelectorAsync("div#UseLTDialog~.ui-dialog-buttonpane button", new WaitForSelectorOptions { Timeout = 3000});
        if(enablePltButton is not null) {
            await enablePltButton.ClickAsync();
        }

        var pltAway = await _pltAwayPage;
        await pltAway.GoToAsync($"{pltAddress}&team=2");
	}

    public async Task StartLineup() {
        Console.WriteLine("Starting lineup");
        
        var operatorPage = await _operatorPage;

        var startLineupButton = await operatorPage.QuerySelectorAsync("#StopJam");
        await operatorPage.BringToFrontAsync();
        await startLineupButton.ClickAsync();
    }

    public async Task StartJam() {
        Console.WriteLine("Starting jam");

        var operatorPage = await _operatorPage;

        var startJamButton = await operatorPage.QuerySelectorAsync("#StartJam");
        await operatorPage.BringToFrontAsync();
        await startJamButton.ClickAsync();
    }

    public async Task EndJam() {
        Console.WriteLine("Ending jam");
        
        var operatorPage = await _operatorPage;

        var endJamButton = await operatorPage.QuerySelectorAsync("#StopJam");
        await operatorPage.BringToFrontAsync();
        await endJamButton.ClickAsync();
    }

    public async Task SetLead(TeamType team) {
        Console.WriteLine($"Setting lead to {team}");
        
        var operatorPage = await _operatorPage;

        var leadButton = await operatorPage.QuerySelectorAsync(team == TeamType.Home ? "#Team1Lead" : "#Team2Lead");
        await operatorPage.BringToFrontAsync();
        await leadButton.ClickAsync();
    }

    public async Task SetLost(TeamType team) {
        Console.WriteLine($"Setting lost for {team}");
        
        var operatorPage = await _operatorPage;

        var lostButton = await operatorPage.QuerySelectorAsync(team == TeamType.Home ? "#Team1Lost" : "#Team2Lost");
        await operatorPage.BringToFrontAsync();
        await lostButton.ClickAsync();
    }

    public async Task AdvanceTrip(TeamType team) {
        Console.WriteLine($"Advancing trip for {team}");

        var operatorPage = await _operatorPage;

        var advanceTripButton = await operatorPage.QuerySelectorAsync(team == TeamType.Home ? "#Team1AddTrip" : "#Team2AddTrip");
        await operatorPage.BringToFrontAsync();
        await advanceTripButton.ClickAsync();
    }

    public async Task AddToLineup(string number, Position position, TeamType team) {
        Console.WriteLine($"Lining up {number} for {team} as {position}");

        var pltPage = await (team == TeamType.Home ? _pltHomePage : _pltAwayPage);

        var skaterRow = await pltPage.QuerySelectorAsync($"tr[number='{number}']");
        var positionButton = await skaterRow.QuerySelectorAsync($".Role.{position}");
        await pltPage.BringToFrontAsync();

        await positionButton.ClickAsync();
    }

    public async Task AdvancePltJam() {
        Console.WriteLine("Advancing jam");

        async Task AdvancePage(IPage page) {
            var advanceButton = await page.QuerySelectorAsync(".Advance.Active");
            await page.BringToFrontAsync();

            await advanceButton.ClickAsync();
        }

        await AdvancePage(await _pltHomePage);
        await AdvancePage(await _pltAwayPage);
    }

    public async Task SetTripPoints(TeamType team, int points) {
        Console.WriteLine($"Setting trip points for {team} to {points}");

        var operatorPage = await _operatorPage;

        var pointsButton = await operatorPage.QuerySelectorAsync($"#Team{(team == TeamType.Home ? 1 : 2)}TripScore{points}");
        await operatorPage.BringToFrontAsync();
        await pointsButton.ClickAsync();
    }

    public async Task AddPenalty(string number, TeamType team, string penalty) {
        Console.WriteLine($"Adding penalty {penalty} to {number} on {team} team");

        var pltPage = await (team == TeamType.Home ? _pltHomePage : _pltAwayPage);

        var skaterNumberButton = await pltPage.QuerySelectorAsync($"tr[number='{number}']>.Number");
        await pltPage.BringToFrontAsync();
        await skaterNumberButton.ClickAsync();

        var penaltyButton = await pltPage.WaitForSelectorAsync($"div[code='{penalty}']");
        await penaltyButton.ClickAsync();
    }

    public async Task ToggleBox(string number, TeamType team) {
        Console.WriteLine($"Toggling box status for {number} on {team}");

        var pltPage = await (team == TeamType.Home ? _pltHomePage : _pltAwayPage);

        var boxButton = await pltPage.QuerySelectorAsync($"tr[number='{number}']>.Sitting");
        await pltPage.BringToFrontAsync();
        await boxButton.ClickAsync();
    }

    public async Task TeamTimeout(TeamType team) {
        Console.WriteLine($"Starting timeout for {team} team");

        var operatorPage = await _operatorPage;
        
        var timeoutButton = await operatorPage.QuerySelectorAsync(team == TeamType.Home ? "#Team1Timeout" : "#Team2Timeout");
        await operatorPage.BringToFrontAsync();
        await timeoutButton.ClickAsync();
   }
}