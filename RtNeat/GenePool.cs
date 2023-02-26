using System.Text;
using System.Text.Json.Serialization;

namespace RtNeat;

/**
 * The GenePool class is used to keep track of all the genomes in the system.
 * It is used to assign unique IDs to genomes and to keep track of the highest ID
 */
public class GenePool
{
    [JsonInclude] public long InnovationNumber { get; private set; }
    [JsonInclude] public readonly List<GeneDef> GeneDefs = new();

    private static GenePool? _instance;
    
    public int Count => GeneDefs.Count;

    private GenePool()
    {
        InnovationNumber = 0;
    }
    
    [JsonConstructor]
    public GenePool(long innovationNumber, List<GeneDef> geneDefs)
    {
        InnovationNumber = innovationNumber;
        GeneDefs = geneDefs;
        _instance = this;
    }

    public static GenePool GetInstance()
    {
        return _instance ??= new GenePool();
    }

    private long GetInnovationNumber(GeneType type, long sourceGene1, long sourceGene2)
    {
        foreach (var geneDef in GeneDefs.Where(geneDef => geneDef.Type == type && geneDef.SourceGene1 == sourceGene1 && geneDef.SourceGene2 == sourceGene2))
        {
            return geneDef.InnovationNumber;
        }
        GeneDefs.Add(new GeneDef(InnovationNumber, type, sourceGene1, sourceGene2));
        return InnovationNumber++;
    }

    public void Reset()
    {
        InnovationNumber = 0;
        GeneDefs.Clear();
    }


    public void Reset(GenePool genePool)
    {
        InnovationNumber = genePool.InnovationNumber;
        GeneDefs.Clear();
        GeneDefs.AddRange(genePool.GeneDefs);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Innovation Number: {InnovationNumber}");
        sb.AppendLine("Gene Defs:");
        foreach (var geneDef in GeneDefs)
        {
            sb.AppendLine($"  {geneDef}");
        }

        return sb.ToString();
    }

    public long GetNodeInnovationNumber(long source1, long source2)
    {
        return GetInnovationNumber(GeneType.Node, source1, source2);
    }

    public long GetConnectionInnovationNumber(long source1, long source2)
    {
        return GetInnovationNumber(GeneType.Connection, source1, source2);
    }
}