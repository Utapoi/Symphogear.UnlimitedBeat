using Symphogear.Timeline;
using Symphogear.Timeline.Tracks;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Symphogear.Editor.Timeline.Tracks
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(BpmTrack))]
    [CanEditMultipleObjects]
    public class BpmTrackEditor : Editor
    {
        protected BpmTrack BpmTrack;

        protected Button SetBpmButton;
        protected Button ClearMarkerButton;
        protected FloatField BpmField;
        protected FloatField OffsetField;
        protected FloatField DurationField;
        protected Toggle WithOnBeatMarker;
        protected Toggle WithOffBeatMarker;
        protected VisualElement BpmContainer;

        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            BpmTrack = target as BpmTrack;

            var UIElementFields = CreateUIElementInspectorGUI(serializedObject, new List<string>() { "m_StartItem" });

            BpmContainer = new VisualElement();

            BpmField = new FloatField("BPM")
            {
                value = (BpmTrack.timelineAsset as SongTimelineAsset)?.BeatsPerMinute ?? 120
            };

            OffsetField = new FloatField("Offset")
            {
                value = 0
            };

            DurationField = new FloatField("Duration")
            {
                value = (float)BpmTrack.timelineAsset.duration
            };

            WithOnBeatMarker = new Toggle("With On Beat Marker")
            {
                value = true
            };

            WithOffBeatMarker = new Toggle("With Off Beat Marker")
            {
                value = true
            };

            SetBpmButton = new Button
            {
                text = "Set BPM"
            };

            SetBpmButton.clickable.clicked += SetBpm;

            ClearMarkerButton = new Button
            {
                text = "Clear Markers"
            };

            ClearMarkerButton.clickable.clicked += ClearMarkers;

            BpmContainer.Add(BpmField);
            BpmContainer.Add(OffsetField);
            BpmContainer.Add(DurationField);
            BpmContainer.Add(WithOnBeatMarker);
            BpmContainer.Add(WithOffBeatMarker);
            BpmContainer.Add(SetBpmButton);
            BpmContainer.Add(ClearMarkerButton);

            var addTempoFoldout = new Foldout
            {
                text = "Add BPM"
            };

            addTempoFoldout.Add(BpmContainer);
            container.Add(UIElementFields);
            container.Add(addTempoFoldout);

            return container;
        }

        private void SetBpm()
        {
            var step = 60d / BpmField.value;
            var duration = DurationField.value;
            var stepCount = duration / step;

            if (WithOnBeatMarker.value)
            {
                for (double i = 0; i < stepCount; i++)
                {
                    var tempoMarker = BpmTrack.CreateMarker<BpmMarker>((step * i) + OffsetField.value);
                    tempoMarker.name = "BPM On Beat Marker";
                    tempoMarker.Id = BpmTrack.OnBeatSettings?.Id ?? 0;
                }
            }

            var halfStep = step / 2f;

            if (WithOffBeatMarker.value)
            {
                for (double i = 0; i < stepCount; i++)
                {
                    var tempoMarker = BpmTrack.CreateMarker<BpmMarker>((step * i) - halfStep + OffsetField.value);
                    tempoMarker.name = "BPM Off Beat Marker";
                    tempoMarker.Id = BpmTrack.OffBeatSettings?.Id ?? 1;
                }
            }

            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved | RefreshReason.WindowNeedsRedraw);
        }

        private void ClearMarkers()
        {
            var markers = BpmTrack.GetMarkers();
            foreach (var marker in markers)
            {
                BpmTrack.DeleteMarker(marker);
            }
        }

        private static VisualElement CreateUIElementInspectorGUI(SerializedObject serializedObject, List<string> propertiesToExclude)
        {
            var container = new VisualElement();

            var fields = GetVisibleSerializedFields(serializedObject.targetObject.GetType());
            for (int i = 0; i < fields.Length; ++i)
            {
                var field = fields[i];

                // Do not draw HideInInspector fields or excluded properties.
                if (field.GetCustomAttributes(typeof(HideInInspector), false).Length > 0 ||
                    (propertiesToExclude != null &&
                     propertiesToExclude.Contains(field.Name)))
                {
                    continue;
                }

                //Debug.Log(field.Name);
                var serializedProperty = serializedObject.FindProperty(field.Name);

                var propertyField = new PropertyField(serializedProperty);

                container.Add(propertyField);
            }


            return container;
        }

        private static FieldInfo[] GetVisibleSerializedFields(Type T)
        {
            List<FieldInfo> infoFields = new();

            var publicFields = T.GetFields(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < publicFields.Length; i++)
            {
                if (publicFields[i].GetCustomAttribute<HideInInspector>() == null)
                {
                    infoFields.Add(publicFields[i]);
                }
            }

            var privateFields = T.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            for (int i = 0; i < privateFields.Length; i++)
            {
                if (privateFields[i].GetCustomAttribute<SerializeField>() != null)
                {
                    infoFields.Add(privateFields[i]);
                }
            }

            return infoFields.ToArray();
        }
    }
}
