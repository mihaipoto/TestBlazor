namespace TestBlazor;

public class MyNetworkDevice(string description, uint receivedPackets) 
{

    public string Description { get; init; } = description;
    public uint ReceivedPackets { get; set; } = receivedPackets;
    
};
