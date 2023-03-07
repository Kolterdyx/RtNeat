using System.Text.Json.Serialization;

namespace RtNeat;

public abstract class Gene
{
    public long InnovationNumber { get; }

    [JsonConstructor]
    protected Gene(long innovationNumber)
    {
        InnovationNumber = innovationNumber;
    }

    protected Gene(Gene gene)
    {
        InnovationNumber = gene.InnovationNumber;
    }

    public override string ToString()
    {
        return $"Gene: {InnovationNumber}";
    }

    public bool Equals(Gene gene)
    {
        return InnovationNumber == gene.InnovationNumber;
    }

    public bool Equals(long innovationNumber)
    {
        return InnovationNumber == innovationNumber;
    }

    public override int GetHashCode()
    {
        return InnovationNumber.GetHashCode();
    }

    public abstract GeneType GetGeneType();
}