using System.Threading;

internal class FirebaseRequests
{
    private int _requestIds = 0;

    internal int NextId()
    {
        return Interlocked.Increment(ref _requestIds);
    }
}