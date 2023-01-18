using Symphogear.Events;
using UnityEngine;

namespace Symphogear.Notes
{
    public class FlickNote : Note
    {
        #region Events

        public override void OnSwipeTriggered(SwipeEventArgs e)
        {
            IsTriggered = true;
            gameObject.SetActive(false);

            var perfectTime = NoteClipInfo.Duration / 2f;
            var timeDifference = TimeFromActivate - perfectTime;
            var timeDifferencePercentage = Mathf.Abs((float)(100f * timeDifference)) / perfectTime;

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

        #region Update

        protected override void InternalUpdate(double timeFromStart, double timeFromEnd)
        {
            var perfectTime = NoteClipInfo.Duration / 2f;
            var deltaT = (float)(timeFromStart - perfectTime);
            var direction = NoteClipInfo.SongTrack.GetNoteDirection(deltaT);
            var distance = deltaT * NoteClipInfo.SongDirector.NoteSpeed;
            var targetPosition = NoteClipInfo.SongTrack.EndPoint.position;
            var newPosition = targetPosition + (direction * distance);

            transform.position = newPosition;
        }

        #endregion

        #region Notes

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
                    DspTimeDifferencePercentage = 100,
                    IsMiss = true
                });
            }
        }

        #endregion
    }
}
