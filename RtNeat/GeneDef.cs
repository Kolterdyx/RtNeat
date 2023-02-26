using System.Text.Json.Serialization;

namespace RtNeat;

public struct GeneDef
{
    public long InnovationNumber { get; }
    public GeneType Type { get; }
    public long SourceGene1 { get; }
    public long SourceGene2 { get; }
    
    [JsonConstructor]
    public GeneDef(long innovationNumber, GeneType type, long sourceGene1, long sourceGene2)
    {
        InnovationNumber = innovationNumber;
        Type = type;
        SourceGene1 = sourceGene1;
        SourceGene2 = sourceGene2;
    }
    
    public override string ToString()
    {
        return $"GeneDef: {InnovationNumber} ({Type}) | {SourceGene1} {SourceGene2}";
    }
}