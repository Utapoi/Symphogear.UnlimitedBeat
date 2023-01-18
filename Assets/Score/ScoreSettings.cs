using System;
using System.Collections.Generic;
using UnityEngine;

namespace Symphogear.Score
{
    [Serializable]
    public class NoteAccuracySettings
    {
        public string Name;

        public bool IsMiss;

        public bool IsBreakCombo;

        public float PercentageTheshold;

        public float Score;

        public Sprite Icon;

        public GameObject Prefab;

        public AudioClip HitSound;
    }

    [CreateAssetMenu(fileName = "ScoreSettings", menuName = "Symphogear/Score/Score Settings", order = 1)]
    public class ScoreSettings : ScriptableObject
    {
        public List<NoteAccuracySettings> NoteAccuracies = new();
    }
}
