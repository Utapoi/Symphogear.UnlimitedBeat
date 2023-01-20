using Sirenix.OdinInspector;
using Symphogear.Common;
using Symphogear.Events;
using Symphogear.Inputs;
using Symphogear.Notes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Symphogear.Tracks
{
    public class SongTrack : SymphogearBehaviour
    {
        #region Properties

        [TitleGroup("References", "General", TitleAlignments.Split)]
        public Transform StartPoint;

        [TitleGroup("References", "General", TitleAlignments.Split)]
        public Transform EndPoint;

        [TitleGroup("References", "General", TitleAlignments.Split)]
        public Collider2D Collider2d;

        public Collider Collider3d;

        public AudioSource EffectsAudioSource;

        public AudioClip HitSound;

        public Color ActiveColor;

        [TitleGroup("References", "General", TitleAlignments.Split)]
        public Camera Camera;

        [TitleGroup("References", "General", TitleAlignments.Split)]
        public SpriteRenderer Background;

        [TitleGroup("Inputs", "General", TitleAlignments.Split)]
        public InputManager InputManager;

        [TitleGroup("Inputs", "General", TitleAlignments.Split)]
        public SwipeManager SwipeManager;

        [TitleGroup("Inputs", "General", TitleAlignments.Split)]
        public TouchManager TouchManager;

        [TitleGroup("Inputs", "General", TitleAlignments.Split)]
        public Key SongTrackKey;

        public LayerMask ColliderLayerMask;

        public event Action<NoteEventArgs> OnNoteTriggered;

        public Note CurrentNote => Notes == null || Notes.Count == 0 ? null : Notes[0];

        protected List<Note> Notes;

        private Color _OriginalBackgroundColor;

        #endregion

        #region Behaviour Setup

        public override bool Prepare()
        {
            Clear();

            _OriginalBackgroundColor = Background.color;

            return base.Prepare();
        }

        public override bool Initialize()
        {
            InputManager = InputManager.Instance;

            InitializeComponent(ref Camera, Camera.main);
            InitializeComponent(ref SwipeManager);
            InitializeComponent(ref TouchManager);

            InputManager.OnKeyPressed += InputManager_OnKeyPressed;
            InputManager.OnKeyReleased += InputManager_OnKeyReleased;

            SwipeManager.OnSwipeTriggered += SwipeManager_OnSwipeTriggered;

            TouchManager.OnTouchPressed += TouchManager_OnTouchPressed;
            TouchManager.OnTouchReleased += TouchManager_OnTouchReleased;

            return base.Initialize();
        }

        public override bool CleanUp()
        {
            InputManager.OnKeyPressed -= InputManager_OnKeyPressed;
            InputManager.OnKeyReleased -= InputManager_OnKeyReleased;

            SwipeManager.OnSwipeTriggered -= SwipeManager_OnSwipeTriggered;

            TouchManager.OnTouchPressed -= TouchManager_OnTouchPressed;
            TouchManager.OnTouchReleased -= TouchManager_OnTouchReleased;

            return base.CleanUp();
        }

        #endregion

        #region Notes

        public void Add(Note note)
        {
            Notes ??= new List<Note>();

            note.OnNoteTriggered += OnNoteTriggered;

            Notes.Add(note);
        }

        public void Remove(Note note)
        {
            if (Notes == null || !Notes.Contains(note))
                return;

            note.OnNoteTriggered -= OnNoteTriggered;

            Notes.Remove(note);
        }

        protected virtual void Clear()
        {
            if (Notes == null)
            {
                Notes = new List<Note>();
            }
            else
            {
                Notes.Clear();
            }
        }

        public virtual Vector3 GetNoteDirection(float t)
        {
            return (EndPoint.position - StartPoint.position).normalized;
        }

        #endregion

        #region Input Events

        private void InputManager_OnKeyPressed(KeyEventArgs e)
        {
            if (e.Key != SongTrackKey)
                return;

            Background.color = ActiveColor;

            if (CurrentNote == null)
                EffectsAudioSource.PlayOneShot(HitSound);
            else
                CurrentNote.OnKeyPressed(e);
        }

        private void InputManager_OnKeyReleased(KeyEventArgs e)
        {
            if (e.Key != SongTrackKey)
                return;

            Background.color = _OriginalBackgroundColor;

            if (CurrentNote == null)
                EffectsAudioSource.PlayOneShot(HitSound);
            else
                CurrentNote.OnKeyReleased(e);
        }

        private void TouchManager_OnTouchPressed(TouchEventArgs e)
        {
            var ray = Camera.ScreenPointToRay(e.Position);
            Physics.Raycast(ray, out var hit, ColliderLayerMask);

            if (hit.collider == null)
                return;

            if (hit.collider != Collider2d && hit.collider != Collider3d)
                return;

            Background.color = ActiveColor;

            if (CurrentNote == null)
                EffectsAudioSource.PlayOneShot(HitSound);
            else
                CurrentNote.OnTouchPressed(e);
        }

        private void TouchManager_OnTouchReleased(TouchEventArgs e)
        {
            if (Background.color != _OriginalBackgroundColor)
                Background.color = _OriginalBackgroundColor;

            var ray = Camera.ScreenPointToRay(e.Position);
            Physics.Raycast(ray, out var hit, ColliderLayerMask);

            if (hit.collider == null)
                return;

            if (hit.collider != Collider2d && hit.collider != Collider3d)
                return;

            if (CurrentNote == null)
                EffectsAudioSource.PlayOneShot(HitSound);
            else
                CurrentNote.OnTouchReleased(e);
        }

        private void SwipeManager_OnSwipeTriggered(SwipeEventArgs e)
        {
            var ray = Camera.ScreenPointToRay(e.StartPosition);
            Physics.Raycast(ray, out var hit, ColliderLayerMask);

            if (hit.collider == null)
                return;

            if (hit.collider != Collider2d && hit.collider != Collider3d)
                return;

            if (CurrentNote != null)
            {
                CurrentNote.OnSwipeTriggered(e);
            }
        }

        #endregion
    }
}
