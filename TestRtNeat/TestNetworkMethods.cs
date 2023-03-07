namespace TestRtNeat;

public class TestNetworkMethods
{
    private Network _network;
    private Configuration _config;

    [SetUp]
    public void Setup()
    {
        _config = new Configuration
        {
            InputCount = 3,
            OutputCount = 1
        };
        _network = new Network(_config);
    }

    [Test]
    public void TestNetwork_Mutate()
    {
        var config = new Configuration()
        {
            ChangeActivationFunctionChance = 1,
            AddConnectionChance = 1,
            AddNodeChance = 1,
            RemoveNodeChance = 1,
            WeightMutationChance = 1,
            BiasMutationChance = 1
        };
        _network = new Network(config);
        var before = _network.ToString();
        for (int i = 0; i < 5; i++)
        {
            try
            {
                _network.Mutate();
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        var after = _network.ToString();
        Assert.That(after, Is.Not.EqualTo(before));
    }

    [Test]
    public void TestNetwork_FeedInputs()
    {
        var inputs = new double[] { 1, 2, 3 };
        try
        {
            _network.FeedInputs(inputs);
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }

        inputs = new double[] { 1, 2, 3, 4 };
        Assert.Throws<ArgumentException>(() => _network.FeedInputs(inputs));
    }

    [Test]
    public void TestNetwork_GetOutputs()
    {
        var inputs = new double[] { 1, 2, 3 };
        _network.FeedInputs(inputs);
        _network.Update();
        var output = _network.GetOutputs();
        Assert.That(output, Is.Not.Null);
        Assert.That(output, Has.Length.EqualTo(1));
    }

    [Test]
    public void TestNetwork_FromJson()
    {
        Network? net = null;
        try
        {
            net = Network.FromJson("""
{
  "Connections": [
    {
      "From": 0,
      "To": 2,
      "Weight": 1.5,
      "Enabled": false,
      "InnovationNumber": 3,
      "GeneType": 1
    }
  ],
  "Nodes": [
    {
      "NodeType": 0,
      "Value": 0,
      "Bias": 0.5,
      "Activation": 11,
      "InnovationNumber": 0,
      "GeneType": 0
    },
    {
      "NodeType": 0,
      "Value": 0,
      "Bias": 1,
      "Activation": 0,
      "InnovationNumber": 1,
      "GeneType": 0
    },
    {
      "NodeType": 1,
      "Value": 0,
      "Bias": -1,
      "Activation": 0,
      "InnovationNumber": 2,
      "GeneType": 0
    }
  ],
  "Species": "00000000-0000-0000-0000-000000000000",
  "InputCount": 2,
  "OutputCount": 1,
  "HiddenCount": 0
}
""");
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }

        Assert.That(net, Is.Not.Null);
    }

    [Test]
    public void TestNetwork_ToJson()
    {
        var config = new Configuration()
        {
            ChangeActivationFunctionChance = 1,
            AddConnectionChance = 1,
            AddNodeChance = 1,
            RemoveNodeChance = 1,
            WeightMutationChance = 1,
            BiasMutationChance = 1
        };
        _network = new Network(config);
        var before = _network.ToJson();
        for (int i = 0; i < 5; i++)
        {
            try
            {
                _network.Mutate();
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        var after = _network.ToJson();
        Assert.That(after, Is.Not.EqualTo(before));
    }

    [Test]
    public void TestNetwork_ValidOutput()
    {
        var net = Network.FromJson("""
{
  "Connections": [
    {
      "From": 0,
      "To": 3,
      "Weight": -1.42,
      "Enabled": true,
      "InnovationNumber": 5
    },{
      "From": 1,
      "To": 3,
      "Weight": -0.8,
      "Enabled": true,
      "InnovationNumber": 6
    },{
      "From": 2,
      "To": 4,
      "Weight": 1.13,
      "Enabled": true,
      "InnovationNumber": 7
    },{
      "From": 3,
      "To": 4,
      "Weight": -0.74,
      "Enabled": true,
      "InnovationNumber": 8
    }
  ],
  "Nodes": [
    {
      "NodeType": 0,
      "Value": 0,
      "Bias": -0.12,
      "Activation": 0,
      "InnovationNumber": 0
    },
    {
      "NodeType": 0,
      "Value": 0,
      "Bias": -0.08,
      "Activation": 0,
      "InnovationNumber": 1
    },
    {
      "NodeType": 0,
      "Value": 0,
      "Bias": 0.11,
      "Activation": 0,
      "InnovationNumber": 2
    },
    {
      "NodeType": 1,
      "Value": 0,
      "Bias": 1.96,
      "Activation": 0,
      "InnovationNumber": 3
    },
    {
      "NodeType": 2,
      "Value": 0,
      "Bias": -0.53,
      "Activation": 0,
      "InnovationNumber": 4
    }
  ],
  "Species": "00000000-0000-0000-0000-000000000000",
  "InputCount": 2,
  "OutputCount": 1,
  "HiddenCount": 1
}
""");
        
        double[] inputs = { 0.79, 2.31, 0.19 };
        
        Assert.That(net, Is.Not.Null);
        net.FeedInputs(inputs);
        net.Update();
        var output = net.GetOutputs();
        Assert.That(output, Is.Not.Null);
        Assert.That(output, Has.Length.EqualTo(1));
        Assert.That(output[0] - 2.363196, Is.LessThan(0.0000001));
    }
}