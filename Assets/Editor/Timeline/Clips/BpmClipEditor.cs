using Symphogear.Timeline.Clips;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace Symphogear.Editor.Timeline.Clips
{
    [CustomTimelineEditor(typeof(BpmClip))]
    public class BpmClipEditor : ClipEditor
    {
        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            var clipOptions = base.GetClipOptions(clip);
            clipOptions.highlightColor = Color.red;

            return clipOptions;
        }
    }
}
