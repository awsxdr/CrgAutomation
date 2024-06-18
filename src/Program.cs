using CrgAutomation;
using PuppeteerSharp;

Console.WriteLine("Creating game");

var game = GameFactory.CreateRandom();

Console.WriteLine("Creating statsbook");
File.Delete("test.xlsx");
StatsbookPreparer.PrepareStats("./wftda-statsbook-full-A4.xlsx", game, "test.xlsx");

Console.WriteLine("Simulating game");
var runner = new GameSimulator(game);
var events = await runner.Run();

Console.WriteLine("Downloading browser");
var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();

Console.WriteLine("Launching browser");
var browser = await Puppeteer.LaunchAsync(new LaunchOptions {
    Headless = true,
    HeadlessMode = HeadlessMode.True,
    IgnoreHTTPSErrors = true,
    Args = [ "--no-sandbox"],
});

var baseScoreboardUrl = Environment.GetEnvironmentVariable("scoreboard-url") ?? "http://localhost:8000";

var automator = new ScoreboardAutomator(baseScoreboardUrl, browser);
Console.WriteLine("Loading statsbook into scoreboard");
await automator.LoadStatsBook("test.xlsx");

Console.WriteLine("Starting game on scoreboard");
await automator.StartLoadedGame();

Console.WriteLine("Running game");
var gameRunner = new GameRunner(automator, events);
await gameRunner.Run();

await browser.CloseAsync();