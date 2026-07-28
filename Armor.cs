public class Armor
{
    public string Type { get; set; }
    public int Protection { get; set; }

    public Armor(string type, int protection)
    {
        Type = type;
        Protection = protection;
    }
}
