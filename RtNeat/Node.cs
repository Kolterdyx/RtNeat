using System.Text.Json.Serialization;

namespace RtNeat;

/**
 * The Node class represents a node in the network.
 * It can be an input, output or hidden node.
 * It has a value that is updated when the network is fed inputs.
 */
public class Node : Gene
{
    public NodeType NodeType { get; }

    public double Value { get; private set; }

    public double Bias { get; private set; }

    public ActivationFunctionType Activation { get; private set; }

    [JsonIgnore] public NodeState State { get; set; }

    public Node(NodeType type, long innovationNumber) : this(type, 0, 0, ActivationFunctionType.Sigmoid,
        innovationNumber)
    {
    }

    public Node(Node node) : this(node.NodeType, node.Value, node.Bias, ActivationFunctionType.Sigmoid,
        node.InnovationNumber)
    {
    }


    [JsonConstructor]
    public Node(NodeType nodeType, double value, double bias, ActivationFunctionType activation,
        long innovationNumber) : base(GeneType.Node,
        innovationNumber)
    {
        NodeType = nodeType;
        Value = value;
        Bias = bias;
        Activation = activation;
        State = NodeState.Unvisited;
    }


    public double Update(double value)
    {
        Value = ActivationFunction.Call(Activation, value + Bias);
        State = NodeState.Visited;
        return Value;
    }

    public override string ToString()
    {
        return $"Node: {InnovationNumber} | {NodeType} - {Value} - {Bias} - {Activation}";
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var node = (Node)obj;
        return InnovationNumber == node.InnovationNumber && NodeType == node.NodeType && Value.Equals(node.Value) &&
               Bias.Equals(node.Bias);
    }

    public override int GetHashCode()
    {
        return InnovationNumber.GetHashCode() ^ NodeType.GetHashCode() ^ Value.GetHashCode() ^ Bias.GetHashCode();
    }

    public void Reset()
    {
        State = NodeState.Unvisited;
    }

    public void MutateBias(Random random, Configuration config)
    {
        if (random.NextDouble() < config.BiasMutationChance)
        {
            Bias += random.NextDouble() * config.BiasMutationRate * 2 - config.BiasMutationRate;
        }
    }
    
    public void MutateActivation(Random random, Configuration config)
    {
        if (random.NextDouble() < config.ChangeActivationFunctionChance)
        {
            Activation = ActivationFunction.GetRandomActivationFunction(random);
        }
    }
}