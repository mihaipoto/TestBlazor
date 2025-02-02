using PacketDotNet;

namespace TestBlazor;



/// <summary>
/// Represents a network packet with source and destination information.
/// </summary>
/// <param name="SourcePort">The source port of the packet.</param>
/// <param name="DestinationPort">The destination port of the packet.</param>
/// <param name="PacketType">The type of the packet (TCP, UDP, ICMP, etc.).</param>
public record MyPacket(ushort SourcePort = default, ushort DestinationPort = default, ConnectionType PacketType = ConnectionType.Unknown)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MyPacket"/> class using a TCP packet and an IP packet.
	/// </summary>
	/// <param name="tcpPacket">The TCP packet.</param>
	/// <param name="iPPacket">The IP packet.</param>
	public MyPacket(TcpPacket tcpPacket, IPPacket iPPacket) : this(tcpPacket.SourcePort, tcpPacket.DestinationPort, ConnectionType.Tcp)
	{
		SourceIp = iPPacket.SourceAddress.ToString();
		DestinationIp = iPPacket.DestinationAddress.ToString();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MyPacket"/> class using a UDP packet and an IP packet.
	/// </summary>
	/// <param name="udpPacket">The UDP packet.</param>
	/// <param name="iPPacket">The IP packet.</param>
	public MyPacket(UdpPacket udpPacket, IPPacket iPPacket) : this(udpPacket.SourcePort, udpPacket.DestinationPort, ConnectionType.Udp)
	{
		SourceIp = iPPacket.SourceAddress.ToString();
		DestinationIp = iPPacket.DestinationAddress.ToString();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MyPacket"/> class using an IP packet and a specified connection type.
	/// </summary>
	/// <param name="iPPacket">The IP packet.</param>
	/// <param name="packetType">The type of the packet (default is Unknown).</param>
	public MyPacket(IPPacket iPPacket, ConnectionType packetType = ConnectionType.Unknown) : this(PacketType: packetType)
	{
		SourceIp = iPPacket.SourceAddress.ToString();
		DestinationIp = iPPacket.DestinationAddress.ToString();
	}

	/// <summary>
	/// Gets or sets the source IP address of the packet.
	/// </summary>
	public string SourceIp { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the destination IP address of the packet.
	/// </summary>
	public string DestinationIp { get; set; } = string.Empty;

	/// <summary>
	/// Gets the request key for the packet, used for identifying the request.
	/// </summary>
	public string RequestKey => PacketType.Equals(ConnectionType.Icmp) ?
		$"{PacketType}-{SourceIp}:{DestinationIp}" :
		$"{PacketType}-{SourceIp}:{DestinationIp}-{SourcePort}:{DestinationPort}";

	/// <summary>
	/// Gets the response key for the packet, used for identifying the response.
	/// </summary>
	public string ResponseKey => PacketType.Equals(ConnectionType.Icmp) ?
		$"{PacketType}-{DestinationIp}:{SourceIp}" :
		$"{PacketType}-{DestinationIp}:{SourceIp}-{DestinationPort}:{SourcePort}";
}
