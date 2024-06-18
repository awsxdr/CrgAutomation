namespace CrgAutomation;

public static class CollectionExtensionMethods {
    public static T? Random<T>(this ICollection<T> collection) =>
        collection.Any()
            ? collection.ElementAt(System.Random.Shared.Next(collection.Count))
            : default;

    public static T? RandomFavorStart<T>(this ICollection<T> collection) =>
        Enumerable.Range(1, collection.Count)
            .Reverse()
            .Zip(collection)
            .SelectMany(x => Enumerable.Repeat(x.Second, x.First))
            .ToArray()
            .Random();
}
