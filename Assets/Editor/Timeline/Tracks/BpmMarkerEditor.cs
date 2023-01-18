using Symphogear.Timeline.Tracks;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.Timeline;
using UnityEngine;

namespace Symphogear.Editor.Timeline.Tracks
{
    [CustomTimelineEditor(typeof(BpmMarker))]
    public class BpmMarkerEditor : MarkerEditor
    {
        public static readonly string DefaultOnBeatMarkerSettings = "Assets/Resources/Timeline/Editor/DefaultOnBeatMarkerSettings.asset";
        public static readonly string DefaultOffBeatMarkerSettings = "Assets/Resources/Timeline/Editor/DefaultOffBeatMarkerSettings.asset";

        public override void OnCreate(IMarker marker, IMarker clonedFrom)
        {
            base.OnCreate(marker, clonedFrom);

            (marker as BpmMarker).Copy(clonedFrom as BpmMarker);
        }

        public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
        {
            return base.GetMarkerOptions(marker);
        }

        public override void DrawOverlay(IMarker marker, MarkerUIStates uiState, MarkerOverlayRegion region)
        {
            var bpmMarker = marker as BpmMarker;
            var bpmTrack = bpmMarker.parent as BpmTrack;

            if (bpmTrack == null)
            {
                base.DrawOverlay(marker, uiState, region);
                return;
            }

            var markerEditorSettings = bpmTrack.GetMarkerEditorSettings(bpmMarker);
            
            if (markerEditorSettings == null)
            {
                if (bpmMarker.Id == 0)
                {
                    markerEditorSettings = (BpmMarkerEditorSettings)AssetDatabase.LoadAssetAtPath(DefaultOnBeatMarkerSettings, typeof(BpmMarkerEditorSettings));
                }
                else if (bpmMarker.Id == 1)
                {
                    markerEditorSettings = (BpmMarkerEditorSettings)AssetDatabase.LoadAssetAtPath(DefaultOffBeatMarkerSettings, typeof(BpmMarkerEditorSettings));
                }

                if (markerEditorSettings == null)
                {
                    base.DrawOverlay(marker, uiState, region);
                    return;
                }
            }

            var iconsSize = new Vector2(18, 18);

            var markerRegion = new Rect(
                region.markerRegion.center.x - iconsSize.x / 2,
                region.markerRegion.position.y,
                iconsSize.x,
                iconsSize.y);
            markerRegion = uiState != MarkerUIStates.Collapsed ?
                markerRegion :
                new Rect(markerRegion.position, new Vector2(markerRegion.size.x, markerRegion.size.y / 2f));

            var texture = markerEditorSettings.DefaultTexture;

            Color previousGuiColor = GUI.color;
            GUI.color = markerEditorSettings.DefaultColor;

            if (texture != null)
            {
                if (uiState == MarkerUIStates.Selected)
                {
                    GUI.color = markerEditorSettings.SelectedColor;
                }
                GUI.DrawTexture(markerRegion, texture);
            }

            GUI.color = previousGuiColor;
        }
    }
}
