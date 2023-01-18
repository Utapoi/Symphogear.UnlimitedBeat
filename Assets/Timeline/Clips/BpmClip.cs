using Symphogear.Timeline.Behaviours;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Symphogear.Timeline.Clips
{
    public class BpmClip : PlayableAsset
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<BpmBehaviour>.Create(graph);

            return playable;
        }
    }
}
