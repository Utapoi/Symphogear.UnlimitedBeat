using Symphogear.Common;
using Symphogear.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Symphogear.Inputs
{
    public struct TouchInfo
    {
        public int FingerId { get; set; }

        public int SongTrackId { get; set; }

        public Vector3 Position { get; set; }
    }

    public class TouchManager : SymphogearBehaviour
    {
        #region Properties

        public event Action<TouchEventArgs> OnTouchPressed;

        public event Action <TouchEventArgs> OnTouchReleased;

        private Camera Camera;

        private InputManager InputManager;

        /// <summary>
        /// A key-value pair dictionary where the TouchId from <see cref="TouchEventArgs"/> is used as a key.<br/>
        /// <see cref="TouchEventArgs"/> data is stored as the value.
        /// </summary>
        private readonly Dictionary<int, TouchInfo> m_TouchInfos = new();

        #endregion

        #region Behaviour Setup

        public override bool Initialize()
        {
            if (Camera == null)
            {
                Camera = Camera.main;
            }

            return base.Initialize();
        }

        private void OnEnable()
        {
            if (InputManager == null)
            {
                InputManager = InputManager.Instance;
            }

            InputManager.OnTouchPressed += InputManager_OnTouchPressed;
            InputManager.OnTouchReleased += InputManager_OnTouchReleased;
        }

        private void OnDisable()
        {
            InputManager.OnTouchPressed -= InputManager_OnTouchPressed;
            InputManager.OnTouchReleased -= InputManager_OnTouchReleased;
        }

        #endregion

        #region Updates

        private void Update()
        {
            //UpdateTouch();
        }

        private void UpdateTouch()
        {
            for (int i = 0; i < Input.touches.Length; i++)
            {
                var touch = Input.touches[i];
                var touchPosition = touch.position;

                if (m_TouchInfos.TryGetValue(touch.fingerId, out var info))
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Ended:
                            {
                                InvokeOnTouchReleased(new TouchEventArgs
                                {
                                    TouchId = info.FingerId,
                                    Position = info.Position
                                });
                            } break;
                        default:
                            break;
                    }

                    m_TouchInfos.Remove(info.FingerId);
                }

                if (touch.fingerId == info.FingerId)
                {
                    return;
                }

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        {
                            m_TouchInfos.Add(touch.fingerId, new TouchInfo
                            {
                                FingerId = touch.fingerId,
                                Position = touchPosition
                            });

                            InvokeOnTouchPressed(new TouchEventArgs
                            {
                                TouchId = touch.fingerId,
                                Position = touchPosition
                            });
                        } break;
                    case TouchPhase.Ended:
                        {
                            InvokeOnTouchReleased(new TouchEventArgs
                            {
                                TouchId = touch.fingerId,
                                Position = touchPosition
                            });

                            m_TouchInfos.Remove(touch.fingerId);
                        } break;
                    // We don't handle Swipe here.
                    default:
                        { } break;
                }
            }
        }

        #endregion

        #region Events

        private void InputManager_OnTouchPressed(TouchEventArgs e)
        {
            if (m_TouchInfos.TryGetValue(e.TouchId, out var _))
                return;

            var result = m_TouchInfos.TryAdd(e.TouchId, new TouchInfo
            {
                Position = e.Position,
            });

            if (!result)
            {
                Debug.LogError($"Failed to add TouchEvent({e.TouchId}) to Cache.");
                return;
            }

            InvokeOnTouchPressed(new TouchEventArgs
            {
                TouchId = e.TouchId,
                Position = e.Position,
            });
        }

        private void InputManager_OnTouchReleased(TouchEventArgs e)
        {
            if (!m_TouchInfos.TryGetValue(e.TouchId, out var _))
                return;

            InvokeOnTouchReleased(new TouchEventArgs
            {
                TouchId = e.TouchId,
                Position = e.Position
            });

            m_TouchInfos.Remove(e.TouchId);
        }

        #endregion

        #region Invoke Actions

        private void InvokeOnTouchPressed(TouchEventArgs e)
        {
            OnTouchPressed?.Invoke(e);
        }

        private void InvokeOnTouchReleased(TouchEventArgs e)
        {
            OnTouchReleased?.Invoke(e);
        }

        #endregion
    }
}
