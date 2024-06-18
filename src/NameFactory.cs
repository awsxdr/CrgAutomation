namespace CrgAutomation;

public static class NameFactory {
    public static string GetRandomName() => 
        $"{NameAdjectives.Random()} {NameNouns.Random()}";

    public static string GetRandomPlace() =>
        PlaceNames.Random()!;

    public static string GetRandomColor() =>
        Colors.Random()!;

    private static readonly string[] NameAdjectives = [
        "Abhorrent",
        "Absurd",
        "Abusive",
        "Accidental",
        "Acidic",
        "Adaptable",
        "Adorable",
        "Adventurous",
        "Aggressive",
        "Alert",
        "Alluring",
        "Aloof",
        "Amazing",
        "Ambitious",
        "Amused",
        "Amusing",
        "Ancient",
        "Angry",
        "Animated",
        "Annoyed",
        "Annoying",
        "Anxious",
        "Arrogant",
        "Astonishing",
        "Attractive",
        "Average",
        "Aware",
        "Awesome",
        "Awful",
        "Bad",
        "Barbarous",
        "Bashful",
        "Bawdy",
        "Beautiful",
        "Belligerent",
        "Berserk",
        "Best",
        "Better",
        "Bewildered",
        "Big",
        "Bite-sized",
        "Bitter",
        "Bizarre",
        "Bloody",
        "Bored",
        "Boundless",
        "Brainy",
        "Brave",
        "Brawny",
        "Bright",
        "Broad",
        "Burly",
        "Busy",
        "Calculating",
        "Callous",
        "Calm",
        "Capable",
        "Capricious",
        "Careful",
        "Careless",
        "Cautious",
        "Ceaseless",
        "Charming",
        "Cheerful",
        "Classy",
        "Clever",
        "Complex",
        "Concerned",
        "Confused",
        "Cool",
        "Courageous",
        "Cowardly",
        "Crabby",
        "Cuddly",
        "Cultured",
        "Curious",
        "Cute",
        "Damaging",
        "Dangerous",
        "Dapper",
        "Dashing",
        "Dazzling",
        "Defiant",
        "Delightful",
        "Determined",
        "Dirty",
        "Disgusting",
        "Distinct",
        "Dizzy",
        "Dramatic",
        "Dynamic",
        "Dysfunctional",
        "Eager",
        "Earsplitting",
        "Efficient",
        "Elastic",
        "Elite",
        "Enchanting",
        "Enduring",
        "Energetic",
        "Entertaining",
        "Enthusiastic",
        "Erratic",
        "Evasive",
        "Excellent",
        "Excited",
        "Exuberant",
        "Fabulous",
        "Fair",
        "Fancy",
        "Fantastic",
        "Fast",
        "Fearless",
        "Fierce",
        "Filthy",
        "Fine",
        "Flashy",
        "Flawless",
        "Fluffy",
        "Fluttering",
        "Foolish",
        "Fortunate",
        "Fragile",
        "Frantic",
        "Free",
        "Fresh",
        "Frightening",
        "Funny",
        "Gaudy",
        "Giddy",
        "Gifted",
        "Glamorous",
        "Gleaming",
        "Glorious",
        "Goofy",
        "Gorgeous",
        "Graceful",
        "Grandiose",
        "Great",
        "Groovy",
        "Grouchy",
        "Gruesome",
        "Grumpy",
        "Handsome",
        "Happy",
        "Hateful",
        "Hilarious",
        "Hissing",
        "Honorable",
        "Horrible",
        "Hot",
        "Illustrious",
        "Imperfect",
        "Important",
        "Impossible",
        "Incredible",
        "Infamous",
        "Innocent",
        "Invincible",
        "Irate",
        "Jazzy",
        "Jolly",
        "Juicy",
        "Keen",
        "Kind",
        "Lavish",
        "Lean",
        "Lethal",
        "Little",
        "Loose",
        "Loud",
        "Lovely",
        "Ludicrous",
        "Lush",
        "Macabre",
        "Magical",
        "Magnificent",
        "Majestic",
        "Malicious",
        "Marvelous",
        "Mean",
        "Mighty",
        "Mysterious",
        "Nasty",
        "Naughty",
        "Nervous",
        "Nice",
        "Nimble",
        "Nippy",
        "Noisy",
        "Noxious",
        "Obnoxious",
        "Obscene",
        "Observant",
        "Odd",
        "Outrageous",
        "Outstanding",
        "Perfect",
        "Plucky",
        "Poised",
        "Powerful",
        "Previous",
        "Pretty",
        "Prickly",
        "Protective",
        "Pumped",
        "Purring",
        "Pushy",
        "Puzzled",
        "Quick",
        "Quiet",
        "Rampant",
        "Rapid",
        "Remarkable",
        "Ritzy",
        "Rotten",
        "Rough",
        "Royal",
        "Ruthless",
        "Salty",
        "Sassy",
        "Scandalous",
        "Scary",
        "Screeching",
        "Selfish",
        "Serious",
        "Shaggy",
        "Shocking",
        "Silly",
        "Skillful",
        "Slippery",
        "Smart",
        "Sneaky",
        "Solid",
        "Sophisticated",
        "Sour",
        "Sparkling",
        "Special",
        "Spectacular",
        "Spicy",
        "Spiffy",
        "Spiky",
        "Spiteful",
        "Splendid",
        "Spooky",
        "Spotless",
        "Steadfast",
        "Steady",
        "Stormy",
        "Strange",
        "Strong",
        "Sturdy",
        "Sudden",
        "Sulky",
        "Super",
        "Superb",
        "Supreme",
        "Swanky",
        "Sweet",
        "Swift",
        "Terrible",
        "Terrific",
        "Thundering",
        "Tough",
        "Towering",
        "Trashy",
        "Tremendous",
        "Tricky",
        "Ultra",
        "Unequaled",
        "Unique",
        "Unruly",
        "Vengeful",
        "Venemous",
        "Victorious",
        "Vigorous",
        "Violent",
        "Volatile",
        "Wacky",
        "Wicked",
        "Wild",
        "Wonderful",
        "Wrathful",
        "Zippy"
    ];

    private static readonly string[] NameNouns = [
        "Aftermath",
        "Animal",
        "Ant",
        "Apple",
        "Banana",
        "Bat",
        "Bean",
        "Bear",
        "Beast",
        "Bee",
        "Beetle",
        "Bird",
        "Blade",
        "Bomb",
        "Brick",
        "Bun",
        "Burst",
        "Butter",
        "Cabbage",
        "Cactus",
        "Cake",
        "Cat",
        "Cemetery",
        "Chain",
        "Cheese",
        "Cherry",
        "Chicken",
        "Cloud",
        "Clover",
        "Competition",
        "Condition",
        "Cook",
        "Cow",
        "Crayon",
        "Creature",
        "Crook",
        "Crow",
        "Crown",
        "Crush",
        "Cub",
        "Death",
        "Deer",
        "Desire",
        "Dinosaur",
        "Disease",
        "Dog",
        "Doll",
        "Donkey",
        "Duck",
        "Dust",
        "Ear",
        "Egg",
        "Engine",
        "Error",
        "Event",
        "Fairy",
        "Feet",
        "Fish",
        "Flesh",
        "Flower",
        "Fly",
        "Fowl",
        "Fox",
        "Frog",
        "Game",
        "Giraffe",
        "Goat",
        "Goose",
        "Grass",
        "Grip",
        "Hammer",
        "Hands",
        "Heart",
        "Hen",
        "Horse",
        "Insect",
        "Instrument",
        "Invention",
        "Jelly",
        "Jewel",
        "Joke",
        "Judge",
        "Kite",
        "Kitten",
        "Kitty",
        "Lettuce",
        "Lizard",
        "Machine",
        "Metal",
        "Mist",
        "Monkey",
        "Moon",
        "Nail",
        "Noise",
        "Owl",
        "Pancake",
        "Party",
        "Person",
        "Pest",
        "Pet",
        "Pickle",
        "Pie",
        "Pig",
        "Pizza",
        "Plant",
        "Popcorn",
        "Potato",
        "Punishment",
        "Purpose",
        "Rabbit",
        "Rain",
        "Rat",
        "Ray",
        "Reaction",
        "Regret",
        "Ring",
        "Rock",
        "Rose",
        "Scarecrow",
        "Scent",
        "Screw",
        "Seed",
        "Sheep",
        "Shock",
        "Skate",
        "Smash",
        "Snake",
        "Song",
        "Spider",
        "Squirrel",
        "Star",
        "Steel",
        "Stranger",
        "Substance",
        "Sugar",
        "Surprise",
        "Tail",
        "Tank",
        "Teeth",
        "Temper",
        "Tendency",
        "Thrill",
        "Thunder",
        "Tiger",
        "Toad",
        "Toy",
        "Train",
        "Trick",
        "Umbrella",
        "Unit",
        "Voice",
        "Volcano",
        "Wave",
        "Wheel",
        "Whip",
        "Whistle",
        "Wing",
        "Wish",
        "Wolf",
        "Wren",
        "Yak",
        "Zebra",
        "Zephyr",
        "Zipper"
    ];

    private static readonly string[] Colors = [
        "Black",
        "Gray",
        "White",
        "Red",
        "Orange",
        "Brown",
        "Yellow",
        "Gold",
        "Lime",
        "Green",
        "Turquoise",
        "Teal",
        "Blue",
        "Purple"
    ];

    private static readonly string[] PlaceNames = [
        "Test City",
        "Test Town",
        "Testshire",
        "Testington",
        "Testford",
        "Testchester",
        "Testville",
        "Example City",
        "Example Town",
        "Exampleshire",
        "Exampleton",
        "Exampleford",
        "Examplechester",
        "Exampleville",
    ];
}