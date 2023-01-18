using Symphogear.Timeline.Clips;
using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace Symphogear.Notes
{
    [Serializable]
    [CreateAssetMenu(fileName = "Note Settings", menuName = "Symphogear/Notes/Settings", order = 1)]
    public class NoteSettings : ScriptableObject
    {
        public enum ClipDurationType
        {
            Free,
            Crochet,
            HalfCrochet,
            QuarterCrochet,
            ScaledCrochet,
        }

        public GameObject Prefab;

        public ClipDurationType ClipDuration = ClipDurationType.Crochet;

        public float ScaledCrochetValue;

        public AudioClip HitSound;

#if UNITY_EDITOR
        [SerializeField]
        public NoteClipEditorSettings EditorSettings;
#endif


        public virtual void SetClipDuration(NoteClip noteClip, TimelineClip clip)
        {
            if (ClipDuration == ClipDurationType.Free) { return; }

            if (ClipDuration == ClipDurationType.HalfCrochet)
            {
                clip.duration = noteClip.NoteClipInfo.SongDirector.HalfCrochet;
                return;
            }

            if (ClipDuration == ClipDurationType.Crochet)
            {
                clip.duration = noteClip.NoteClipInfo.SongDirector.Crochet;
                return;
            }

            if (ClipDuration == ClipDurationType.QuarterCrochet)
            {
                clip.duration = noteClip.NoteClipInfo.SongDirector.QuarterCrochet;
                return;
            }

            if (ClipDuration == ClipDurationType.ScaledCrochet)
            {
                clip.duration = noteClip.NoteClipInfo.SongDirector.Crochet * ScaledCrochetValue;
                return;
            }
        }
    }
}
