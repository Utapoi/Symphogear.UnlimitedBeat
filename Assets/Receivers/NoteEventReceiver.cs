using Sirenix.OdinInspector;
using Symphogear.Common;
using Symphogear.Events;
using Symphogear.Notes;

namespace Symphogear.Receivers
{
    public class NoteEventReceiver : SymphogearBehaviour
    {
        [Title("Events", "General", TitleAlignments.Split)]
        public NoteEvent OnNoteTriggered;

        public NoteEvent OnNoteMissed;

        public NoteEvent OnNoteActivated;

        public NoteEvent OnNoteDeactivated;

        private Note Note;

        public override bool Prepare()
        {
            InitializeComponent(ref Note);

            if (Note == null)
                return false;

            Note.OnNoteTriggered += HandleOnNoteTriggered;
            Note.OnActivate += HandleOnNoteActivated;
            Note.OnDeactivate += HandleOnNoteDeactivated;

            return base.Prepare();
        }

        private void HandleOnNoteTriggered(NoteEventArgs e)
        {
            if (e.IsMiss)
            {
                OnNoteMissed.Invoke(e);
            }
            else
            {
                OnNoteTriggered.Invoke(e);
            }
        }

        private void HandleOnNoteActivated(NoteEventArgs e)
        {
            OnNoteActivated.Invoke(e);
        }

        private void HandleOnNoteDeactivated(NoteEventArgs e)
        {
            OnNoteDeactivated.Invoke(e);
        }
    }
}
