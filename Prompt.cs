// Asking the player something and insisting on an acceptable answer. Every
// prompt in the game goes through here, so the rules for what counts as an
// answer are decided once rather than sixteen times.
//
// The reader and writers are constructor parameters defaulting to the console,
// which is what makes prompting testable: a test scripts the answers and reads
// back what was written, exactly as an injected Die makes a roll testable.
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

    // Several inputs can mean the same thing: 'n' and 'north', 'a' and 'attack'.
    // The caller supplies that map and gets the canonical answer back.
    public string AskChoice(
        string promptKey,
        IReadOnlyDictionary<string, string> aliases,
        string invalidKey = "invalid",
        string? entryKey = null)
    {
        while (true)
        {
            string answer = Ask(promptKey, entryKey).ToLower();

            if (aliases.TryGetValue(answer, out string? canonical))
                return canonical;

            _writeLine(_messages.GetMessage(invalidKey));
        }
    }

    // Menus whose length varies: the main menu, the Profile list, the shop.
    // Returns the number the player chose, counting from one.
    public int AskNumber(
        string promptKey,
        int count,
        string invalidKey = "invalid",
        string? entryKey = null)
    {
        while (true)
        {
            string answer = Ask(promptKey, entryKey);

            if (int.TryParse(answer, out int choice) && choice >= 1 && choice <= count)
                return choice;

            _writeLine(_messages.GetMessage(invalidKey));
        }
    }

    // Free text the caller judges. Never lowercased, so a character name keeps
    // the capitalisation the player typed.
    public string AskText(
        string promptKey,
        Func<string, bool> isAcceptable,
        string invalidKey = "invalid",
        string? entryKey = null)
    {
        while (true)
        {
            string answer = Ask(promptKey, entryKey);

            if (isAcceptable(answer))
                return answer;

            _writeLine(_messages.GetMessage(invalidKey));
        }
    }

    // Yes or no, in the y/n form the game already uses everywhere.
    public bool AskYesNo(string promptKey, string invalidKey = "invalid")
    {
        return AskChoice(promptKey, YesNo, invalidKey) == "yes";
    }

    private static readonly Dictionary<string, string> YesNo = new()
    {
        ["y"] = "yes",
        ["n"] = "no"
    };

    // The entry prompt is written without a newline, matching how the game
    // already puts "Enter choice: " on the same line as the answer.
    private string Ask(string promptKey, string? entryKey)
    {
        _writeLine(_messages.GetMessage(promptKey));

        if (entryKey != null)
            _write(_messages.GetMessage(entryKey));

        return (_readLine() ?? "").Trim();
    }
}
