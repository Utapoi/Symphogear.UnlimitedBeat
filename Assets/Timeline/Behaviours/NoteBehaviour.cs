// Copyright (c) Utapoi Ltd <contact@utapoi.moe>

using System;
using Symphogear.Notes;
using Symphogear.Timeline.Clips;
using UnityEngine.Playables;

namespace Symphogear.Timeline.Behaviours
{
    /// <summary>
    /// A custom <see cref="PlayableBehaviour" /> used to represents a <see cref="Notes.Note"/> in a <see cref="SongTimelineAsset"/>.
    /// </summary>
    [Serializable]
    public class NoteBehaviour : PlayableBehaviour
    {
        /// <summary>
        /// The <see cref="Notes.NoteSettings"/> of this <see cref="NoteBehaviour"/>.
        /// </summary>
        public NoteSettings NoteSettings;

        /// <summary>
        /// The <see cref="Clips.NoteClip"/> of this <see cref="NoteBehaviour"/>.
        /// </summary>
        public NoteClip NoteClip;

        /// <summary>
        /// The <see cref="Clips.NoteClipInfo"/> of this <see cref="NoteBehaviour"/>.
        /// </summary>
        public NoteClipInfo NoteClipInfo => NoteClip.NoteClipInfo;

        /// <summary>
        /// The <see cref="Notes.Note"/> of this <see cref="NoteBehaviour"/>.
        /// </summary>
        public Note Note;

        /// <summary>
        /// Indicates whether or not the <see cref="Notes.Note"/> was spawned.
        /// </summary>
        public bool IsNoteSpawned;

        /// <inheritdoc cref="PlayableBehaviour.ProcessFrame(Playable, FrameData, object)"/>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var timelineCurrentTime = (playable.GetGraph().GetResolver() as PlayableDirector).time;
            var inputCount = playable.GetInputCount();

            for (var i = 0; i < inputCount; i++)
            {
                var notePlayable = (ScriptPlayable<NoteBehaviour>)playable.GetInput(i);
                var behaviour = notePlayable.GetBehaviour();

                behaviour.Update(notePlayable, info, playerData, timelineCurrentTime);
            }
        }

        /// <summary>
        /// Update the <see cref="NoteBehaviour" /> each time the <see cref="ProcessFrame(Playable, FrameData, object)"/> method is called.
        /// </summary>
        /// <param name="playable">The playable.</param>
        /// <param name="info">The information about the current frame.</param>
        /// <param name="playerData">The user data out the <see cref="ScriptPlayableOutput"/>.</param>
        /// <param name="timelineCurrentTime">The current time of the timeline.</param>
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

        /// <summary>
        /// Instantiate a new <see cref="Note"/> based on the <see cref="NoteSettings" /> and <see cref="NoteClip"/> of this <see cref="NoteBehaviour"/>.
        /// </summary>
        private void SpawnNote()
        {
            if (IsNoteSpawned)
                return;

            Note = NoteClipInfo.SongDirector.SpawnNote(NoteSettings, NoteClip);
            IsNoteSpawned = true;
        }

        /// <summary>
        /// Destroy the previously instantiated <see cref="Note"/>.
        /// </summary>
        private void RemoveNote()
        {
            if (!IsNoteSpawned)
                return;

            NoteClipInfo.SongDirector.DestroyNote(Note);
            IsNoteSpawned = false;
        }
    }
}
