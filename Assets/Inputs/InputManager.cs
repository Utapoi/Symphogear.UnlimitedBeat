using Sirenix.OdinInspector;
using Symphogear.Common;
using Symphogear.Events;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Symphogear.Inputs
{
    [DefaultExecutionOrder(-1)]
    public class InputManager : Singleton<InputManager>
    {
        #region Properties

        [TitleGroup("References", "General", TitleAlignments.Split)]
        public Camera Camera;

        [TitleGroup("Inputs", "General", TitleAlignments.Split)]
        public InputActionAsset Controls;

        [TitleGroup("Events", "General", TitleAlignments.Split)]
        public event Action<KeyEventArgs> OnKeyPressed;

        public event Action<KeyEventArgs> OnKeyReleased;

        public event Action<TouchEventArgs> OnTouchPressed;

        public event Action<TouchEventArgs> OnTouchReleased;

        public event Action<SwipeEventArgs> OnSwipeStart;

        public event Action<SwipeEventArgs> OnSwipeEnd;

        public event Action<SwipeEventArgs> OnSwipe;

        #endregion

        #region Behaviour Setup

        private void OnEnable()
        {
            Controls.Enable();

            Controls["Key"].performed += InputManager_OnKeyPerformed;
            Controls["Key"].canceled += InputManager_OnKeyCanceled;

            Controls["Flick"].started += InputManager_OnSwipePerformed;
            Controls["Flick"].performed += InputManager_OnSwipePerformed;
            Controls["Flick"].canceled += InputManager_OnSwipeCanceled;

            Controls["Touch"].started += InputManager_TouchPerformed;
            Controls["Touch"].performed += InputManager_TouchPerformed;
            Controls["Touch"].canceled += InputManager_TouchCanceled;
        }

        private void OnDisable()
        {
            Controls["Key"].performed -= InputManager_OnKeyPerformed;
            Controls["Key"].canceled -= InputManager_OnKeyCanceled;

            Controls["Flick"].started -= InputManager_OnSwipePerformed;
            Controls["Flick"].performed -= InputManager_OnSwipePerformed;
            Controls["Flick"].canceled -= InputManager_OnSwipeCanceled;

            Controls["Touch"].started -= InputManager_TouchPerformed;
            Controls["Touch"].performed -= InputManager_TouchPerformed;
            Controls["Touch"].canceled -= InputManager_TouchCanceled;

            Controls.Disable();
        }

        #endregion

        #region Keyboard

        private void InputManager_OnKeyPerformed(InputAction.CallbackContext ctx)
        {
            if (ctx.control is not KeyControl kc)
                return;

            if (!kc.isPressed)
            {
                InvokeOnKeyReleased(new KeyEventArgs
                {
                    Key = kc.keyCode
                });
            }
            else
            {
                InvokeOnKeyPressed(new KeyEventArgs
                {
                    Key = kc.keyCode
                });
            }

        }

        private void InputManager_OnKeyCanceled(InputAction.CallbackContext ctx)
        {
            if (ctx.control is not KeyControl kc)
                return;

            InvokeOnKeyReleased(new KeyEventArgs
            {
                Key = kc.keyCode
            });
        }

        #endregion

        #region Touch

        private void InputManager_TouchPerformed(InputAction.CallbackContext ctx)
        {
            // Note(Mikyan): We don't convert the position from Screen to World position here.
            if (ctx.control is not TouchControl tc)
                return;

            switch (tc.phase.ReadValue())
            {
                case TouchPhase.Ended:
                    {
                        InvokeOnTouchReleased(new TouchEventArgs
                        {
                            TouchId = tc.touchId.ReadValue(),
                            Position = tc.position.ReadValue(),
                        });
                    } break;
                case TouchPhase.Began:
                    {
                        InvokeOnTouchPressed(new TouchEventArgs
                        {
                            TouchId = tc.touchId.ReadValue(),
                            Position = tc.position.ReadValue(),
                        });
                    } break;
                default:
                    break;
            }
        }

        private void InputManager_TouchCanceled(InputAction.CallbackContext ctx)
        {
            // Note(Mikyan): We don't convert the position from Screen to World position here.
            if (ctx.control is not TouchControl tc)
                return;

            InvokeOnTouchReleased(new TouchEventArgs
            {
                TouchId = tc.touchId.ReadValue(),
                Position = tc.position.ReadValue(),
            });
        }

        #endregion

        #region Swipe

        private void InputManager_OnSwipePerformed(InputAction.CallbackContext ctx)
        {
            // Note(Mikyan): We don't convert the position from Screen to World position here.
            if (ctx.control is not TouchControl tc)
                return;

            switch (tc.phase.ReadValue())
            {
                case TouchPhase.Ended:
                case TouchPhase.Moved: // We want the swipe to be as reactive as possible. Even a slight movement should trigger it.
                    {
                        InvokeOnSwipeEnded(new SwipeEventArgs
                        {
                            TouchId = tc.touchId.ReadValue(),
                            StartPosition = tc.startPosition.ReadValue(),
                            EndPosition = tc.position.ReadValue(),
                        });
                    }
                    break;
                case TouchPhase.Began:
                    {
                        InvokeOnSwipeStarted(new SwipeEventArgs
                        {
                            TouchId = tc.touchId.ReadValue(),
                            StartPosition = tc.startPosition.ReadValue(),
                        });
                    }
                    break;
                default:
                    break;
            }
        }

        private void InputManager_OnSwipeCanceled(InputAction.CallbackContext ctx)
        {
            // Note(Mikyan): We don't convert the position from Screen to World position here.
            if (ctx.control is not TouchControl tc)
                return;

            InvokeOnSwipeEnded(new SwipeEventArgs
            {
                TouchId = tc.touchId.ReadValue(),
                EndPosition = tc.position.ReadValue(),
                Time = ctx.time
            });
        }

        #endregion

        #region Invoke Actions

        private void InvokeOnKeyPressed(KeyEventArgs e)
        {
            OnKeyPressed?.Invoke(e);
        }

        private void InvokeOnKeyReleased(KeyEventArgs e)
        {
            OnKeyReleased?.Invoke(e);
        }

        private void InvokeOnTouchPressed(TouchEventArgs e)
        {
            OnTouchPressed?.Invoke(e);
        }

        private void InvokeOnTouchReleased(TouchEventArgs e)
        {
            OnTouchReleased?.Invoke(e);
        }

        private void InvokeOnSwipeStarted(SwipeEventArgs e)
        {
            OnSwipeStart?.Invoke(e);
        }

        private void InvokeOnSwipeEnded(SwipeEventArgs e)
        {
            OnSwipeEnd?.Invoke(e);
        }

        #endregion
    }
}
