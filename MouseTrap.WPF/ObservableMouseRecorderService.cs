using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace MouseTrap.WPF
{
    public class RecordingView
    {
        public Guid Id { get; }
        public DateTime Date { get; }
        public PointCollection Points { get; }
        public TimeSpan Length { get; }

        public RecordingView(Recording recording)
        {
            Id = recording.Id;
            Date = recording.Date;
            Length = recording.Points.Select(v => v.time).Aggregate((a, b) => a + b);
            Points = new PointCollection();

            var points = recording.Points.Select(v => new System.Windows.Point(v.point.X, v.point.Y)).ToList();
            var minX = points.Min(p => p.X);
            var minY = points.Min(p => p.Y);
            double maxX = points.Max(p => p.X) - minX;
            double maxY = points.Max(p => p.Y) - minY;

            for (int i = 0; i < points.Count(); i++)
            {
                points[i] -= new System.Windows.Vector(minX, minY);
                var x = (points[i].X / maxX * 100);
                var y = (points[i].Y / maxY * 100);
                Points.Add(new System.Windows.Point(x, y));
            }
            
        }

    }

    internal class ObservableMouseRecorderService : MouseRecorderService
    {
        public ObservableMouseRecorderService(MouseControllerService mouseController, int recordingSleepMilliseconds) : base(mouseController, recordingSleepMilliseconds)
        {
        }

        public ObservableCollection<RecordingView> ObservableRecordings { get; } = new();

        public override void StartRecording(Func<bool> shouldStop)
        {
            base.StartRecording(shouldStop);
            App.Current.Dispatcher.Invoke(() =>
            {
                ObservableRecordings.Add(new RecordingView(Recordings.Last()));
            });
        }

        public override void DeleteRecording(Guid id)
        {
            base.DeleteRecording(id);
            var rec = ObservableRecordings.FirstOrDefault(r => r.Id == id);
            if (rec is not null)
            {
                App.Current.Dispatcher.Invoke(() => ObservableRecordings.Remove(rec));
            }
        }
    }
}
