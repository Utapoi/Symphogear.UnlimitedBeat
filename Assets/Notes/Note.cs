// Copyright (c) Utapoi Ltd <contact@utapoi.moe>

using System;
using Sirenix.OdinInspector;
using Symphogear.Common;
using Symphogear.Events;
using Symphogear.Timeline.Clips;
using UnityEngine;

namespace Symphogear.Notes
{
    /// <summary>
    /// The base class for all types of <see cref="Note"/>.
    /// </summary>
    /// <remarks>
    /// Defines some basic behaviour and logic common for all notes.
    /// </remarks>
    public abstract class Note : SymphogearBehaviour
    {
        #region Properties

        public enum ActiveState
        {
            Disabled,
            PreActive,
            Active,
            PostActive
        }

        /// <summary>
        /// A reference to the <see cref="Common.DspTime"/> behaviour.
        /// </summary>
        /// <remarks>
        /// This is used to calculate the current position of the <see cref="Note"/> based on its duration and start time.
        /// </remarks>
        [Title("References", "General", TitleAlignments.Split)]
        public DspTime DspTime;

        /// <summary>
        /// An event triggered when the note is tapped.
        /// </summary>
        [Title("Events", "General", TitleAlignments.Split)]
        public event Action<NoteEventArgs> OnNoteTriggered;

        /// <summary>
        /// Enable or disable note update in the editor.
        /// </summary>
        [Title("Settings", "General", TitleAlignments.Split)]
        public bool UpdateWithTimeline = true;

        /// <summary>
        /// Gets or sets the scale of the <see cref="Note"/>.
        /// </summary>
        public Vector3 Scale;

        /// <summary>
        /// The state of the <see cref="Note"/> during its lifetime.
        /// </summary>
        [Title("State", "Info")]
        public ActiveState State;

        /// <summary>
        /// The information of the associated <see cref="Timeline.Clips.NoteClipInfo"/> struct.
        /// </summary>
        public NoteClipInfo NoteClipInfo;

        /// <summary>
        /// Gets the current time of the song.
        /// </summary>
        [Title("Timings", "Info")]
        public double CurrentTime =>
            UpdateWithTimeline || !Application.isPlaying
                ? NoteClipInfo.SongDirector.PlayableDirector.time
                : DspTime.AdaptiveTime - NoteClipInfo.SongDirector.DspSongStartTime;

        /// <summary>
        /// The time at which the <see cref="Note"/> was initialized.
        /// </summary>
        /// <remarks>
        /// This is calculated in <see cref="Initialize"/>.
        /// </remarks>
        public double ActualInitializeTime;

        /// <summary>
        /// The time at which the <see cref="Note"/> was activated.
        /// </summary>
        public double ActualActivateTime;

        /// <summary>
        /// Time remaining before the <see cref="Note"/> is activated.
        /// </summary>
        public double TimeFromActivate => CurrentTime - NoteClipInfo.Start;

        /// <summary>
        /// Time remaining before the <see cref="Note"/> is deactivated.
        /// </summary>
        public double TimeFromDeactivate => CurrentTime - NoteClipInfo.End;

        /// <summary>
        /// Indicates whether or not the <see cref="Note"/> was triggered by the player.
        /// </summary>
        protected bool IsTriggered;

        #endregion

        #region Behaviour Setup

        /// <inheritdoc cref="SymphogearBehaviour.Initialize"/>
        public override bool Initialize()
        {
            if (DspTime == null)
            {
                DspTime = FindObjectOfType<DspTime>();
            }

            return base.Initialize();
        }

        /// <summary>
        /// Configure the <see cref="Note"/> with its information from the <see cref="Timeline.SongTimelineAsset"/>.
        /// </summary>
        /// <param name="clipInfo">The <see cref="Timeline.Clips.NoteClipInfo"/> data associated with the <see cref="Note"/>.</param>
        public virtual void Setup(NoteClipInfo clipInfo)
        {
            NoteClipInfo = clipInfo;
            ActualInitializeTime = CurrentTime;
            ActualActivateTime = -1;
            State = ActiveState.PreActive;
        }

        #endregion

        #region Events

        public virtual void OnKeyPressed(KeyEventArgs e)
        {
        }

        public virtual void OnKeyReleased(KeyEventArgs e)
        {
        }

        public virtual void OnTouchPressed(TouchEventArgs e)
        {
        }

        public virtual void OnTouchReleased(TouchEventArgs e)
        {
        }

        public virtual void OnSwipeTriggered(SwipeEventArgs e)
        {
        }

        #endregion

        #region Update

        public virtual void TimelineUpdate(double clipStartTime, double clipEndTime)
        {
            if (!UpdateWithTimeline && !Application.isPlaying)
                return;

            if (State == ActiveState.Active && clipEndTime >= 0)
            {
                DeactivateNote();
            }
            else if (State == ActiveState.PreActive && clipStartTime >= 0)
            {
                ActivateNote();
            }

            InternalUpdate(clipStartTime, clipEndTime);
        }

        protected virtual void Update()
        {
            if (UpdateWithTimeline && !Application.isPlaying)
                return;

            if (State == ActiveState.Active && TimeFromDeactivate >= 0)
            {
                DeactivateNote();
            }
            else if (State == ActiveState.PreActive && TimeFromActivate >= -0.15)
            {
                ActivateNote();
            }

            InternalUpdate(TimeFromActivate, TimeFromDeactivate);
        }
        /// <summary>
        /// Utility method to update the <see cref="Note"/> during Runtime and in the Editor.
        /// </summary>
        /// <param name="timeFromStart">The time remaining before the note activation.</param>
        /// <param name="timeFromEnd">The time remaining before the note deactivation.</param>
        protected virtual void InternalUpdate(double timeFromStart, double timeFromEnd)
        {
        }

        #endregion

        #region Notes

        /// <summary>
        /// Activate the note and assign it to the corresponding <see cref="Tracks.SongTrack"/>
        /// </summary>
        protected virtual void ActivateNote()
        {
            State = ActiveState.Active;
            NoteClipInfo.SongTrack.Add(this);
            ActualActivateTime = CurrentTime;
        }

        /// <summary>
        /// Deactivate the note and remove it from the corresponding <see cref="Tracks.SongTrack"/>
        /// </summary>
        protected virtual void DeactivateNote()
        {
            State = ActiveState.PostActive;
            NoteClipInfo.SongTrack.Remove(this);
        }

        #endregion

        #region Invoke Actions

        protected virtual void InvokeOnNoteTriggered(NoteEventArgs e)
        {
            OnNoteTriggered?.Invoke(e);
        }

        #endregion
    }
}
