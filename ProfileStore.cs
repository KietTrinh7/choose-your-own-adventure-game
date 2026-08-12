using System.Text.Json;
using System.Text.Encodings.Web;

// Reads and writes Profiles: one saved character per character name, held in a
// single JSON document. The directory is injected so tests can point at a
// temporary path instead of the player's real data folder (see ADR-0002 for the
// same injection idiom used for Die).
public class ProfileStore
{
    private const string StoreFileName = "profiles.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        // Weapon art contains angle brackets; the default encoder escapes them,
        // which round-trips correctly but makes the file unreadable by hand.
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    // Deliberately not the application base directory used for the language
    // data: that resolves to the build output, which a clean or rebuild wipes.
    public static string DefaultDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "CYOA");

    private readonly string _directory;

    public ProfileStore(string directory)
    {
        _directory = directory;
    }

    private string StorePath => Path.Combine(_directory, StoreFileName);

    public void Save(Player player)
    {
        Dictionary<string, Player> profiles = ReadAll();
        profiles[player.Name] = player;

        Directory.CreateDirectory(_directory);
        File.WriteAllText(StorePath, JsonSerializer.Serialize(profiles, SerializerOptions));
    }

    public Player? Load(string name)
    {
        if (!ReadAll().TryGetValue(name, out Player? player))
            return null;

        // Profiles saved before maximum health existed have no value for it, so
        // it deserialises as zero. Treat the health they were saved with as the
        // ceiling rather than leaving a character who can never be healed.
        if (player.MaxHealthPoints <= 0)
            player.MaxHealthPoints = player.HealthPoints;

        return player;
    }

    public List<string> ListNames()
    {
        return ReadAll().Keys.ToList();
    }

    public bool Exists(string name)
    {
        return ReadAll().ContainsKey(name);
    }

    // A store that exists but cannot be parsed is renamed aside rather than
    // overwritten, so its contents survive for anyone who wants to salvage them.
    // A missing store is the ordinary first run and is not reported.
    public bool SetAsideUnreadableStore()
    {
        if (!File.Exists(StorePath) || CanParseStore())
            return false;

        string quarantined = StorePath + ".unreadable";
        int suffix = 1;
        while (File.Exists(quarantined))
            quarantined = StorePath + ".unreadable." + suffix++;

        File.Move(StorePath, quarantined);
        return true;
    }

    private bool CanParseStore()
    {
        try
        {
            JsonSerializer.Deserialize<Dictionary<string, Player>>(File.ReadAllText(StorePath));
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private Dictionary<string, Player> ReadAll()
    {
        if (!File.Exists(StorePath))
            return new Dictionary<string, Player>();

        try
        {
            string json = File.ReadAllText(StorePath);
            return JsonSerializer.Deserialize<Dictionary<string, Player>>(json)
                   ?? new Dictionary<string, Player>();
        }
        catch (JsonException)
        {
            // Reading never throws: a damaged store simply looks empty until
            // SetAsideUnreadableStore has dealt with it.
            return new Dictionary<string, Player>();
        }
    }
}
