using System;
using UnityEngine.Events;

namespace Symphogear.Events
{
    [Serializable]
    public struct DefaultEventArgs
    {
        public static DefaultEventArgs Empty => new();
    }

    [Serializable]
    public class DefaultEvent : UnityEvent<DefaultEventArgs> { }
}
