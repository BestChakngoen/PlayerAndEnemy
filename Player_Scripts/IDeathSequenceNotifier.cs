using System;

namespace CoreSystem
{
    public interface IDeathSequenceNotifier
    {
        event Action OnDeathSequenceComplete;
    }
}