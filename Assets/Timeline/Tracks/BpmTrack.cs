using Symphogear.Timeline.Clips;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace Symphogear.Timeline.Tracks
{
    [TrackColor(192f / 255f, 24f / 255f, 49f / 255f)]
    [DisplayName("Symphogear/BPM Track")]
    [TrackClipType(typeof(BpmClip))]
    public class BpmTrack : TrackAsset
    {
        public BpmMarkerEditorSettings OnBeatSettings;

        public BpmMarkerEditorSettings OffBeatSettings;

        public BpmMarkerEditorSettings GetMarkerEditorSettings(BpmMarker marker)
        {
            if (OnBeatSettings != null && marker.Id == OnBeatSettings.Id)
            {
                return OnBeatSettings;
            }

            if (OffBeatSettings != null && marker.Id == OffBeatSettings.Id)
            {
                return OffBeatSettings;
            }

            return default;
        }
    }
}
