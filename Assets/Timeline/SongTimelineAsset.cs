using Symphogear.Timeline.Tracks;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace Symphogear.Timeline
{
    [Serializable]
    [CreateAssetMenu(fileName = "SongTimelineAsset", menuName = "Symphogear/Timeline/Song Timeline Asset", order = 1)]
    public class SongTimelineAsset : TimelineAsset
    {
        public float BeatsPerMinute;

        public AudioClip AudioClip;

#if UNITY_EDITOR

        private void OnEnable()
        {
            if (outputTrackCount != 0)
                return;

            CreateTrack(typeof(AudioTrack), null, "Song");
            (CreateTrack(typeof(NoteTrack), null, "Song Track #1") as NoteTrack).TrackId = 0;
            (CreateTrack(typeof(NoteTrack), null, "Song Track #2") as NoteTrack).TrackId = 1;
            (CreateTrack(typeof(NoteTrack), null, "Song Track #3") as NoteTrack).TrackId = 2;
            (CreateTrack(typeof(NoteTrack), null, "Song Track #4") as NoteTrack).TrackId = 3;
            (CreateTrack(typeof(NoteTrack), null, "Song Track #5") as NoteTrack).TrackId = 4;
            (CreateTrack(typeof(NoteTrack), null, "Song Track #6") as NoteTrack).TrackId = 5;
            (CreateTrack(typeof(NoteTrack), null, "Song Track #7") as NoteTrack).TrackId = 6;
            CreateTrack(typeof(BpmTrack), null, "BPM Track");

            AssetDatabase.SaveAssets();
        }

#endif
    }
}
