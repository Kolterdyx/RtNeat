using System.Text.Json.Serialization;

namespace RtNeat;

public class Connection : Gene
{

    public long From { get; }
    public long To { get; }
    public double Weight { get; private set; }
    public bool Enabled { get; private set; }

    
    [JsonConstructor]
    public Connection(long from, long to, double weight, bool enabled, long innovationNumber) : base(innovationNumber)
    {
        From = from;
        To = to;
        Weight = weight;
        Enabled = enabled;
    }

    public Connection(Connection connection) : this(connection.From, connection.To, connection.Weight,
        connection.Enabled, connection.InnovationNumber)
    {
    }
    
    public Connection(Node from, Node to, double weight, bool enabled, long innovationNumber) : this(from.InnovationNumber, to.InnovationNumber, weight, enabled, innovationNumber)
    {
    }


    public override string ToString()
    {
        return $"Connection: {InnovationNumber} | {From} -> {To} | {Weight} - {(Enabled ? "Enabled" : "Disabled")}";
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var connection = (Connection)obj;
        return InnovationNumber == connection.InnovationNumber &&
               From == connection.From && To == connection.To && Weight.Equals(connection.Weight);
    }

    public override int GetHashCode()
    {
        return InnovationNumber.GetHashCode() ^ From.GetHashCode() ^ To.GetHashCode();
    }

    public override GeneType GetGeneType()
    {
        return GeneType.Connection;
    }

    public void Disable()
    {
        Enabled = false;
    }
    
    public void Enable()
    {
        Enabled = true;
    }

    public void MutateWeight(Random random, Configuration config)
    {
        if (random.NextDouble() < config.WeightMutationChance)
        {
            Weight += random.NextDouble() * config.WeightMutationRate * 2 - config.WeightMutationRate;
        }
    }
}