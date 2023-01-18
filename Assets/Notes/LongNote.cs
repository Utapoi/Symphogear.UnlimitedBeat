using Symphogear.Events;
using Symphogear.Timeline.Clips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Symphogear.Notes
{
    public class LongNote : Note
    {
        #region Properties

        public Transform StartNote;

        public Transform EndNote;

        public Color ActiveLineColor;

        public bool RemoveNoteIfMissed = true;

        public LineRenderer LineRenderer;

        private double _StartHoldTimeOffset;

        private bool _Holding;

        #endregion

        #region Behaviour Setup

        public override void Setup(NoteClipInfo clipInfo)
        {
            base.Setup(clipInfo);

            _Holding = false;
            LineRenderer.positionCount = 2;
            _StartHoldTimeOffset = 0;
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

        public override void TimelineUpdate(double clipStartTime, double clipEndTime)
        {
            base.TimelineUpdate(clipStartTime, clipEndTime);
            UpdateLinePositions();
        }

        protected override void InternalUpdate(double timeFromStart, double timeFromEnd)
        {
            if (Application.isPlaying && (State == ActiveState.PostActive || State == ActiveState.Disabled))
                return;

            var deltaTStart = (float)(timeFromStart - NoteClipInfo.SongDirector.HalfCrochet);
            var deltaTEnd = (float)(timeFromEnd + NoteClipInfo.SongDirector.HalfCrochet);

            if (_Holding && !AudioSource.isPlaying)
            {
                if (NoteClipInfo.Clip.NoteBehaviour.NoteSettings.HitSound != null && AudioSource.isActiveAndEnabled)
                {
                    AudioSource.clip = NoteClipInfo.Clip.NoteBehaviour.NoteSettings.HitSound;
                    AudioSource.loop = true;

                    AudioSource.Play();
                }
            }
            else if (AudioSource.isPlaying && !_Holding)
            {
                AudioSource.Stop();
            }

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

        private void UpdateLinePositions()
        {
            LineRenderer.SetPosition(0, StartNote.transform.localPosition);
            LineRenderer.SetPosition(1, EndNote.transform.localPosition);
        }

        #endregion

        #region Notes

        protected override void ActivateNote()
        {
            base.ActivateNote();

            LineRenderer.startColor = ActiveLineColor;
        }

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

            AudioSource.Stop();
        }

        private Vector3 GetNotePosition(float deltaT)
        {
            var direction = NoteClipInfo.SongTrack.GetNoteDirection(deltaT);
            var distance = deltaT * NoteClipInfo.SongDirector.NoteSpeed;
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
