using Symphogear.Notes;
using Symphogear.Timeline.Clips;
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Symphogear.Timeline.Behaviours
{
    [Serializable]
    public class NoteBehaviour : PlayableBehaviour
    {
        public NoteSettings NoteSettings;

        public NoteClip NoteClip;

        public NoteClipInfo NoteClipInfo => NoteClip.NoteClipInfo;

        public Note Note;

        public bool IsNoteSpawned;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            double timelineCurrentTime = (playable.GetGraph().GetResolver() as PlayableDirector).time;
            int inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++)
            {
                ScriptPlayable<NoteBehaviour> notePlayable = (ScriptPlayable<NoteBehaviour>)playable.GetInput(i);
                NoteBehaviour behaviour = notePlayable.GetBehaviour();
                behaviour.Update(notePlayable, info, playerData, timelineCurrentTime);
            }
        }

        protected virtual void Update(Playable playable, FrameData info, object playerData, double timelineCurrentTime)
        {
#if UNITY_EDITOR
            // Update the BPM in case it was changed in the inspector.
            NoteClipInfo.SongDirector.UpdateBpm();
#endif

            var globalClipStartTime = timelineCurrentTime - NoteClipInfo.Start;
            var globalClipEndTime = timelineCurrentTime - NoteClipInfo.End;
            var timeRange = NoteClipInfo.SongDirector.SpawnTimeRange;

            if (!(globalClipStartTime >= -timeRange.x) || !(globalClipEndTime < timeRange.y))
            {
                if (IsNoteSpawned)
                {
                    RemoveNote();
                }

                return;
            }

            if (IsNoteSpawned == false)
            {
                SpawnNote();
            }


            Note.TimelineUpdate(globalClipStartTime, globalClipEndTime);
        }

        private void SpawnNote()
        {
            if (IsNoteSpawned)
                return;

            Note = NoteClipInfo.SongDirector.SpawnNote(NoteSettings, NoteClip);
            IsNoteSpawned = true;
        }

        private void RemoveNote()
        {
            if (!IsNoteSpawned)
                return;

            NoteClipInfo.SongDirector.DestroyNote(Note);
            IsNoteSpawned = false;
        }
    }
}
