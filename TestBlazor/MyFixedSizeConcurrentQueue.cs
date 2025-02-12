using System.Collections.Concurrent;
using System.ComponentModel;

namespace TestBlazor;



public class MyFixedSizeConcurrentQueue<T>(int size) : ConcurrentQueue<T>, INotifyPropertyChanged where T : class, IEquatable<T>
{
    private readonly object syncObject = new object();

    public int Size { get; private set; } = size;

    public event PropertyChangedEventHandler? PropertyChanged;

    public new void Enqueue(T obj)
    {
        if(!this.Contains(obj))
        {
            base.Enqueue(obj);
            lock (syncObject)
            {
                while (base.Count > Size)
                {
                    base.TryDequeue(out _);
                }
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }
        
    }
}
