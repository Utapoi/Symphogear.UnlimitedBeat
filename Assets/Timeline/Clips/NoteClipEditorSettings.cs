using System;
using UnityEngine;

namespace Symphogear.Timeline.Clips
{
    [Serializable]
    public class NoteClipEditorSettings
    {
        public Texture Center;

        public Texture Start;

        public Texture End;

        public Color Color = new(0, 0, 0, 255);
    }
}
