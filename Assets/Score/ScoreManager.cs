// Copyright (c) Utapoi Ltd <contact@utapoi.moe>

using System.Linq;
using Sirenix.OdinInspector;
using Symphogear.Common;
using Symphogear.Events;
using Symphogear.Notes;
using UnityEngine;

namespace Symphogear.Score
{
    public class ScoreManager : SymphogearBehaviour
    {
        #region Properties

        /// <summary>
        /// A reference to the <see cref="ScoreSettings"/> scriptable object used to calculate the score and accuracy for each song.
        /// </summary>
        [Title("Settings", "General", TitleAlignments.Split)]
        public ScoreSettings Configuration;

        /// <summary>
        /// A reference to the <see cref="Symphogear.SongDirector"/> behaviour.
        /// </summary>
        [Title("References", "General", TitleAlignments.Split)]
        public SongDirector SongDirector;

        /// <summary>
        /// A reference to the <see cref="Transform"/> position used to spawn accuracy effects.
        /// </summary>
        /// <remarks>Perfect, Great, Good, Bad and Miss.</remarks>
        public Transform AccuracyPrefabSpawnPoint;

        /// <summary>
        /// A reference to the <see cref="AudioSource" /> used to play SFX.
        /// </summary>
        public AudioSource EffectsAudioSource;

        #endregion

        #region Behaviour Setup

        /// <inheritdoc cref="SymphogearBehaviour.Initialize"/>
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
