using Symphogear.Common.Utils;
using Symphogear.Timeline.Clips;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Symphogear.Notes
{
    public enum SliderNoteType
    {
        None = 0,
        Start = 1,
        End = 2,
        Key = 3,
    }

    public class SliderNote : Note
    {
        public SliderNoteType NoteType;

        public List<Note> Notes;

        public LineRenderer LineRenderer;

        private List<Vector3> _Points = new();

        public override void Setup(NoteClipInfo clipInfo)
        {
            base.Setup(clipInfo);


            if (NoteType != SliderNoteType.Start)
                return;

            // Only the start node needs to calculate the curve of the slider.

            _Points = Bezier.Curve(Notes.Select(x => x.transform.position).ToList());
            LineRenderer.positionCount = _Points.Count;
        }

        protected override void InternalUpdate(double timeFromStart, double timeFromEnd)
        {
            throw new NotImplementedException();
        }
    }
}
