using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MouseTrap.WPF
{
    internal class ObservableMouseRecorderService : MouseRecorderService
    {
        public ObservableMouseRecorderService(MouseControllerService mouseController) : base(mouseController)
        {
        }

        public ObservableCollection<Recording> ObservableRecordings { get; } = new();

        public override void StartRecording(Func<bool> shouldStop)
        {
            base.StartRecording(shouldStop);
            ObservableRecordings.Add(Recordings.Last());
        }

        public override void DeleteRecording(Guid id)
        {
            base.DeleteRecording(id);
            var rec = ObservableRecordings.FirstOrDefault(r => r.Id == id);
            if (rec is not null) ObservableRecordings.Remove(rec);
        }
    }
}
