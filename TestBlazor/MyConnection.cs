namespace TestBlazor;

public class MyConnection(MyPacket packet) :IEquatable<MyConnection>
{
	public void SetResponse()
	{
		ResponseReceived = true;
		LastActivity = DateTime.Now;
		ConnectionKey = packet.RequestKey;
    }

	public bool Equals(MyConnection? other)
	 => other is not null && ConnectionKey.Equals(other.ConnectionKey);

    public ConnectionType Type { get; set; } = packet.PacketType;
	public string Source { get; set; } = $"{packet.SourceIp}:{packet.SourcePort}";
	public string Destination { get; set; } = $"{packet.DestinationIp} : {packet.DestinationPort}";
	public bool ResponseReceived { get; set; } = false;

	public string ConnectionKey { get; set; } = string.Empty;
    public DateTime LastActivity { get; set; } = DateTime.Now;
}
