using Sirenix.OdinInspector;
using Symphogear.Common;
using Symphogear.Events;
using Symphogear.Notes;
using Symphogear.Timeline;
using Symphogear.Timeline.Clips;
using Symphogear.Tracks;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Symphogear
{
    [Serializable]
    public class SongDirector : SymphogearBehaviour
    {
        [Title("Settings", "General", TitleAlignments.Split)]
        public bool PlayOnStart;

        public bool ScaleNoteSpeedToBpm;

        public Vector2 SpawnTimeRange = new(9, 3);

        [Title("References", "General", TitleAlignments.Split)]
        public PlayableDirector PlayableDirector;

        public DspTime DspTime;

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
            set { _SongTimelineAsset = value; }
        }

        public SongTrack[] SongTracks;

        public AudioSource[] AudioSources;

        public event Action<NoteEventArgs> OnNoteTriggered;

        [Title("Speed", "General", TitleAlignments.Split)]
        public float NoteSpeed;

        public float RealNoteSpeed;

        [Title("Timing", "General", TitleAlignments.Split)]
        public float Bpm;

        public double DspSongStartTime;

        public float Crochet => 60f / Bpm;

        public float HalfCrochet => 30f / Bpm;

        public float QuarterCrochet => 15f / Bpm;

        protected int AudioTracksCount = 0;

        protected bool IsPlaying;

        private SongTimelineAsset _SongTimelineAsset;

        public override bool Initialize()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

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

        public override bool CleanUp()
        {
            foreach (var track in SongTracks)
                track.OnNoteTriggered -= InvokeOnNoteTriggered;

            return base.CleanUp();
        }

        public void UpdateBpm()
        {
            Bpm = SongTimelineAsset.BeatsPerMinute;

            if (ScaleNoteSpeedToBpm)
            {
                RealNoteSpeed = NoteSpeed * Bpm;
            }
            else
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

            PlayableDirector.RebuildGraph();
            PlayableDirector.time = 0.0;
            PlayableDirector.Play();
            DspSongStartTime = DspTime.AdaptiveTime;
            IsPlaying = true;
        }

        public Note SpawnNote(NoteSettings settings, NoteClip clip)
        {
            var noteGameObject = GameObject.Instantiate<GameObject>(settings.Prefab);
            var note = noteGameObject.GetComponent<Note>();

            note.Setup(clip.NoteClipInfo);
            noteGameObject.SetActive(true);

            return note;
        }

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
