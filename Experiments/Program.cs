using System.Text.Json;
using System.Text.Json.Nodes;
using RtNeat;

var config = new Configuration()
{
    InputCount = 2,
    OutputCount = 1
};
var net = new Network(config);

for (int i = 0; i < 100; i++)
{
    net.Mutate();
}

Console.WriteLine(JsonSerializer.Serialize(net));