// Copyright (c) Utapoi Ltd <contact@utapoi.moe>

using Sirenix.OdinInspector;
using Symphogear.Events;
using Symphogear.Timeline.Clips;
using UnityEngine;

namespace Symphogear.Notes
{
    /// <summary>
    /// A <see cref="Note" /> that needs to be held and released at the right time.
    /// </summary>
    public class LongNote : Note
    {
        #region Properties

        /// <summary>
        /// A reference to the <see cref="Transform"/> of the start <see cref="Note"/> of this <see cref="LongNote"/>.
        /// </summary>
        [Title("References", "General", TitleAlignments.Split)]
        public Transform StartNote;

        /// <summary>
        /// A reference to the <see cref="Transform"/> of the end <see cref="Note"/> of this <see cref="LongNote"/>.
        /// </summary>
        public Transform EndNote;

        /// <summary>
        /// A reference to the <see cref="Sprite"/> that joins the start and end <see cref="Note"/>.
        /// </summary>
        public SpriteRenderer Line;

        /// <summary>
        /// Indicates whether or not we should remove and destroy the note when the player doesn't active it.
        /// </summary>
        [Title("Settings", "General", TitleAlignments.Split)]
        public bool RemoveNoteIfMissed = true;

        /// <summary>
        /// The moment the player start holding the <see cref="LongNote"/>.
        /// </summary>
        private double _StartHoldTimeOffset;

        /// <summary>
        /// Indicates whether or not the <see cref="LongNote"/> is being held.
        /// </summary>
        private bool _Holding;

        #endregion

        #region Behaviour Setup

        /// <inheritdoc cref="Note.Setup(NoteClipInfo)"/>
        public override void Setup(NoteClipInfo clipInfo)
        {
            base.Setup(clipInfo);

            _Holding = false;
            _StartHoldTimeOffset = 0;
            transform.rotation = NoteClipInfo.SongTrack.transform.rotation;
            //Line.transform.rotation = NoteClipInfo.SongTrack.transform.rotation;
            Scale = new Vector3(0.65f, 0.65f, 0.65f);
            StartNote.transform.localScale = Scale;
            EndNote.transform.localScale = Scale;
        }

        #endregion

        #region Events

        public override void OnKeyPressed(KeyEventArgs e)
        {
            UpdateOnPress();
        }

        public override void OnKeyReleased(KeyEventArgs e)
        {
            UpdateOnRelease();
        }

        public override void OnTouchPressed(TouchEventArgs e)
        {
            UpdateOnPress();
        }

        public override void OnTouchReleased(TouchEventArgs e)
        {
            UpdateOnRelease();
        }

        #endregion

        #region Update

        /// <inheritdoc cref="Note.TimelineUpdate(double, double)"/>
        public override void TimelineUpdate(double clipStartTime, double clipEndTime)
        {
            base.TimelineUpdate(clipStartTime, clipEndTime);
            UpdateLinePositions();
        }

        /// <inheritdoc cref="Note.InternalUpdate(double, double)"/>
        protected override void InternalUpdate(double timeFromStart, double timeFromEnd)
        {
            if (Application.isPlaying && (State == ActiveState.PostActive || State == ActiveState.Disabled))
                return;

            var deltaTStart = (float)(timeFromStart - NoteClipInfo.SongDirector.HalfCrochet);
            var deltaTEnd = (float)(timeFromEnd + NoteClipInfo.SongDirector.HalfCrochet);

            if (_Holding)
            {
                EndNote.position = GetNotePosition(deltaTEnd);

                if (Vector3.Distance(EndNote.position, StartNote.position) < 0)
                    EndNote.position = StartNote.position;

                return;
            }

            StartNote.position = GetNotePosition(deltaTStart);

            if (!Application.isPlaying)
            {
                EndNote.position = GetNotePosition(deltaTEnd);
                return;
            }

            if (RemoveNoteIfMissed && timeFromStart > NoteClipInfo.SongDirector.Crochet)
            {
                DeactivateNote();
                gameObject.SetActive(false);
            }

            EndNote.position = GetNotePosition(deltaTEnd);
        }

        private void LateUpdate()
        {
            UpdateLinePositions();
        }

        /// <summary>
        /// Update the size and position of the <see cref="Line"/>.
        /// </summary>
        private void UpdateLinePositions()
        {
            Line.size = new Vector2(Line.size.x, Mathf.Abs(StartNote.transform.position.y - EndNote.transform.position.y));
            Line.transform.position = new Vector3(Line.transform.position.x, Line.size.y / 2f, Line.transform.position.z);

            Debug.Log($"Size: {Line.size} Pos: {Line.transform.localPosition}");
        }

        #endregion

        #region Notes

        /// <inheritdoc cref="Note.ActivateNote"/>
        protected override void ActivateNote()
        {
            base.ActivateNote();
        }

        /// <inheritdoc cref="Note.DeactivateNote"/>
        protected override void DeactivateNote()
        {
            base.DeactivateNote();

            if (!Application.isPlaying)
                return;

            if (!IsTriggered)
            {
                gameObject.SetActive(false);

                InvokeOnNoteTriggered(new NoteEventArgs
                {
                    Note = this,
                    DspTime = DspTime.AdaptiveTime,
                    DspTimeDifference = 0,
                    DspTimeDifferencePercentage = 100,
                    IsMiss = true
                });
            }
        }

        /// <summary>
        /// Calculates the current position of the note on the <see cref="Tracks.SongTrack"/>.
        /// </summary>
        /// <param name="deltaT">The delta time between updates.</param>
        /// <returns>The calculated position of the <see cref="Note"/>.</returns>
        private Vector3 GetNotePosition(float deltaT)
        {
            var direction = NoteClipInfo.SongTrack.GetNoteDirection(deltaT);
            var distance = deltaT * NoteClipInfo.SongDirector.RealNoteSpeed;
            var targetPosition = NoteClipInfo.SongTrack.EndPoint.position;

            return targetPosition + (direction * distance);
        }

        #endregion

        #region Utils

        private void UpdateOnPress()
        {
            if (_Holding)
                return;

            _Holding = true;

            StartNote.position = NoteClipInfo.SongTrack.EndPoint.position;

            var perfectTime = NoteClipInfo.SongDirector.HalfCrochet;
            var timeDifference = TimeFromActivate - perfectTime;

            _StartHoldTimeOffset = timeDifference;
        }

        private void UpdateOnRelease()
        {
            if (!_Holding)
                return;

            gameObject.SetActive(false);

            var perfectTime = NoteClipInfo.SongDirector.HalfCrochet;
            var timeDifference = TimeFromDeactivate + perfectTime;
            var averageTotalTimeDifference = (_StartHoldTimeOffset + timeDifference) / 2f;
            var timeDifferencePercentage = Mathf.Abs((float)(100f * averageTotalTimeDifference)) / perfectTime;

            InvokeOnNoteTriggered(new NoteEventArgs
            {
                Note = this,
                DspTime = DspTime.AdaptiveTime,
                DspTimeDifference = timeDifference,
                DspTimeDifferencePercentage = timeDifferencePercentage
            });

            NoteClipInfo.SongTrack.Remove(this);
        }

        #endregion
    }
}
