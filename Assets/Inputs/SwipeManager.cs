using Sirenix.OdinInspector;
using Symphogear.Common;
using Symphogear.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Symphogear.Inputs
{
    public struct SwipeInfo
    {
        public Vector2 StartPosition { get; set; }

        public Vector2 EndPosition { get; set; }

        public double StartTime { get; set; }

        public double EndTime { get; set; }
    }

    public class SwipeManager : SymphogearBehaviour
    {
        #region Properties

        [TitleGroup("Settings", "General", TitleAlignments.Split)]
        public SwipeSettings Configuration;

        public event Action<SwipeEventArgs> OnSwipeTriggered;

        private InputManager InputManager;

        /// <summary>
        /// A key-value pair dictionary where the TouchId from <see cref="SwipeEventArgs"/> is used as a key.<br/>
        /// <see cref="SwipeEventArgs"/> data is stored as the value.
        /// </summary>
        private readonly Dictionary<int, SwipeInfo> m_SwipeInfos = new();

        #endregion

        #region Behaviour Setup

        private void OnEnable()
        {
            if (InputManager == null)
            {
                InputManager = InputManager.Instance;
            }

            InputManager.OnSwipeStart += SwipeManager_OnSwipeStart;
            InputManager.OnSwipeEnd += SwipeManager_OnSwipeEnd;
        }

        private void OnDisable()
        {
            InputManager.OnSwipeStart -= SwipeManager_OnSwipeStart;
            InputManager.OnSwipeEnd -= SwipeManager_OnSwipeEnd;
        }

        #endregion

        #region Events

        private void SwipeManager_OnSwipeStart(SwipeEventArgs e)
        {
            if (m_SwipeInfos.TryGetValue(e.TouchId, out var _))
                return;

            var result = m_SwipeInfos.TryAdd(e.TouchId, new SwipeInfo
            {
                StartPosition = e.StartPosition,
                StartTime = e.Time,
            });

            if (!result)
            {
                Debug.LogError($"Failed to add SwipeEvent({e.TouchId}) to Cache.");
            }
        }

        private void SwipeManager_OnSwipeEnd(SwipeEventArgs e)
        {
            if (!m_SwipeInfos.TryGetValue(e.TouchId, out var info))
                return;

            info.EndPosition = e.EndPosition;
            info.EndTime = e.Time;

            if (Vector3.Distance(info.StartPosition, info.EndPosition) >= Configuration.MinimumSwipeDistance
                && info.EndTime - info.StartTime <= Configuration.MaximumInputDuration)
            {
                InvokeOnSwipeTriggered(new SwipeEventArgs
                {
                    TouchId = e.TouchId,
                    StartPosition = info.StartPosition,
                    EndPosition = info.EndPosition,
                    Time = info.EndTime - info.StartTime
                });

                m_SwipeInfos.Remove(e.TouchId);
            }
        }

        #endregion

        #region Invoke Actions

        private void InvokeOnSwipeTriggered(SwipeEventArgs e)
        {
            OnSwipeTriggered?.Invoke(e);
        }

        #endregion
    }
}
