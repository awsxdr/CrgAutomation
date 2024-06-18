using System.IO.Compression;
using System.Xml.Linq;

namespace CrgAutomation;

public static class StatsbookPreparer {
    public static void PrepareStats(string sourcePath, Game game, string outputPath) {
        File.Copy(sourcePath, outputPath, true);

        using var statsbook = ZipFile.Open(outputPath, ZipArchiveMode.Update);

		var igrf = statsbook.GetEntry("xl/worksheets/sheet2.xml")!;
		XDocument document;

		using (var stream = igrf.Open())
		{
			document = XDocument.Load(stream);
		}

        SetCell(document, 1, 3, "Test Sports Center");
        SetCell(document, 8, 3, "Test");
        SetCell(document, 10, 3, "Testshire");
        SetCell(document, 11, 3, "1");
        SetCell(document, 8, 5, game.HomeTeam.Name);
        SetCell(document, 1, 7, DateTime.Now.ToString("yyyy-MM-dd"));
        SetCell(document, 8, 7, DateTime.Now.ToString("HH:mm"));

		SetRoster(document, game, TeamType.Home);
		SetRoster(document, game, TeamType.Away);

		SetOfficialsRoster(document, game);

		using (var stream = igrf.Open())
		{
			document.Save(stream, SaveOptions.DisableFormatting);
			stream.Flush();
		}
    }

	private static void SetRoster(XDocument document, Game game, TeamType teamType)
	{
		var team = teamType switch
		{
			TeamType.Home => game.HomeTeam,
			TeamType.Away => game.AwayTeam,
            _ => throw new ArgumentException()
        };

        SetCell(document, teamType == TeamType.Home ? 1 : 8, 10, team.Name);
        SetCell(document, teamType == TeamType.Home ? 1 : 8, 11, team.Name);
        SetCell(document, teamType == TeamType.Home ? 1 : 8, 12, team.Color);
		SetCell(document, teamType == TeamType.Home ? 1 : 8, 49, team.Skaters.Random()!.Name);

		for (var i = 0; i < team.Skaters.Length; ++i)
		{
			SetSkater(document, teamType, i, team.Skaters[i].Number, team.Skaters[i].Name);
		}
	}

	private static void SetOfficialsRoster(XDocument document, Game game)
	{
		var officials = game.Officials.ToList();
		string[] targetOrder = [
			"Head Non-Skating Official",
			"Penalty Wrangler",
			"Inside Whiteboard Operator",
			"Jam Timer",
			"Penalty Lineup Tracker",
			"Penalty Lineup Tracker",
			"Scorekeeper",
			"Scorekeeper",
			"ScoreBoard Operator",
			"Penalty Box Manager",
			"Penalty Box Timer",
			"Penalty Box Timer",
			"Non-Skating Official Alternate",
			"Period Timer",
			"",
			"",
			"",
			"",
			"",
			"",
			"Head Referee",
			"Inside Pack Referee",
			"Jammer Referee",
			"Jammer Referee",
			"Outside Pack Referee",
			"Outside Pack Referee",
			"Outside Pack Referee",
			"Referee Alternate",
		];

		string NormalizeRole(string role) => role.ToLowerInvariant().Replace(" ", "").Replace("-", "");

		var orderedOfficials = new List<Official>();

		foreach (var target in targetOrder)
		{
			var normalizedTarget = NormalizeRole(target);
			var matchIndex = officials.FindIndex(o => NormalizeRole(o.Role.ToString()).Equals(normalizedTarget));

			if(matchIndex >= 0)
			{
				orderedOfficials.Add(officials[matchIndex]);
				officials.RemoveAt(matchIndex);
			}
			else
			{
				orderedOfficials.Add(new Official(Guid.NewGuid(), "", false, OfficialRole.JammerReferee));
			}
		}

		if(officials.Count > 5)
		{
			// Unable to find enough matches
			orderedOfficials = game.Officials.ToList();
		}
		else
		{
			var firstBlankTarget = targetOrder.TakeWhile(r => !string.IsNullOrEmpty(r)).Count();
			for(var i = 0; i < officials.Count; ++i)
			{
				orderedOfficials[firstBlankTarget + i] = officials[i];
			}
		}

		for(var i = 0; i < 28; ++i)
		{
            var official = orderedOfficials[i];

            if(!string.IsNullOrEmpty(official.Name)) {
                SetCell(document, 0, 60 + i, official.Role.ToString());
                SetCell(document, 2, 60 + i, official.Name);
                SetCell(document, 7, 60 + i, "Independent");
            }
		}
    }

	private static void SetCell(XDocument document, int column, int row, string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return;

		var ns = document.Root!.Name.Namespace;
		
		var rowString = row.ToString();
		var columnString = GetColumnString(column);

		var cell = document.Root!
			.Element(ns + "sheetData")!
			.Elements(ns + "row")
			.Single(e => e.Attribute("r")!.Value == rowString)
			.Elements(ns + "c")
			.Single(e => e.Attribute("r")!.Value == $"{columnString}{rowString}");

		cell.Attribute("t")?.Remove();

		cell.Add(
			new XAttribute("t", "inlineStr"),
			new XElement(ns + "is",
				new XElement(ns + "t", value)
			)
		);
	}
		
	private static string GetColumnString(int column)
	{
		if(column >= 26)
		{
			return $"{(char)('A' + (column / 26) - 1)}{(char)('A' + column % 26)}";
		}
		return ((char)('A' + column)).ToString();
	}

	private static void SetCellValue(XDocument document, int column, int row, int value)
	{		
		var ns = document.Root!.Name.Namespace;

		var rowString = row.ToString();
		var columnString = GetColumnString(column);

		var cell = document.Root!
			.Element(ns + "sheetData")!
			.Elements(ns + "row")
			.Single(e => e.Attribute("r")!.Value == rowString)
			.Elements(ns + "c")
			.Single(e => e.Attribute("r")!.Value == $"{columnString}{rowString}");

		cell.Add(
			new XElement(ns + "v", value)
		);
	}

	private static void SetSkater(XDocument document, TeamType teamType, int line, string number, string name)
	{
		SetCell(document, teamType == TeamType.Home ? 1 : 8, line + 14, number);
		SetCell(document, teamType == TeamType.Home ? 2 : 9, line + 14, name);
	}
}