using System.Text.Json.Serialization;

namespace RtNeat;

public class Species
{
    [JsonInclude] public List<Guid> Members { get; set; }
    [JsonInclude] public string Name { get; set; }
    public int Count => Members.Count;
    
    [JsonInclude] public Guid UUID { get; set; }
    
    private static readonly HashSet<string> ExistingNames = new();
    public Species(string? name = null)
    {
        Members = new List<Guid>();
        Name = name ?? RandomName(3);
        ExistingNames.Add(Name);
        UUID = Guid.NewGuid();
    }
    
    [JsonConstructor] 
    private Species(string name, List<Guid> members, Guid uuid)
    {
        Name = name;
        ExistingNames.Add(Name);
        Members = members;
        UUID = uuid;
    }

    private string RandomName(int syllableCount)
    {
        string name = "";
        var random = new Random();
        int maxTries = 3;
        do {
            string[] vowels = { "a", "e", "i", "o", "u" };
            string[] consonants =
            {
                "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z"
            };

            string tmp = "";
            for (int i = 0; i < syllableCount; i++)
            {
                tmp += consonants[random.Next(consonants.Length)];
                tmp += vowels[random.Next(vowels.Length)];
                if (i == syllableCount - 1 && random.Next(2) == 1)
                {
                    tmp += consonants[random.Next(consonants.Length)];
                }
            }

            name = char.ToUpper(tmp[0]) + tmp.Substring(1);
            maxTries--;
        } while (ExistingNames.Contains(name) && maxTries > 0);
        if (maxTries == 0)
        {
            name = RandomName(syllableCount + 1);
        }
        return name;
    }

    public void AddMember(Guid network)
    {
        Members.Add(network);
    }
    
    public void RemoveMember(Guid network)
    {
        Members.Remove(network);
    }
    
    public override string ToString()
    {
        return $"{Name}: {Count}";
    }
}