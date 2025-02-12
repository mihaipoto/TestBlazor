using System.Collections.Concurrent;
using System.ComponentModel;

namespace TestBlazor
{
    public interface ITrafficClassifier : IHostedService, INotifyPropertyChanged
    {
       

        List<MyPacket> WhitelistedPackets { get; }

        MyFixedSizeConcurrentQueue<MyConnection> LastRedConnections { get; }
        MyFixedSizeConcurrentQueue<MyConnection> LastGreenConnections { get; }
        string InitialisationError { get; }
        List<MyNetworkDevice> MyNetworkDevices { get; }

        event PropertyChangedEventHandler? PropertyChanged;

        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}