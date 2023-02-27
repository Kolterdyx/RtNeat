namespace TestRtNeat;

public class TestNetworkMethods
{
    
    private Network network;
    private Configuration config;
    
    [SetUp]
    public void Setup()
    {
        config = new Configuration();
        config.InputCount = 3;
        config.OutputCount = 1;
        network = new Network(config);
    }
    
    // [Test]
    
    
}