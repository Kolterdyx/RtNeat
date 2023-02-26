using System.Text.Json.Serialization;

namespace RtNeat;

public class Gene
{
    public long InnovationNumber { get; }
    public GeneType GeneType { get; }

    [JsonConstructor]
    protected Gene(GeneType geneType, long innovationNumber)
    {
        InnovationNumber = innovationNumber;
        GeneType = geneType;
    }

    protected Gene(Gene gene)
    {
        InnovationNumber = gene.InnovationNumber;
        GeneType = gene.GeneType;
    }

    public override string ToString()
    {
        return $"Gene: {InnovationNumber} - {GeneType}";
    }

    public bool Equals(Gene gene)
    {
        return InnovationNumber == gene.InnovationNumber && GeneType == gene.GeneType;
    }

    public bool Equals(long innovationNumber)
    {
        return InnovationNumber == innovationNumber;
    }

    public override int GetHashCode()
    {
        return InnovationNumber.GetHashCode() ^ GeneType.GetHashCode();
    }
}