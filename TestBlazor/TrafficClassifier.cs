using PacketDotNet;
using SharpPcap;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace TestBlazor;

public record MyNetworkDevice(string Description, ICaptureStatistics Statistics);


public class TrafficClassifier : ITrafficClassifier
{
	private readonly TimeSpan PacketTimeout = TimeSpan.FromSeconds(30); // Timeout for TCP connections

	public event PropertyChangedEventHandler? PropertyChanged;
	public ConcurrentDictionary<string, MyConnection> Connections { get; } = new();

	private readonly CaptureDeviceList _networkDevices = CaptureDeviceList.Instance;
	public IEnumerable<MyNetworkDevice> MyNetworkDevices => _networkDevices.Select(device => new MyNetworkDevice(device.Description, device.Statistics));

	public string InitialisationError { get; private set; } = string.Empty;

	private void Device_OnPacketArrival(object sender, PacketCapture e)
	{
		var rawPacket = e.GetPacket();
		var packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
		var ipPacket = packet.Extract<IPPacket>();
		if (ipPacket == null) return;
		var tcpPacket = packet.Extract<TcpPacket>();
		if (tcpPacket != null)
		{
			HandlePacket(new(tcpPacket, ipPacket));
			return;
		}
		var udpPacket = packet.Extract<UdpPacket>();
		if (udpPacket != null)
		{
			HandlePacket(new(udpPacket, ipPacket));
			return;
		}
		var icmpPacket = packet.Extract<IcmpV4Packet>();
		if (icmpPacket != null)
		{
			HandlePacket(new(ipPacket, ConnectionType.Icmp));
			return;
		}
		HandlePacket(new());
	}
	private void HandlePacket(MyPacket packet)
	{
		if (Connections.TryGetValue(packet.ResponseKey, out MyConnection? value))
		{
			value.SetResponse();
			PropertyChanged?.Invoke(this, new(nameof(Connections)));
		}
		else
		{
			if (Connections.TryAdd(packet.RequestKey, new MyConnection(packet)))
			{
				PropertyChanged?.Invoke(this, new(nameof(Connections)));
			}

		}
	}
	private async Task CleanupStaleEntries<T>(ConcurrentDictionary<string, T> dictionary, TimeSpan timeout, CancellationToken cancellationToken)
		where T : MyConnection
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			await Task.Delay(5000, cancellationToken); // Cleanup every 5 seconds

			var now = DateTime.Now;
			var staleEntries = dictionary
				.Where(kvp => (now - kvp.Value.LastActivity) > timeout)
				.Select(kvp => kvp.Key)
				.ToList();

			foreach (var key in staleEntries)
			{
				if (dictionary.TryRemove(key, out var staleCommunication))
				{
					PropertyChanged?.Invoke(this, new(nameof(Connections)));
					Console.WriteLine($"Request timed out: {key} (Last Activity: {staleCommunication.LastActivity})");
				}
			}

		}
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		try
		{
			if (_networkDevices.Count < 1)
			{
				throw new Exception("No network devices found. Ensure Npcap is installed.");
			}

			_ = CleanupStaleEntries(Connections, PacketTimeout, cancellationToken);

			foreach (var device in _networkDevices)
			{
				device.OnPacketArrival += Device_OnPacketArrival;
				device.Open(DeviceModes.Promiscuous, 1000);
				device.StartCapture();
			}
		}
		catch (Exception ex)
		{
			InitialisationError = ex.Message;
		}
		_ = DoWork(cancellationToken);
		return Task.CompletedTask;

	}


	private async Task DoWork(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await Task.Delay(1000, stoppingToken);
		}
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		foreach (var device in _networkDevices)
		{
			device.StopCapture();
			device.Close();
		}
		return Task.CompletedTask;
	}
}
