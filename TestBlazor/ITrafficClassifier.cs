using System.Collections.Concurrent;
using System.ComponentModel;

namespace TestBlazor
{
	public interface ITrafficClassifier : IHostedService, INotifyPropertyChanged
	{
		ConcurrentDictionary<string, MyConnection> Connections { get; }
		string InitialisationError { get; }
		IEnumerable<MyNetworkDevice> MyNetworkDevices { get; }

		event PropertyChangedEventHandler? PropertyChanged;

		Task StartAsync(CancellationToken cancellationToken);
		Task StopAsync(CancellationToken cancellationToken);
	}
}