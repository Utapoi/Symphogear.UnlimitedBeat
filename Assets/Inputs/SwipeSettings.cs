using System;
using UnityEngine;

namespace Symphogear.Inputs
{
    [Serializable]
    [CreateAssetMenu(fileName = "SwipeSettings", menuName = "Symphogear/Inputs/Swipe Settings", order = 1)]
    public class SwipeSettings : ScriptableObject
    {
        public float MinimumSwipeDistance = 0.01f;

        public double MaximumInputDuration = 5f;
    }
}
