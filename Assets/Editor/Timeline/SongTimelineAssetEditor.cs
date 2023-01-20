using Symphogear.Timeline;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Symphogear.Editor.Timeline
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(SongTimelineAsset), true)]
    public class SongTimelineAssetEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            container.Add(new IMGUIContainer(OnInspectorGUI));

            var button = new Button();
            button.style.marginTop = 30;
            button.text = "Open In Song Director";
            button.clicked += () =>
            {
                var director = FindObjectOfType<SongDirector>();
                if (director == null)
                {
                    Debug.LogWarning("The Song Director could not be found in the scene.");
                    return;
                }

                director.PlayableDirector.playableAsset = target as SongTimelineAsset;
                director.UpdateBpm();

                Selection.SetActiveObjectWithContext(director.gameObject, director.gameObject);
            };

            container.Add(button);

            return container;
        }
    }
}
