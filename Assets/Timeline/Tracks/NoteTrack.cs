using Symphogear.Timeline.Behaviours;
using Symphogear.Timeline.Clips;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Symphogear.Timeline.Tracks
{
    [TrackColor(248f / 255f, 152f / 255f, 0f)]
    [DisplayName("Symphogear/Note Track")]
    [TrackClipType(typeof(NoteClip))]
    public class NoteTrack : TrackAsset
    {
        public int TrackId;

        protected SongDirector SongDirector;

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject gameObject, int inputCount)
        {
            var playable = ScriptPlayable<NoteBehaviour>.Create(graph, inputCount);

            if (SongDirector == null)
            {
                if (!gameObject.TryGetComponent(out SongDirector))
                {
                    Debug.LogError("The Song Director is missing from the Song Track Binding.");
                    return playable;
                }
            }

            SongDirector.UpdateBpm();

            foreach (var clip in m_Clips)
            {
                var noteClip = clip.asset as NoteClip;

                noteClip.NoteClipInfo = new NoteClipInfo(
                    SongDirector,
                    noteClip,
                    TrackId,
                    clip.start,
                    clip.duration
                );

                SetClipDuration(noteClip, clip);
            }

            return playable;
        }

        protected virtual void SetClipDuration(NoteClip noteClip, TimelineClip clip)
        {
            if (noteClip.NoteBehaviour.NoteSettings == null)
                return;

            noteClip.NoteBehaviour.NoteSettings.SetClipDuration(noteClip, clip);
        }
    }
}