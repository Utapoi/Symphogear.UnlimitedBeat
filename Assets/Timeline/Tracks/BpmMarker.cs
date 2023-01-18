using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace Symphogear.Timeline.Tracks
{
    [ExcludeFromPreset]
    [HideInMenu]
    public class BpmMarker : Marker
    {
        public int Id;

        public virtual void Copy(BpmMarker clonedFrom)
        {
            if (clonedFrom == null)
                return;

            Id = clonedFrom.Id;
        }
    }
}
