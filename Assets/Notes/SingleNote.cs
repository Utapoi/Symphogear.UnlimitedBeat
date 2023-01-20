// Copyright (c) Utapoi Ltd <contact@utapoi.moe>

using Symphogear.Events;
using Symphogear.Timeline.Clips;
using UnityEngine;

namespace Symphogear.Notes
{
    /// <summary>
    /// A <see cref="Note" /> that only need to be taped once.
    /// </summary>
    public class SingleNote : Note
    {
        #region Setup Behaviour

        public override void Setup(NoteClipInfo clipInfo)
        {
            base.Setup(clipInfo);

            Scale = new Vector3(0.65f, 0.65f, 0.65f);
            transform.localScale = Scale;
            transform.rotation = NoteClipInfo.SongTrack.transform.rotation;
        }

        #endregion


        #region Events

        /// <inheritdoc cref="Note.OnKeyPressed(KeyEventArgs)"/>
        public override void OnKeyPressed(KeyEventArgs e)
        {
            IsTriggered = true;
            gameObject.SetActive(false);

            var perfectTime = NoteClipInfo.Duration / 2f;
            var timeDifference = (TimeFromActivate - perfectTime);
            var timeDifferencePercentage = (Mathf.Abs((float)(100f * timeDifference)) / perfectTime);

            InvokeOnNoteTriggered(new NoteEventArgs
            {
                Note = this,
                DspTime = DspTime.AdaptiveTime,
                DspTimeDifference = timeDifference,
                DspTimeDifferencePercentage = timeDifferencePercentage
            });

            NoteClipInfo.SongTrack.Remove(this);
        }

        /// <inheritdoc cref="Note.OnTouchPressed(TouchEventArgs)"/>
        public override void OnTouchPressed(TouchEventArgs e)
        {
            IsTriggered = true;
            gameObject.SetActive(false);
            NoteClipInfo.SongTrack.Remove(this);

            var perfectTime = NoteClipInfo.Duration / 2f;
            var timeDifference = (TimeFromActivate - perfectTime);
            var timeDifferencePercentage = (Mathf.Abs((float)(100f * timeDifference)) / perfectTime);

            InvokeOnNoteTriggered(new NoteEventArgs
            {
                Note = this,
                DspTime = DspTime.AdaptiveTime,
                DspTimeDifference = timeDifference,
                DspTimeDifferencePercentage = timeDifferencePercentage
            });
        }

        #endregion

        #region Update

        /// <inheritdoc cref="Note.InternalUpdate"/>
        protected override void InternalUpdate(double timeFromStart, double timeFromEnd)
        {
            base.InternalUpdate(timeFromStart, timeFromEnd);

            var perfectTime = NoteClipInfo.Duration / 2f;
            var deltaT = (float)(timeFromStart - perfectTime);
            var direction = NoteClipInfo.SongTrack.GetNoteDirection(deltaT);
            var distance = deltaT * NoteClipInfo.SongDirector.RealNoteSpeed;
            var targetPosition = NoteClipInfo.SongTrack.EndPoint.position;
            var newPosition = targetPosition + (direction * distance);

            transform.position = newPosition;
        }

        #endregion

        #region Notes

        /// <inheritdoc cref="Note.DeactivateNote"/>
        protected override void DeactivateNote()
        {
            base.DeactivateNote();

            if (!Application.isPlaying)
                return;

            if (!IsTriggered)
            {
                InvokeOnNoteTriggered(new NoteEventArgs
                {
                    Note = this,
                    DspTime = DspTime.AdaptiveTime,
                    DspTimeDifference = 0,
                    DspTimeDifferencePercentage = 1000,
                    IsMiss = true
                });
            }
        }

        #endregion
    }
}
