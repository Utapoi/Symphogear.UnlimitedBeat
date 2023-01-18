using Sirenix.OdinInspector;
using Symphogear.Common;
using Symphogear.Events;
using Symphogear.Score;
using Symphogear.Timeline.Clips;
using System;
using System.Linq;
using UnityEngine;

namespace Symphogear.Notes
{
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

        [Title("References", "General", TitleAlignments.Split)]
        public DspTime DspTime;

        public AudioSource AudioSource;

        public ScoreSettings AccuracySettings;

        [Title("Events", "General", TitleAlignments.Split)]
        public event Action<NoteEventArgs> OnNoteTriggered;

        public event Action<NoteEventArgs> OnInitialize;

        public event Action<NoteEventArgs> OnReset;

        public event Action<NoteEventArgs> OnActivate;

        public event Action<NoteEventArgs> OnDeactivate;

        [Title("Settings", "Update", TitleAlignments.Split)]
        public bool UpdateWithTimeline = true;

        [Title("State", "Info")]
        public ActiveState State;

        public NoteClipInfo NoteClipInfo;

        [Title("Timings", "Info")]
        public double CurrentTime =>
            UpdateWithTimeline || !Application.isPlaying
                ? NoteClipInfo.SongDirector.PlayableDirector.time
                : DspTime.AdaptiveTime - NoteClipInfo.SongDirector.DspSongStartTime;

        public double ActualInitializeTime;

        public double ActualActivateTime;

        public double TimeFromActivate => CurrentTime - NoteClipInfo.Start;

        public double TimeFromDeactivate => CurrentTime - NoteClipInfo.End;

        protected bool IsTriggered;

        #endregion

        #region Behaviour Setup

        public override bool Initialize()
        {
            if (DspTime == null)
            {
                DspTime = FindObjectOfType<DspTime>();
            }

            InitializeComponent(ref AudioSource);

            return base.Initialize();
        }

        public virtual void Setup(NoteClipInfo clipInfo)
        {
            NoteClipInfo = clipInfo;
            ActualInitializeTime = CurrentTime;
            ActualActivateTime = -1;
            State = ActiveState.PreActive;

            transform.rotation = NoteClipInfo.SongTrack.StartPoint.rotation;
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
        protected abstract void InternalUpdate(double timeFromStart, double timeFromEnd);

        #endregion

        #region Notes

        protected virtual void ActivateNote()
        {
            State = ActiveState.Active;
            NoteClipInfo.SongTrack.Add(this);
            ActualActivateTime = CurrentTime;
        }

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
