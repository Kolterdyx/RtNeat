using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;

namespace RtNeat;

/**
 * A network is a collection of nodes and connections.
 * Can be fed inputs and outputs can be read.
 *
 * @author Ciro García
 */
public class Network
{
    [JsonInclude] public List<Connection> Connections { get; private set; } = new();
    [JsonInclude] public List<Node> Nodes { get; private set; } = new();
    [JsonInclude] public Guid Species { get; set; }

    public int InputCount => Nodes.Count(n => n.NodeType == NodeType.Input);
    public int OutputCount => Nodes.Count(n => n.NodeType == NodeType.Output);

    public int HiddenCount => Nodes.Count(n => n.NodeType == NodeType.Hidden);

    private static readonly GenePool GenePool = GenePool.GetInstance();

    private static Random _random = new Random();
    private static Configuration _config = new Configuration();

    public Network(int inputs, int outputs)
    {
        if (inputs < 1)
        {
            throw new ArgumentException("Network must have at least one input");
        }
        if (outputs < 1)
        {
            throw new ArgumentException("Network must have at least one output");
        }
        
        for (var i = 0; i < inputs; i++)
        {
            var node = new Node(
                NodeType.Input,
                0,
                _random.NextDouble() * 4 - 2,
                ActivationFunction.GetRandomActivationFunction(_random),
                GenePool.GetNodeInnovationNumber(-1, i));
            Nodes.Add(node);
        }

        for (var i = 0; i < outputs; i++)
        {
            var node = new Node(
                NodeType.Output,
                0,
                _random.NextDouble() * 4 - 2,
                ActivationFunction.GetRandomActivationFunction(_random),
                GenePool.GetNodeInnovationNumber(-2, i));
            Nodes.Add(node);
        }
    }

    public static void SetSeed(int seed)
    {
        _random = new Random(seed);
    }

    public Network(Network network)
    {
        Nodes.Clear();
        foreach (var node in network.Nodes)
        {
            Nodes.Add(new Node(node));
        }

        Connections.Clear();
        foreach (var connection in network.Connections)
        {
            Connections.Add(new Connection(connection));
        }
    }

    [JsonConstructor]
    public Network(List<Node> nodes, List<Connection> connections, Guid species)
    {
        Nodes = nodes;
        Connections = connections;
        Species = species;
    }

    public Network(Configuration configuration) : this(configuration.InputCount, configuration.OutputCount)
    {
        _config = configuration;
    }

    private long InsertNode(long connection)
    {
        // Disable the connection
        var disabledConnection = Connections.Find(c => c.InnovationNumber == connection);
        if (disabledConnection == null)
        {
            return -1;
        }

        if (_config.LimitNodeCount && Nodes.Count >= _config.MaxNodeCount)
        {
            return -1;
        }

        if (_config.LimitConnectionCount && Connections.Count >= _config.MaxConnectionCount - 1)
        {
            return -1;
        }

        disabledConnection.Disable();

        // Create a new node
        var newNode = new Node(
            NodeType.Hidden,
            0,
            _random.NextDouble() * 4 - 2,
            ActivationFunction.GetRandomActivationFunction(_random),
            GenePool.GetNodeInnovationNumber(disabledConnection.From, disabledConnection.To));
        if (Nodes.Find(n => n.InnovationNumber == newNode.InnovationNumber) != null)
        {
            return -1;
        }

        Nodes.Add(newNode);

        // Create a new connection from the input node to the new node
        InsertConnection(disabledConnection.From, newNode.InnovationNumber);

        // Create a new connection from the new node to the output node
        InsertConnection(newNode.InnovationNumber, disabledConnection.To);

        return newNode.InnovationNumber;
    }

    public long InsertConnection(long from, long to)
    {
        if (Nodes.Find(n => n.InnovationNumber == from) == null ||
            Nodes.Find(n => n.InnovationNumber == to) == null)
        {
            return -1;
        }

        if (_config.LimitConnectionCount && Connections.Count >= _config.MaxConnectionCount)
        {
            return -1;
        }

        var newConnection = new Connection(from, to, _random.NextDouble() * 2 - 1, true,
            GenePool.GetConnectionInnovationNumber(from, to));
        var connection = Connections.Find(c => c.InnovationNumber == newConnection.InnovationNumber);
        if (connection != null)
        {
            if (!connection.Enabled)
            {
                connection.Enable();
            }

            return connection.InnovationNumber;
        }

        Connections.Add(newConnection);
        return newConnection.InnovationNumber;
    }

    public void FeedInputs(double[] inputs)
    {
        if (inputs.Length != Nodes.Count(n => n.NodeType == NodeType.Input))
        {
            throw new ArgumentException(
                $"The number of inputs ({inputs.Length}) does not match the number of input nodes {Nodes.Count(n => n.NodeType == NodeType.Input)}");
        }

        var inputNodes = Nodes.Where(n => n.NodeType == NodeType.Input).ToList();
        for (var i = 0; i < inputNodes.Count; i++)
        {
            inputNodes[i].Update(inputs[i]);
        }
    }

    public double[] GetOutputs()
    {
        var outputNodes = Nodes.Where(n => n.NodeType == NodeType.Output).ToList();
        var outputs = new double[outputNodes.Count];
        for (var i = 0; i < outputNodes.Count; i++)
        {
            outputs[i] = outputNodes[i].Value;
        }

        return outputs;
    }

    public void Update()
    {
        var nodesToBeUpdated = Nodes.Where(n => n.NodeType != NodeType.Input).ToList();
        foreach (var node in nodesToBeUpdated)
        {
            node.Reset();
        }

        var outputNodes = Nodes.Where(n => n.NodeType == NodeType.Output).ToList();
        foreach (var node in outputNodes)
        {
            UpdateNode(node.InnovationNumber);
        }
    }

    private void UpdateNode(long nodeInnovationNumber)
    {
        var node = Nodes.Find(n => n.InnovationNumber == nodeInnovationNumber);
        if (node == null)
        {
            return;
        }

        if (node.State == NodeState.Visited)
        {
            return;
        }

        if (node.State == NodeState.Visiting)
        {
            CalculateNodeValue(node, GetNodesBefore(nodeInnovationNumber), GetConnectionsBefore(nodeInnovationNumber));
            return;
        }

        node.State = NodeState.Visiting;
        CalculateNodeValue(node, GetNodesBefore(nodeInnovationNumber), GetConnectionsBefore(nodeInnovationNumber));
    }

    private void CalculateNodeValue(Node node, List<Node> nodesBefore, List<Connection> connectionsBefore)
    {
        var sum = 0.0;
        for (var i = 0; i < nodesBefore.Count; i++)
        {
            sum += nodesBefore[i].Value * connectionsBefore[i].Weight;
        }

        node.Update(sum);
    }

    public void Mutate()
    {
        // Add a new connection
        if (_random.NextDouble() < _config.AddConnectionChance)
        {
            var nonInputNodes = Nodes.Where(n => n.NodeType != NodeType.Input).ToList();
            var nonOutputNodes = Nodes.Where(n => n.NodeType != NodeType.Output).ToList();
            var from = nonOutputNodes[_random.Next(nonOutputNodes.Count)];
            var to = nonInputNodes[_random.Next(nonInputNodes.Count)];
            InsertConnection(from.InnovationNumber, to.InnovationNumber);
        }

        // Add a new node
        if (_random.NextDouble() < _config.AddNodeChance && Connections.Count > 0)
        {
            var connection = Connections[_random.Next(Connections.Count)];
            InsertNode(connection.InnovationNumber);
        }

        // MutateWeight the weights of the connections
        foreach (var connection in Connections)
        {
            connection.MutateWeight(_random, _config);
        }

        // Disable a connection
        if (_random.NextDouble() < _config.DisableConnectionChance && Connections.Count > 0)
        {
            var connection = Connections[_random.Next(Connections.Count)];
            connection.Disable();
        }

        // Change the activation function
        foreach (var node in Nodes)
        {
            node.MutateActivation(_random, _config);
        }

        // Change the bias
        foreach (var node in Nodes)
        {
            node.MutateBias(_random, _config);
        }

        // Remove a hidden node
        if (_random.NextDouble() < _config.RemoveNodeChance && Nodes.Count(n => n.NodeType == NodeType.Hidden) > 0)
        {
            var hiddenNodes = Nodes.Where(n => n.NodeType == NodeType.Hidden).ToList();
            var node = hiddenNodes[_random.Next(hiddenNodes.Count)];
            RemoveNode(node.InnovationNumber);
        }
    }

    private void RemoveNode(long nodeInnovationNumber)
    {
        if (Nodes.All(n => n.InnovationNumber != nodeInnovationNumber))
        {
            throw new ArgumentException(
                $"The node with innovation number {nodeInnovationNumber} does not exist in the network");
        }

        if (Nodes.Count(n => n.NodeType == NodeType.Hidden) == 0)
        {
            throw new ArgumentException("There are no hidden nodes in the network");
        }

        if (Nodes.First(n => n.InnovationNumber == nodeInnovationNumber).NodeType != NodeType.Hidden)
        {
            throw new ArgumentException("Cannot remove an input/output node");
        }

        var node = Nodes.First(n => n.InnovationNumber == nodeInnovationNumber);
        var connectionsBefore = GetConnectionsBefore(nodeInnovationNumber);
        var connectionsAfter = GetConnectionsAfter(nodeInnovationNumber);
        foreach (var connection in connectionsBefore)
        {
            Connections.Remove(connection);
        }

        foreach (var connection in connectionsAfter)
        {
            Connections.Remove(connection);
        }

        Nodes.Remove(node);
    }

    /// <summary>
    /// This method gets the sorted genes of the network. It is used to calculate the distance between two networks.
    /// Currently, only the Connection genes are used to calculate the distance.
    /// </summary>
    /// <returns>List&lt;Connection></returns>
    private List<Connection> GetGenes()
    {
        var genes = new List<Connection>();
        genes.AddRange(Connections);
        genes.Sort((a, b) => a.InnovationNumber.CompareTo(b.InnovationNumber));
        return genes;
    }

    public static double Distance(Network networkA, Network networkB)
    {
        // Must not use the GenePool of networkA or networkB, because they are both always the same. Use the genes of the networks instead.

        var disjointGenes = 0;
        var excessGenes = 0;
        var matchingGenes = 0;
        var weightDifference = 0.0;

        var genesA = networkA.GetGenes();
        var genesB = networkB.GetGenes();
        var maxGenes = Math.Max(genesA.Count, genesB.Count);

        // Ensure that genesA is the largest network
        if (genesA.Count < genesB.Count)
        {
            (genesA, genesB) = (genesB, genesA);
        }

        // Iterate over the genes of the largest network (genesA). If the gene is not in the smallest one (genesB), it is can be either
        // disjoint or excess, otherwise it is a matching gene. For an excess gene to be excess, it must be after the
        // last gene of the smallest network.
        var lastGeneB = genesB.LastOrDefault();
        for (var i = 0; i < genesA.Count; i++)
        {
            var geneA = genesA[i];
            var geneB = genesB.FirstOrDefault(g => g.InnovationNumber == geneA.InnovationNumber);
            if (geneB is null)
            {
                if (geneA.InnovationNumber > lastGeneB?.InnovationNumber)
                {
                    excessGenes++;
                }
                else
                {
                    disjointGenes++;
                }
            }
            else
            {
                matchingGenes++;
                weightDifference += Math.Abs(geneA.Weight - geneB.Weight);
            }
        }

        // We may have missed some disjoint genes, because we only checked the genes of the largest network.
        // We need to check the genes of the smallest network to find the remaining disjoint genes.
        for (var i = 0; i < genesB.Count; i++)
        {
            var geneB = genesB[i];
            var geneA = genesA.FirstOrDefault(g => g.InnovationNumber == geneB.InnovationNumber);
            if (geneA is null)
            {
                disjointGenes++;
            }
        }

        var distance = (_config.CompatibilityExcessCoefficient * excessGenes / maxGenes) +
                       (_config.CompatibilityDisjointCoefficient * disjointGenes / maxGenes) +
                       (_config.CompatibilityWeightCoefficient * weightDifference / matchingGenes);
        return Math.Abs(distance);
    }

    public static Network? Crossover(Network parentA, Network parentB)
    {
        if (Distance(parentA, parentB) > _config.CompatibilityThreshold)
        {
            Console.WriteLine("Cannot crossover networks with a distance greater than the compatibility threshold.");
            return null;
        }

        var child = new Network(_config);
        var genesA = parentA.GetGenes();
        var genesB = parentB.GetGenes();
        var maxGenes = Math.Max(genesA.Count, genesB.Count);

        // Ensure that genesA is the largest network
        if (genesA.Count < genesB.Count)
        {
            (genesA, genesB) = (genesB, genesA);
        }

        // We find matching, disjoint and excess genes as in the distance method.
        var lastGeneB = genesB.LastOrDefault();
        foreach (var geneA in genesA)
        {
            var a = geneA;
            var geneB = genesB.FirstOrDefault(g => g.InnovationNumber == a.InnovationNumber);
            if (geneB is null)
            {
                if (geneA.InnovationNumber > lastGeneB?.InnovationNumber)
                {
                    // Excess gene
                    continue;
                }

                // Disjoint gene
                child.ForceAddConnection(geneA);
            }
            else
            {
                // Matching gene
                child.ForceAddConnection(_random.NextDouble() < 0.5 ? geneA : geneB);
            }
        }

        // We may have missed some disjoint genes, because we only checked the genes of the largest network.
        // We need to check the genes of the smallest network to find the remaining disjoint genes.
        foreach (var geneB in genesB)
        {
            var b = geneB;
            var geneA = genesA.FirstOrDefault(g => g.InnovationNumber == b.InnovationNumber);
            if (geneA is null)
            {
                child.ForceAddConnection(geneB);
            }
        }

        return child;
    }

    private void ForceAddConnection(Connection gene)
    {
        var connection = new Connection(gene);
        Connections.Add(connection);
        if (Nodes.All(n => n.InnovationNumber != connection.From))
        {
            Nodes.Add(new Node(NodeType.Hidden, 0, _random.NextDouble() * 4 - 2,
                ActivationFunction.GetRandomActivationFunction(_random), connection.From));
        }

        if (Nodes.All(n => n.InnovationNumber != connection.To))
        {
            Nodes.Add(new Node(NodeType.Hidden, 0, _random.NextDouble() * 4 - 2,
                ActivationFunction.GetRandomActivationFunction(_random), connection.To));
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Nodes:");
        foreach (var node in Nodes)
        {
            sb.AppendLine($"  {node}");
        }

        sb.AppendLine("Connections:");
        foreach (var connection in Connections)
        {
            sb.AppendLine($"  {connection}");
        }

        // sb.AppendLine("Gene Pool:");
        // sb.AppendLine($"{GenePool}");

        return sb.ToString();
    }

    /// The following methods are used for memory usage optimization.
    private List<Node> GetNodesBefore(long nodeInnovationNumber)
    {
        var connections = new List<Connection>();
        foreach (var connection in Connections)
        {
            if (connection.To == nodeInnovationNumber)
            {
                connections.Add(connection);
            }
        }

        var nodes = new List<Node>();
        foreach (var node in Nodes)
        {
            foreach (var connection in connections)
            {
                if (node.InnovationNumber == connection.From)
                {
                    nodes.Add(node);
                }
            }
        }

        return nodes;
    }


    private List<Connection> GetConnectionsBefore(long nodeInnovationNumber)
    {
        var connections = new List<Connection>();
        foreach (var connection in Connections)
        {
            if (connection.To == nodeInnovationNumber)
            {
                connections.Add(connection);
            }
        }

        return connections;
    }

    private List<Connection> GetConnectionsAfter(long nodeInnovationNumber)
    {
        var connections = new List<Connection>();
        foreach (var connection in Connections)
        {
            if (connection.From == nodeInnovationNumber)
            {
                connections.Add(connection);
            }
        }

        return connections;
    }
}