using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using IniParser;
using IniParser.Model;

namespace RtNeat;

/// <summary>
/// This struct is used to configure the Real Time NEAT algorithm.
/// </summary>
public class Configuration
{
    
    [JsonInclude] public int InputCount { get; }
    [JsonInclude] public int OutputCount { get; }
    [JsonInclude] public bool LimitPopulationSize { get; }
    [JsonInclude] public int PopulationSize { get; }

    [JsonInclude] public bool LimitNodeCount { get; }
    [JsonInclude] public int MaxNodeCount { get; }
    [JsonInclude] public bool LimitConnectionCount { get; }
    [JsonInclude] public int MaxConnectionCount { get; }

    [JsonInclude] public double WeightMutationRate { get; }
    [JsonInclude] public double WeightMutationChance { get; }
    [JsonInclude] public double BiasMutationRate { get; }
    [JsonInclude] public double BiasMutationChance { get; }

    [JsonInclude] public double ChangeActivationFunctionChance { get; }

    [JsonInclude] public double AddNodeChance { get; }
    [JsonInclude] public double RemoveNodeChance { get; }
    [JsonInclude] public double AddConnectionChance { get; }
    [JsonInclude] public double DisableConnectionChance { get; }

    [JsonInclude] public double CompatibilityThreshold { get; }
    [JsonInclude] public double CompatibilityExcessCoefficient { get; }
    [JsonInclude] public double CompatibilityDisjointCoefficient { get; }
    [JsonInclude] public double CompatibilityWeightCoefficient { get; }

    [JsonInclude] public List<string> SpeciesNames { get; }

    public Configuration()
    {
        // Default values
        InputCount = 1;
        OutputCount = 1;
        LimitPopulationSize = false;
        PopulationSize = 100;
        LimitNodeCount = false;
        MaxNodeCount = 100;
        LimitConnectionCount = false;
        MaxConnectionCount = 100;
        WeightMutationRate = 0.1;
        WeightMutationChance = 0.8;
        BiasMutationRate = 0.1;
        BiasMutationChance = 0.8;
        ChangeActivationFunctionChance = 0.1;
        AddNodeChance = 0.03;
        RemoveNodeChance = 0.03;
        AddConnectionChance = 0.05;
        DisableConnectionChance = 0.4;
        CompatibilityThreshold = 3;
        CompatibilityExcessCoefficient = 1;
        CompatibilityDisjointCoefficient = 1;
        CompatibilityWeightCoefficient = 0.4;
        SpeciesNames = new List<string>();
    }
    
    public Configuration(string file) : this()
    {
        FileIniDataParser parser = new();
        var data = parser.ReadFile(file);
        if (data == null)
        {
            throw new Exception("Configuration file not found");
        }
        
        InputCount = int.Parse(data["Network"]["InputCount"] ?? InputCount.ToString());
        OutputCount = int.Parse(data["Network"]["OutputCount"] ?? OutputCount.ToString());
        LimitNodeCount = bool.Parse(data["Network"]["LimitNodeCount"] ?? LimitNodeCount.ToString());
        MaxNodeCount = int.Parse(data["Network"]["MaxNodeCount"] ?? MaxNodeCount.ToString());
        LimitConnectionCount = bool.Parse(data["Network"]["LimitConnectionCount"] ?? LimitConnectionCount.ToString());
        MaxConnectionCount = int.Parse(data["Network"]["MaxConnectionCount"] ?? MaxConnectionCount.ToString());
        
        WeightMutationRate = double.Parse(data["Mutation"]["WeightMutationRate"] ?? WeightMutationRate.ToString());
        WeightMutationChance = double.Parse(data["Mutation"]["WeightMutationChance"] ?? WeightMutationChance.ToString());
        BiasMutationRate = double.Parse(data["Mutation"]["BiasMutationRate"] ?? BiasMutationRate.ToString());
        BiasMutationChance = double.Parse(data["Mutation"]["BiasMutationChance"] ?? BiasMutationChance.ToString());
        ChangeActivationFunctionChance = double.Parse(data["Mutation"]["ChangeActivationFunctionChance"] ?? ChangeActivationFunctionChance.ToString());
        AddNodeChance = double.Parse(data["Mutation"]["AddNodeChance"] ?? AddNodeChance.ToString());
        RemoveNodeChance = double.Parse(data["Mutation"]["RemoveNodeChance"] ?? RemoveNodeChance.ToString());
        AddConnectionChance = double.Parse(data["Mutation"]["AddConnectionChance"] ?? AddConnectionChance.ToString());
        DisableConnectionChance = double.Parse(data["Mutation"]["DisableConnectionChance"] ?? DisableConnectionChance.ToString());
        
        CompatibilityThreshold = double.Parse(data["GeneticCompatibility"]["Threshold"] ?? CompatibilityThreshold.ToString());
        CompatibilityExcessCoefficient = double.Parse(data["GeneticCompatibility"]["ExcessCoefficient"] ?? CompatibilityExcessCoefficient.ToString());
        CompatibilityDisjointCoefficient = double.Parse(data["GeneticCompatibility"]["DisjointCoefficient"] ?? CompatibilityDisjointCoefficient.ToString());
        CompatibilityWeightCoefficient = double.Parse(data["GeneticCompatibility"]["WeightCoefficient"] ?? CompatibilityWeightCoefficient.ToString());
        LimitPopulationSize = bool.Parse(data["Population"]["LimitSize"] ?? LimitPopulationSize.ToString());
        PopulationSize = int.Parse(data["Population"]["Size"] ?? PopulationSize.ToString());

        SpeciesNames = new List<string>();
        var names = JsonSerializer.Deserialize<string[]>(data["Population"]["SpeciesNames"] ?? "[]") ?? Array.Empty<string>();
        SpeciesNames.AddRange(names);
    }

    [JsonConstructor]
    public Configuration(int inputCount, int outputCount, bool limitPopulationSize, int populationSize, bool limitNodeCount, int maxNodeCount, bool limitConnectionCount, int maxConnectionCount, double weightMutationRate, double weightMutationChance, double biasMutationRate, double biasMutationChance, double changeActivationFunctionChance, double addNodeChance, double removeNodeChance, double addConnectionChance, double disableConnectionChance, double compatibilityThreshold, double compatibilityExcessCoefficient, double compatibilityDisjointCoefficient, double compatibilityWeightCoefficient, List<string> speciesNames)
    {
        InputCount = inputCount;
        OutputCount = outputCount;
        LimitPopulationSize = limitPopulationSize;
        PopulationSize = populationSize;
        LimitNodeCount = limitNodeCount;
        MaxNodeCount = maxNodeCount;
        LimitConnectionCount = limitConnectionCount;
        MaxConnectionCount = maxConnectionCount;
        WeightMutationRate = weightMutationRate;
        WeightMutationChance = weightMutationChance;
        BiasMutationRate = biasMutationRate;
        BiasMutationChance = biasMutationChance;
        ChangeActivationFunctionChance = changeActivationFunctionChance;
        AddNodeChance = addNodeChance;
        RemoveNodeChance = removeNodeChance;
        AddConnectionChance = addConnectionChance;
        DisableConnectionChance = disableConnectionChance;
        CompatibilityThreshold = compatibilityThreshold;
        CompatibilityExcessCoefficient = compatibilityExcessCoefficient;
        CompatibilityDisjointCoefficient = compatibilityDisjointCoefficient;
        CompatibilityWeightCoefficient = compatibilityWeightCoefficient;
        SpeciesNames = speciesNames;
    }
    
    public void ToFile(string file)
    {
        // Save the configuration to a file
        IniData data = new();
        data.Sections.AddSection("Network");
        data["Network"].AddKey("InputCount", InputCount.ToString());
        data["Network"].AddKey("OutputCount", OutputCount.ToString());
        data["Network"].AddKey("LimitNodeCount", LimitNodeCount.ToString());
        data["Network"].AddKey("MaxNodeCount", MaxNodeCount.ToString());
        data["Network"].AddKey("LimitConnectionCount", LimitConnectionCount.ToString());
        data["Network"].AddKey("MaxConnectionCount", MaxConnectionCount.ToString());
        data.Sections.AddSection("Mutation");
        data["Mutation"].AddKey("WeightMutationRate", WeightMutationRate.ToString());
        data["Mutation"].AddKey("WeightMutationChance", WeightMutationChance.ToString());
        data["Mutation"].AddKey("BiasMutationRate", BiasMutationRate.ToString());
        data["Mutation"].AddKey("BiasMutationChance", BiasMutationChance.ToString());
        data["Mutation"].AddKey("ChangeActivationFunctionChance", ChangeActivationFunctionChance.ToString());
        data["Mutation"].AddKey("AddNodeChance", AddNodeChance.ToString());
        data["Mutation"].AddKey("RemoveNodeChance", RemoveNodeChance.ToString());
        data["Mutation"].AddKey("AddConnectionChance", AddConnectionChance.ToString());
        data["Mutation"].AddKey("DisableConnectionChance", DisableConnectionChance.ToString());
        data.Sections.AddSection("GeneticCompatibility");
        data["GeneticCompatibility"].AddKey("Threshold", CompatibilityThreshold.ToString());
        data["GeneticCompatibility"].AddKey("ExcessCoefficient", CompatibilityExcessCoefficient.ToString());
        data["GeneticCompatibility"].AddKey("DisjointCoefficient", CompatibilityDisjointCoefficient.ToString());
        data["GeneticCompatibility"].AddKey("WeightCoefficient", CompatibilityWeightCoefficient.ToString());
        data.Sections.AddSection("Population");
        data["Population"].AddKey("LimitSize", LimitPopulationSize.ToString());
        data["Population"].AddKey("MaxSize", PopulationSize.ToString());
        data["Population"].AddKey("SpeciesNames", "[]");

        FileIniDataParser parser = new();
        parser.WriteFile(file, data);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Network");
        sb.AppendLine($"InputCount: {InputCount}");
        sb.AppendLine($"OutputCount: {OutputCount}");
        sb.AppendLine($"LimitNodeCount: {LimitNodeCount}");
        sb.AppendLine($"MaxNodeCount: {MaxNodeCount}");
        sb.AppendLine($"LimitConnectionCount: {LimitConnectionCount}");
        sb.AppendLine($"MaxConnectionCount: {MaxConnectionCount}");
        sb.AppendLine("\nMutation");
        sb.AppendLine($"WeightMutationRate: {WeightMutationRate}");
        sb.AppendLine($"WeightMutationChance: {WeightMutationChance}");
        sb.AppendLine($"BiasMutationRate: {BiasMutationRate}");
        sb.AppendLine($"BiasMutationChance: {BiasMutationChance}");
        sb.AppendLine($"ChangeActivationFunctionChance: {ChangeActivationFunctionChance}");
        sb.AppendLine($"AddNodeChance: {AddNodeChance}");
        sb.AppendLine($"RemoveNodeChance: {RemoveNodeChance}");
        sb.AppendLine($"AddConnectionChance: {AddConnectionChance}");
        sb.AppendLine($"DisableConnectionChance: {DisableConnectionChance}");
        sb.AppendLine("\nGeneticCompatibility");
        sb.AppendLine($"CompatibilityThreshold: {CompatibilityThreshold}");
        sb.AppendLine($"CompatibilityExcessCoefficient: {CompatibilityExcessCoefficient}");
        sb.AppendLine($"CompatibilityDisjointCoefficient: {CompatibilityDisjointCoefficient}");
        sb.AppendLine($"CompatibilityWeightCoefficient: {CompatibilityWeightCoefficient}");
        sb.AppendLine("\nPopulation");
        sb.AppendLine($"LimitSize: {LimitPopulationSize}");
        sb.AppendLine($"Size: {PopulationSize}");
        sb.Append($"SpeciesNames: [");
        foreach (var name in SpeciesNames)
        {
            sb.Append($"\"{name}\", ");
        }

        sb.Remove(sb.Length - 2, 2);
        sb.AppendLine("]");
        return sb.ToString();
    }
    
}