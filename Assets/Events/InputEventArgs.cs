using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Symphogear.Events
{
    [Serializable]
    public struct KeyEventArgs
    {
        public Key Key { get; set; }
    }


    [Serializable]
    public struct TouchEventArgs
    {
        public int TouchId { get; set; }

        public Vector3 Position { get; set; }
    }

    [Serializable]
    public struct SwipeEventArgs
    {
        public int TouchId { get; set; }

        public double Time { get; set; }

        public Vector3 StartPosition { get; set; }

        public Vector3 EndPosition { get; set; }
    }
}
