// Asking the player something and insisting on an acceptable answer. Every
// prompt in the game goes through here, so the rules for what counts as an
// answer are decided once rather than sixteen times.
//
// The reader and writers are constructor parameters defaulting to the console,
// which is what makes prompting testable: a test scripts the answers and reads
// back what was written, exactly as an injected Die makes a roll testable.
//
// Each method comes in two forms. The key form names a message to print. The
// lines form takes text already rendered, for menus built from current state
// like the Profile list and the merchant shop.
public class Prompt
{
    private readonly Messages _messages;
    private readonly Func<string?> _readLine;
    private readonly Action<string> _writeLine;
    private readonly Action<string> _write;

    public Prompt(
        Messages messages,
        Func<string?>? readLine = null,
        Action<string>? writeLine = null,
        Action<string>? write = null)
    {
        _messages = messages;
        _readLine = readLine ?? Console.ReadLine;
        _writeLine = writeLine ?? Console.WriteLine;
        _write = write ?? Console.Write;
    }

    // ── Choosing from aliases ────────────────────────────────────────────────
    // Several inputs can mean the same thing: 'n' and 'north', 'a' and 'attack'.

    public string AskChoice(
        string promptKey,
        IReadOnlyDictionary<string, string> aliases,
        string invalidKey = "invalid",
        string? entryKey = null)
        => AskChoice(new[] { _messages.GetMessage(promptKey) }, aliases, invalidKey, entryKey);

    public string AskChoice(
        IReadOnlyList<string> lines,
        IReadOnlyDictionary<string, string> aliases,
        string invalidKey = "invalid",
        string? entryKey = null)
    {
        while (true)
        {
            string answer = Ask(lines, entryKey).ToLower();

            if (aliases.TryGetValue(answer, out string? canonical))
                return canonical;

            _writeLine(_messages.GetMessage(invalidKey));
        }
    }

    // ── Choosing a number from a menu ────────────────────────────────────────

    public int AskNumber(
        string promptKey,
        int count,
        string invalidKey = "invalid",
        string? entryKey = null)
        => AskNumber(new[] { _messages.GetMessage(promptKey) }, count, invalidKey, entryKey);

    public int AskNumber(
        IReadOnlyList<string> lines,
        int count,
        string invalidKey = "invalid",
        string? entryKey = null)
    {
        while (true)
        {
            string answer = Ask(lines, entryKey);

            if (int.TryParse(answer, out int choice) && choice >= 1 && choice <= count)
                return choice;

            _writeLine(_messages.GetMessage(invalidKey));
        }
    }

    // ── Free text ────────────────────────────────────────────────────────────
    // Never lowercased, so a character name keeps the capitalisation typed.

    public string AskText(
        string promptKey,
        Func<string, bool> isAcceptable,
        string invalidKey = "invalid",
        string? entryKey = null)
    {
        var lines = new[] { _messages.GetMessage(promptKey) };

        while (true)
        {
            string answer = Ask(lines, entryKey);

            if (isAcceptable(answer))
                return answer;

            _writeLine(_messages.GetMessage(invalidKey));
        }
    }

    // ── Yes or no ────────────────────────────────────────────────────────────

    private static readonly Dictionary<string, string> YesNo = new()
    {
        ["y"] = "yes",
        ["n"] = "no"
    };

    public bool AskYesNo(string promptKey, string invalidKey = "invalid")
        => AskChoice(promptKey, YesNo, invalidKey) == "yes";

    public bool AskYesNo(IReadOnlyList<string> lines, string invalidKey = "invalid")
        => AskChoice(lines, YesNo, invalidKey) == "yes";

    // The entry prompt is written without a newline, matching how the game
    // already puts "Enter choice: " on the same line as the answer.
    private string Ask(IReadOnlyList<string> lines, string? entryKey)
    {
        foreach (string line in lines)
            _writeLine(line);

        if (entryKey != null)
            _write(_messages.GetMessage(entryKey));

        return (_readLine() ?? "").Trim();
    }
}
