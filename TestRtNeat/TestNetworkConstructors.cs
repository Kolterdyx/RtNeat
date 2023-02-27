namespace TestRtNeat;

public class TestNetworkConstructors
{
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test_create_Network_IO_constructor()
    {
        Network network;
        network = new Network(2, 1);
        Assert.Multiple(() =>
        {
            Assert.That(network.InputCount, Is.EqualTo(2));
            Assert.That(network.OutputCount, Is.EqualTo(1));
        });
        
        network = new Network(3, 2);
        Assert.Multiple(() =>
        {
            Assert.That(network.InputCount, Is.EqualTo(3));
            Assert.That(network.OutputCount, Is.EqualTo(2));
        });
        
        try {
            network = new Network(0, 1);
            Assert.Fail("Expected exception");
        } catch (ArgumentException) {
            // Expected
        }
        
        try {
            network = new Network(1, 0);
            Assert.Fail("Expected exception");
        } catch (ArgumentException) {
            // Expected
        }
        
        try {
            network = new Network(0, 0);
            Assert.Fail("Expected exception");
        } catch (ArgumentException) {
            // Expected
        }
    }
    
    [Test]
    public void Test_create_Network_Configuration_constructor()
    {
        Network network;
        Configuration config = new Configuration();
        
        config.InputCount = 3;
        config.OutputCount = 1;
        network = new Network(config);
        Assert.Multiple(() =>
        {
            Assert.That(network.InputCount, Is.EqualTo(3));
            Assert.That(network.OutputCount, Is.EqualTo(1));
        });
        
        config.InputCount = 2;
        config.OutputCount = 2;
        network = new Network(config);
        Assert.Multiple(() =>
        {
            Assert.That(network.InputCount, Is.EqualTo(2));
            Assert.That(network.OutputCount, Is.EqualTo(2));
        });
        
        config.InputCount = 0;
        config.OutputCount = 1;
        try {
            network = new Network(config);
            Assert.Fail("Expected exception");
        } catch (ArgumentException) {
            // Expected
        }
        
        config.InputCount = 1;
        config.OutputCount = 0;
        try {
            network = new Network(config);
            Assert.Fail("Expected exception");
        } catch (ArgumentException) {
            // Expected
        }
        
        config.InputCount = 0;
        config.OutputCount = 0;
        try {
            network = new Network(config);
            Assert.Fail("Expected exception");
        } catch (ArgumentException) {
            // Expected
        }
    }
    
    [Test]
    public void Test_create_Network_Network_constructor()
    {
        Network network;
        Configuration config = new Configuration();
        
        config.InputCount = 3;
        config.OutputCount = 1;
        network = new Network(config);
        Network network2 = new Network(network);
        Assert.Multiple(() =>
        {
            Assert.That(network2.InputCount, Is.EqualTo(3));
            Assert.That(network2.OutputCount, Is.EqualTo(1));
        });
        
        config.InputCount = 2;
        config.OutputCount = 2;
        network = new Network(config);
        network2 = new Network(network);
        Assert.Multiple(() =>
        {
            Assert.That(network2.InputCount, Is.EqualTo(2));
            Assert.That(network2.OutputCount, Is.EqualTo(2));
        });
    }
}