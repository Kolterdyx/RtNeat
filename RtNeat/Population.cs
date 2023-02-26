using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RtNeat;

public class Population
{
    [JsonInclude] public Configuration Configuration { get; }

    [JsonInclude] public Dictionary<Guid, Network> Networks { get; private set; } = new();

    [JsonInclude] public HashSet<Species> Species { get; set; }
    
    [JsonInclude] public GenePool GenePool { get; set; }

    public Population(Configuration configuration)
    {
        Configuration = configuration;
        Species = new HashSet<Species>();
        GenePool = GenePool.GetInstance();
    }
    
    [JsonConstructor]
    private Population(Configuration configuration, Dictionary<Guid, Network> networks, HashSet<Species> species, GenePool genePool)
    {
        Configuration = configuration;
        Networks = networks;
        Species = species;
        GenePool = genePool;
    }

    public Guid CreateNetwork()
    {
        if (Configuration.LimitPopulationSize && Networks.Count >= Configuration.PopulationSize)
        {
            Console.WriteLine("Population is full");
            return Guid.Empty;
        }

        var uuid = Guid.NewGuid();
        Networks.Add(uuid, new Network(Configuration));
        return uuid;
    }

    private Network GetNetwork(Guid uuid)
    {
        if (!Networks.ContainsKey(uuid))
        {
            throw new ArgumentException("Network does not exist");
        }
        return Networks[uuid];
    }

    public void RemoveNetwork(Guid uuid)
    {
        Networks.Remove(uuid);
        Species.Where(species => species.Members.Contains(uuid)).ToList().ForEach(species => species.Members.Remove(uuid));
    }

    public void FeedInputs(Guid uuid, double[] inputs)
    {
        GetNetwork(uuid).FeedInputs(inputs);
    }

    public void UpdateNetwork(Guid uuid)
    {
        GetNetwork(uuid).Update();
    }

    public double[] GetOutputs(Guid uuid)
    {
        return GetNetwork(uuid).GetOutputs();
    }

    public void Mutate(Guid uuid)
    {
        GetNetwork(uuid).Mutate();
    }

    public void MutateAll()
    {
        foreach (var network in Networks.Values)
        {
            network.Mutate();
        }
    }
    
    public Guid Crossover(Guid parentA, Guid parentB)
    {
        if (Configuration.LimitPopulationSize && Networks.Count >= Configuration.PopulationSize)
        {
            Console.WriteLine("Population is full");
            return Guid.Empty;
        }

        var child = Network.Crossover(GetNetwork(parentA), GetNetwork(parentB));
        if (child == null)
        {
            return Guid.Empty;
        }

        var uuid = Guid.NewGuid();
        Networks.Add(uuid, child);
        return uuid;
    }

    public Guid Clone(Guid parent)
    {
        if (Configuration.LimitPopulationSize && Networks.Count >= Configuration.PopulationSize)
        {
            Console.WriteLine("Population is full");
            return Guid.Empty;
        }

        var child = new Network(GetNetwork(parent));
        var uuid = Guid.NewGuid();
        Networks.Add(uuid, child);
        return uuid;
    }

    public void Save(string path)
    {
        File.WriteAllText(path, Serialize());
    }

    public static Population? Load(string path)
    {
        var json = File.ReadAllText(path);
        return Deserialize(json);
    }

    private static void RunCommmand(string command)
    {
        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "/usr/bin/bash",
                Arguments = "-c \"" + command + "\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        Console.WriteLine(result);
    }

    public void VisualizeNetwork(Guid network)
    {
        if (network == Guid.Empty)
        {
            Console.WriteLine("Invalid network");
            return;
        }

        var json = JsonSerializer.Serialize(GetNetwork(network));
        File.WriteAllText("data.json", json);
        RunCommmand("source NetPlotter/venv/bin/activate; python3 NetPlotter/plotNetwork.py data.json -b -s");
    }

    public void DefineSpecies()
    {
        foreach (var network in Networks)
        {
            network.Value.Species = Guid.Empty;
        }

        foreach (var species in Species)
        {
            species.Members.Clear();
        }

        var unassigned = new List<Guid>(Networks.Keys);
        var assigned = new List<Guid>();
        var i = 0;
        do
        {
            var current = unassigned[0];
            if (Species.Count <= i)
            {
                Species.Add(new Species());
            }
            var species = Species.ElementAt(i);
            GetNetwork(current).Species = species.UUID;
            species.AddMember(current);
            assigned.Clear();
            assigned.Add(current);
            foreach (var other in unassigned)
            {
                if (Distance(current, other) < Configuration.CompatibilityThreshold && other != current)
                {
                    species.AddMember(other);
                    GetNetwork(other).Species = species.UUID;
                    assigned.Add(other);
                }
            }

            foreach (var assign in assigned)
            {
                unassigned.Remove(assign);
            }

            i++;
        } while (unassigned.Count > 0);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Population:");
        sb.AppendLine($"  Networks: {Networks.Count}");
        sb.AppendLine($"  Species: {Species.Count}");
        foreach (var species in Species)
        {
            sb.AppendLine($"    {species}");
        }

        return sb.ToString();
    }

    private double Distance(Guid netA, Guid netB)
    {
        return Network.Distance(GetNetwork(netA), GetNetwork(netB));
    }

    public string Serialize()
    {
        DefineSpecies();
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        return JsonSerializer.Serialize(this, options);
    }
    
    public static Population? Deserialize(string json)
    {
        return JsonSerializer.Deserialize<Population>(json);
    }
}