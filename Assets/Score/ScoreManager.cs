using Sirenix.OdinInspector;
using Symphogear.Common;
using Symphogear.Events;
using Symphogear.Notes;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Symphogear.Score
{
    public class ScoreManager : SymphogearBehaviour
    {
        #region Properties

        [TitleGroup("Settings", "General", TitleAlignments.Split)]
        public ScoreSettings Configuration;

        [TitleGroup("References", "General", TitleAlignments.Split)]
        public SongDirector SongDirector;

        [TitleGroup("References", "General", TitleAlignments.Split)]
        public Image NoteAccuracyImage;

        public Transform AccuracyPrefabSpawnPoint;

        public AudioSource EffectsAudioSource;


        #endregion

        #region Behaviour Setup

        public override bool Initialize()
        {
            InitializeComponent(ref SongDirector);

            SongDirector.OnNoteTriggered += SongDirector_OnNoteTriggered;

            return base.Initialize();
        }

        #endregion

        private NoteAccuracySettings GetAccuracy(NoteEventArgs e)
        {
            foreach (var nc in Configuration.NoteAccuracies)
            {
                if (nc.PercentageTheshold >= e.DspTimeDifferencePercentage)
                    return nc;
            }

            return Configuration.NoteAccuracies.Last();
        }

        #region Events

        private void SongDirector_OnNoteTriggered(NoteEventArgs e)
        {
            var accuracy = GetAccuracy(e);

            Debug.Log(accuracy.Name);

            if (accuracy.Prefab != null)
                Instantiate(accuracy.Prefab, AccuracyPrefabSpawnPoint.position, Quaternion.identity);

            if (e.Note is FlickNote _)
            {
                // Play Flick Sound.
            }
            else if (accuracy.HitSound != null)
            {
                EffectsAudioSource.PlayOneShot(accuracy.HitSound);
            }
        }

        #endregion
    }
}
