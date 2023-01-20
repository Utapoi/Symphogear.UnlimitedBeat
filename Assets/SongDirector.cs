// Copyright (c) Utapoi Ltd <contact@utapoi.moe>

using System;
using Sirenix.OdinInspector;
using Symphogear.Common;
using Symphogear.Events;
using Symphogear.Notes;
using Symphogear.Timeline;
using Symphogear.Timeline.Clips;
using Symphogear.Tracks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Symphogear
{
    /// <summary>
    /// A <see cref="SymphogearBehaviour"/> used to manage the current song.
    /// </summary>
    [Serializable]
    public class SongDirector : SymphogearBehaviour
    {
        /// <summary>
        /// A reference to the <see cref="PlayableDirector"/> that manage the timeline.
        /// </summary>
        [Title("References", "General", TitleAlignments.Split)]
        public PlayableDirector PlayableDirector;

        /// <summary>
        /// A reference to the <see cref="Common.DspTime"/> behaviour.
        /// </summary>
        public DspTime DspTime;

        /// <summary>
        /// An array containing all references to <see cref="SongTrack"/> objects.
        /// </summary>
        public SongTrack[] SongTracks;

        /// <summary>
        /// An array containing all references to <see cref="AudioSource"/> objects.
        /// </summary>
        public AudioSource[] AudioSources;

        /// <summary>
        /// Indicates whether or not we should be playing the song when the scene is loaded.
        /// </summary>
        [Title("Settings", "General", TitleAlignments.Split)]
        public bool PlayOnStart;

        /// <summary>
        /// Indicates whether or not we should scale the note speed based on the current song BPM.
        /// </summary>
        public bool ScaleNoteSpeedToBpm;

        /// <summary>
        /// The range we look ahead in order to spawn next notes.
        /// </summary>
        public Vector2 SpawnTimeRange = new(9, 3);

        /// <summary>
        /// The current <see cref="SongTimelineAsset"/>
        /// </summary>
        public SongTimelineAsset SongTimelineAsset
        {
            get
            {
                if (!Application.isPlaying)
                {
                    return PlayableDirector.playableAsset as SongTimelineAsset;
                }

                if (_SongTimelineAsset != null)
                    return _SongTimelineAsset;

                if (PlayableDirector.playableAsset == null)
                {
                    PlayableDirector.playableAsset = Resources.Load<PlayableAsset>("Songs/Hajimari no Babel.asset");
                }

                return PlayableDirector.playableAsset as SongTimelineAsset;
            }
            set => _SongTimelineAsset = value;
        }

        /// <summary>
        /// The approach speed of notes.
        /// </summary>
        [Title("Speed", "General", TitleAlignments.Split)]
        public float NoteSpeed;

        /// <summary>
        /// The real approach speed of notes.
        /// </summary>
        /// <remarks>
        /// This is only different from <see cref="NoteSpeed"/> when <see cref="ScaleNoteSpeedToBpm"/> is set to <c>true</c>.
        /// </remarks>
        public float RealNoteSpeed;

        /// <summary>
        /// The current BPM of the song.
        /// </summary>
        [Title("Timing", "General", TitleAlignments.Split)]
        public float Bpm;

        /// <summary>
        /// The start time of the song.
        /// </summary>
        public double DspSongStartTime;

        public float Crochet => 60f / Bpm;

        public float HalfCrochet => 30f / Bpm;

        public float QuarterCrochet => 15f / Bpm;

        /// <summary>
        /// The event triggered when a note is taped by the player.
        /// </summary>
        public event Action<NoteEventArgs> OnNoteTriggered;

        /// <summary>
        /// The number of audio tracks in the <see cref="SongTimelineAsset"/>.
        /// </summary>
        protected int AudioTracksCount = 0;

        /// <summary>
        /// Indicates whether or not the song is currently playing.
        /// </summary>
        protected bool IsPlaying;

        private SongTimelineAsset _SongTimelineAsset;

        /// <inheritdoc cref="SymphogearBehaviour.Initialize"/>
        public override bool Initialize()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 120;

            if (PlayableDirector == null)
            {
                PlayableDirector = GetComponent<PlayableDirector>();
            }

            if (DspTime == null)
            {
                DspTime = FindObjectOfType<DspTime>();
            }

            foreach (var track in SongTracks)
                track.OnNoteTriggered += InvokeOnNoteTriggered;

            if (PlayOnStart)
                PlaySong(PlayableDirector.playableAsset as SongTimelineAsset);

            return base.Initialize();
        }

        /// <inheritdoc cref="SymphogearBehaviour.CleanUp"/>
        public override bool CleanUp()
        {
            foreach (var track in SongTracks)
                track.OnNoteTriggered -= InvokeOnNoteTriggered;

            return base.CleanUp();
        }

        /// <summary>
        /// Calculate the BPM of a song.
        /// </summary>
        public void UpdateBpm()
        {
            Bpm = SongTimelineAsset.BeatsPerMinute;

            if (ScaleNoteSpeedToBpm)
            {
                RealNoteSpeed = NoteSpeed * Bpm;
            }
            else if (RealNoteSpeed < NoteSpeed)
            {
                RealNoteSpeed = NoteSpeed;
            }
        }

        public void PlaySong(SongTimelineAsset songTimeLine)
        {
            SongTimelineAsset = songTimeLine;
            PlayableDirector.playableAsset = SongTimelineAsset;

            UpdateBpm();
            SetupTrackBindings();

            PlayableDirector.time = 0.0;
            PlayableDirector.Play();
            DspSongStartTime = DspTime.AdaptiveTime;
            IsPlaying = true;
        }

        /// <summary>
        /// Instantiate a new <see cref="Note"/> on the corresponding <see cref="SongTrack"/>.
        /// </summary>
        /// <param name="settings">The <see cref="NoteSettings"/>.</param>
        /// <param name="clip">The <see cref="NoteClip"/>.</param>
        /// <returns>The <see cref="Note"/> instance with everything configured.</returns>
        public Note SpawnNote(NoteSettings settings, NoteClip clip)
        {
            var noteGameObject = GameObject.Instantiate<GameObject>(settings.Prefab); // , clip.NoteClipInfo.SongTrack.transform
            var note = noteGameObject.GetComponent<Note>();

            note.Setup(clip.NoteClipInfo);
            noteGameObject.SetActive(true);

            return note;
        }

        /// <summary>
        /// Destroy the <see cref="Note"/> passed as parameter.
        /// </summary>
        /// <param name="note">The <see cref="Note"/> to destroy.</param>
        public void DestroyNote(Note note)
        {
            if (note == null)
                return;

            if (Application.isPlaying)
            {
                GameObject.Destroy(note.gameObject);
            }
            else
            {
                GameObject.DestroyImmediate(note.gameObject);
            }
        }

        protected virtual void SetupTrackBindings()
        {
            var outputTracks = SongTimelineAsset.GetOutputTracks();

            foreach (var track in outputTracks)
            {
                if (track is AudioTrack audioTrack)
                    SetUpAudioTrackBinding(audioTrack);
            }
        }

        protected void SetUpAudioTrackBinding(AudioTrack audioTrack)
        {
            PlayableDirector.SetGenericBinding(audioTrack, AudioSources[AudioTracksCount]);
            AudioTracksCount++;
        }

        private void InvokeOnNoteTriggered(NoteEventArgs e)
        {
            OnNoteTriggered?.Invoke(e);
        }
    }
}
