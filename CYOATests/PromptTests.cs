[TestClass]
public class PromptTests
{
    private Messages CreateEnglishMessages()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        return messages;
    }

    // Drives Prompt with a scripted sequence of answers and records everything
    // written, so no test ever touches the real console.
    private sealed class ScriptedConsole
    {
        private readonly Queue<string> _answers;
        public List<string> Written { get; } = new();

        public ScriptedConsole(params string[] answers) => _answers = new Queue<string>(answers);

        public string? ReadLine() => _answers.Count > 0 ? _answers.Dequeue() : null;
        public void WriteLine(string text) => Written.Add(text);
        public void Write(string text) => Written.Add(text);
    }

    private Prompt CreatePrompt(ScriptedConsole console)
    {
        return new Prompt(CreateEnglishMessages(), console.ReadLine, console.WriteLine, console.Write);
    }

    private static readonly Dictionary<string, string> AttackOrRetreat = new()
    {
        ["a"] = "attack",
        ["attack"] = "attack",
        ["r"] = "retreat",
        ["retreat"] = "retreat"
    };

    // ── AskChoice ────────────────────────────────────────────────────────────

    [TestMethod]
    public void AskChoice_ShortFormAndFullWord_ReturnTheSameAnswer()
    {
        Assert.AreEqual("attack", CreatePrompt(new ScriptedConsole("a")).AskChoice("attack_prompt", AttackOrRetreat));
        Assert.AreEqual("attack", CreatePrompt(new ScriptedConsole("attack")).AskChoice("attack_prompt", AttackOrRetreat));
        Assert.AreEqual("retreat", CreatePrompt(new ScriptedConsole("r")).AskChoice("attack_prompt", AttackOrRetreat));
    }

    [TestMethod]
    public void AskChoice_AcceptsUppercaseAndPaddedInput()
    {
        Assert.AreEqual("attack", CreatePrompt(new ScriptedConsole("  ATTACK  ")).AskChoice("attack_prompt", AttackOrRetreat));
    }

    [TestMethod]
    public void AskChoice_KeepsAskingUntilAnAnswerIsAcceptable()
    {
        var console = new ScriptedConsole("banana", "", "42", "r");

        string answer = CreatePrompt(console).AskChoice("attack_prompt", AttackOrRetreat);

        Assert.AreEqual("retreat", answer);
    }

    [TestMethod]
    public void AskChoice_WritesTheInvalidMessageBeforeAskingAgain()
    {
        Messages messages = CreateEnglishMessages();
        var console = new ScriptedConsole("nonsense", "a");

        CreatePrompt(console).AskChoice("attack_prompt", AttackOrRetreat);

        CollectionAssert.Contains(console.Written, messages.GetMessage("invalid"));
        // Asked twice: once before the bad answer, once after.
        Assert.AreEqual(2, console.Written.FindAll(w => w == messages.GetMessage("attack_prompt")).Count);
    }

    [TestMethod]
    public void AskChoice_UsesTheInvalidMessageTheCallerAsksFor()
    {
        Messages messages = CreateEnglishMessages();
        var console = new ScriptedConsole("nonsense", "a");

        CreatePrompt(console).AskChoice("path_prompt_full", AttackOrRetreat, "path_invalid");

        CollectionAssert.Contains(console.Written, messages.GetMessage("path_invalid"));
        CollectionAssert.DoesNotContain(console.Written, messages.GetMessage("invalid"));
    }

    [TestMethod]
    public void AskChoice_WritesTheEntryPromptWhenTheCallerAsksForOne()
    {
        Messages messages = CreateEnglishMessages();
        var console = new ScriptedConsole("a");

        CreatePrompt(console).AskChoice("attack_prompt", AttackOrRetreat, "invalid", "enter_choice");

        CollectionAssert.Contains(console.Written, messages.GetMessage("enter_choice"));
    }

    // ── AskNumber ────────────────────────────────────────────────────────────

    [TestMethod]
    public void AskNumber_ReturnsTheChosenIndex()
    {
        Assert.AreEqual(1, CreatePrompt(new ScriptedConsole("1")).AskNumber("menu", 2));
        Assert.AreEqual(2, CreatePrompt(new ScriptedConsole("  2 ")).AskNumber("menu", 2));
    }

    [TestMethod]
    public void AskNumber_RejectsOutOfRangeAndNonNumericInput()
    {
        var console = new ScriptedConsole("0", "-1", "3", "abc", "", "2");

        Assert.AreEqual(2, CreatePrompt(console).AskNumber("menu", 2));
    }

    [TestMethod]
    public void AskNumber_HandlesAMenuWhoseLengthVaries()
    {
        Assert.AreEqual(4, CreatePrompt(new ScriptedConsole("5", "4")).AskNumber("menu", 4));
    }

    // ── AskText ──────────────────────────────────────────────────────────────

    [TestMethod]
    public void AskText_ReturnsTheAnswerTheValidatorAccepts()
    {
        var console = new ScriptedConsole("", "   ", "Aragorn");

        string answer = CreatePrompt(console).AskText("name_prompt", a => !string.IsNullOrWhiteSpace(a));

        Assert.AreEqual("Aragorn", answer);
    }

    // A character name must survive exactly as typed.
    [TestMethod]
    public void AskText_PreservesTheCapitalisationThePlayerTyped()
    {
        var console = new ScriptedConsole("  McTavish  ");

        string answer = CreatePrompt(console).AskText("name_prompt", _ => true);

        Assert.AreEqual("McTavish", answer);
    }

    [TestMethod]
    public void AskText_WritesTheInvalidMessageTheCallerAsksFor()
    {
        Messages messages = CreateEnglishMessages();
        var console = new ScriptedConsole("", "Aragorn");

        CreatePrompt(console).AskText("name_prompt", a => a.Length > 0, "name_invalid");

        CollectionAssert.Contains(console.Written, messages.GetMessage("name_invalid"));
    }
}
