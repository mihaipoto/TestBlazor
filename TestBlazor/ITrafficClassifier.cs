using System.Collections.Concurrent;
using System.ComponentModel;

namespace TestBlazor
{
    public interface ITrafficClassifier : IHostedService, INotifyPropertyChanged
    {
        IEnumerable<KeyValuePair<string, MyConnection>> YellowConnections { get; }

        List<string> WhitelistedConnectionKeys { get; }

        MyFixedSizeConcurrentQueue<MyConnection> LastRedConnections { get; }
        MyFixedSizeConcurrentQueue<MyConnection> LastGreenConnections { get; }
        string InitialisationError { get; }
        IEnumerable<MyNetworkDevice> MyNetworkDevices { get; }

        event PropertyChangedEventHandler? PropertyChanged;

        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}