namespace HttTailor.Utilities;

public static class RandomNames
{
    private static readonly Random Rand = new();
    
    private static readonly string[] Adjectives = 
    {
        "smiling", "curious", "grooming", "happy", "tired",
        "energetic", "lazy", "thoughtful", "playful", "cheerful",
        "brave", "friendly", "clever", "adorable", "graceful",
        "shy", "bold", "calm", "eager", "fearless",
        "gentle", "humble", "innocent", "jovial", "keen",
        "loyal", "merry", "noble", "optimistic", "patient",
        "quiet", "resourceful", "sincere", "tenacious", "unique",
        "vibrant", "warm", "funny", "youthful", "zealous",
        "agile", "bright", "confident", "dynamic", "efficient"
    };
    
    private static readonly string[] Animals = 
    {
        "python", "koala", "grizzly", "cat", "dog",
        "elephant", "giraffe", "penguin", "kangaroo", "lion",
        "tiger", "zebra", "rhino", "hippo", "raccoon",
        "squirrel", "bear", "fox", "wolf", "antelope",
        "buffalo", "chimpanzee", "deer", "elk", "falcon",
        "gorilla", "horse", "iguana", "jaguar", "kudu",
        "lemur", "moose", "narwhal", "ocelot", "panther",
        "quokka", "rat", "seal", "toucan", "urutu",
        "viper", "whale", "xerus", "yak", "zebu",
        "alpaca", "bobcat", "capybara", "dingo", "emu"
    };

    public static string Generate()
    {
        
#if NETCOREAPP
        var adjectiveInd = Random.Shared.Next(Adjectives.Length);
        var animalInd = Random.Shared.Next(Animals.Length);
#else
        var adjectiveInd = Rand.Next(Adjectives.Length);
        var animalInd = Rand.Next(Animals.Length);
#endif
        return $"{Adjectives[adjectiveInd]}_{Animals[animalInd]}";
    }

}