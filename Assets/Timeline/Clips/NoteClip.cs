// Copyright (c) Utapoi Ltd <contact@utapoi.moe>

using System;
using Symphogear.Timeline.Behaviours;
using Symphogear.Tracks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Symphogear.Timeline.Clips
{
    [Serializable]
    public struct NoteClipInfo
    {
        public SongDirector SongDirector;

        public NoteClip Clip;

        public SongTrack SongTrack => SongDirector.SongTracks[SongTrackId];

        public double Start;

        public double Duration;

        public double End => Start + Duration;

        public int SongTrackId;

        public NoteClipInfo(SongDirector songDirector, NoteClip clip, int songTrackId, double start, double duration)
        {
            SongDirector = songDirector;
            Clip = clip;
            SongTrackId = songTrackId;
            Start = start;
            Duration = duration;
        }
    }

    public class NoteClip : PlayableAsset, ITimelineClipAsset
    {
        public NoteClipInfo NoteClipInfo;

        public NoteBehaviour NoteBehaviour = new();

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            NoteBehaviour.NoteClip = this;

            return ScriptPlayable<NoteBehaviour>.Create(graph, NoteBehaviour);
        }

        public void Copy(NoteClip _)
        {
        }
    }
}
