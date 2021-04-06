using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace MouseTrap.WPF
{
    public class RecordingView : INotifyPropertyChanged
    {
        public Guid Id { get; }
        public DateTime Date { get; }
        public PointCollection Points { get; }
        public TimeSpan Length { get; }

        private int played;
        public int Played
        {
            get => played;
            set
            {
                played = value;
                OnPropertyChanged();
            }
        }

        public RecordingView(Recording recording)
        {
            Id = recording.Id;
            Date = recording.Date;
            Length = recording.Points.Select(v => v.time).Aggregate((a, b) => a + b);
            Points = new PointCollection();
            Played = 0;

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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal class ObservableMouseRecorderService : MouseRecorderService
    {
        public ObservableMouseRecorderService(MouseControllerService mouseController, int recordingSleepMilliseconds) : base(mouseController, recordingSleepMilliseconds)
        {
        }

        public ObservableCollection<RecordingView> ObservableRecordings { get; } = new();

        public override Recording StartRecording(Func<bool> shouldStop)
        {
            var rec = base.StartRecording(shouldStop);
            App.Current.Dispatcher.Invoke(() =>
            {
                ObservableRecordings.Add(new RecordingView(rec));
            });

            return rec;
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

        public override void PlaybackRecording(Recording recording, Func<bool> shouldStop = null)
        {
            ObservableRecordings.FirstOrDefault(rv => rv.Id == recording.Id).Played++;
            base.PlaybackRecording(recording, shouldStop);
        }
    }
}
