using Symphogear.Timeline.Clips;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace Symphogear.Editor.Timeline.Clips
{
    using Editor = UnityEditor.Editor;

    [CustomTimelineEditor(typeof(NoteClip))]
    public class NoteClipTimelineEditor : ClipEditor
    {
        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            base.OnCreate(clip, track, clonedFrom);

            var otherAsset = clonedFrom?.asset as NoteClip;

            (clip.asset as NoteClip).Copy(otherAsset);
            clip.displayName = " ";
        }

        public override void OnClipChanged(TimelineClip clip)
        {
            base.OnClipChanged(clip);
        }

        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            base.DrawBackground(clip, region);

            var noteClip = clip.asset as NoteClip;

            if (noteClip == null)
                return;

            var clipEditorSettings = noteClip.NoteBehaviour.NoteSettings.EditorSettings;
            
            if (clipEditorSettings == null)
                return;

            var regionHalfHeight = region.position.height / 2;
            var yPosition = region.position.position.y + regionHalfHeight / 2;
            var iconSize = new Vector2(regionHalfHeight, regionHalfHeight);
            var startRegion = new Rect(
                region.position.position.x,
                yPosition,
                iconSize.x,
                iconSize.y);
            var centerRegion = new Rect(
                region.position.position.x + region.position.width / 2f - iconSize.x / 2,
                yPosition,
                iconSize.x,
                iconSize.y);
            var endRegion = new Rect(
                region.position.position.x + region.position.width - iconSize.x,
                yPosition,
                iconSize.x,
                iconSize.y);
            var backgroundRegion = new Rect(
                region.position.position.x,
                yPosition,
                region.position.width,
                iconSize.y);

            EditorGUI.DrawRect(backgroundRegion, clipEditorSettings.Color);

            Color previousGuiColor = GUI.color;
            GUI.color = Color.clear;

            if (clipEditorSettings.Start != null)
            {
                EditorGUI.DrawTextureTransparent(startRegion, clipEditorSettings.Start);
            }

            if (clipEditorSettings.Center != null)
            {
                EditorGUI.DrawTextureTransparent(centerRegion, clipEditorSettings.Center);
            }

            if (clipEditorSettings.End != null)
            {
                EditorGUI.DrawTextureTransparent(endRegion, clipEditorSettings.End);
            }

            GUI.color = previousGuiColor;
        }
    }


    [CustomEditor(typeof(NoteClip), true)]
    [CanEditMultipleObjects]
    public class NoteClipEditor : Editor
    {
    }
}
