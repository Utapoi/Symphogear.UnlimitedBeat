using Symphogear.Notes;
using System;
using UnityEngine.Events;

namespace Symphogear.Events
{

    [Serializable]
    public struct NoteEventArgs
    {
        public static NoteEventArgs Empty => new();

        public Note Note { get; set; }

        public double DspTime { get; set; }

        public double DspTimeDifference { get; set; }

        public double DspTimeDifferencePercentage { get; set; }

        public bool IsMiss { get; set; }
    }

    [Serializable]
    public class NoteEvent : UnityEvent<NoteEventArgs> { }
}
