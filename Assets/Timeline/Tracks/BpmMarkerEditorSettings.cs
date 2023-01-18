using System;
using UnityEngine;

namespace Symphogear.Timeline.Tracks
{
    [Serializable]
    [CreateAssetMenu(fileName = "BpmMarkerEditorSettings", menuName = "Symphogear/Timeline/BPM Marker Editor Settings", order = 2)]
    public class BpmMarkerEditorSettings : ScriptableObject
    {
        public int Id;

        public Texture DefaultTexture;

        public Texture CollapsedTexture;

        public Color DefaultColor;

        public Color SelectedColor;
    }
}
