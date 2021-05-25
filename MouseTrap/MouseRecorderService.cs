using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace MouseTrap
{
    public record Recording(Guid Id, DateTime Date, List<(Point point, TimeSpan time)> Points);

    public class MouseRecorderService
    {
        private readonly MouseControllerService _mouseController;
        private readonly int _sleepMilliseconds;

        public MouseRecorderService(MouseControllerService mouseController, int recordingSleepMilliseconds)
        {
            _mouseController = mouseController;
            _sleepMilliseconds = recordingSleepMilliseconds;
        }

        public ICollection<Recording> Recordings => _recordings.Values;
        private readonly Dictionary<Guid, Recording> _recordings = new ();

        public Recording StartRecording(CancellationToken cancellationToken)
            => StartRecording(() => cancellationToken.IsCancellationRequested);

        public virtual Recording StartRecording(Func<bool> shouldStop)
        {
            var previousPosition = new Point(0, 0);
            var points = new List<(Point, TimeSpan)>();
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            do
            {
                if (_mouseController.GetCursorPosition(out var currentPosition))
                {
                    if (currentPosition != previousPosition)
                    {
                        points.Add((currentPosition, stopwatch.Elapsed));
                        stopwatch.Restart();
                    }

                    previousPosition = currentPosition;
                }
                Thread.Sleep(_sleepMilliseconds);
            } while (!shouldStop());

            var newRecording = new Recording(Guid.NewGuid(), DateTime.Now, points);
            _recordings.Add(newRecording.Id, newRecording);

            return newRecording;
        }

        public void PlaybackRecording(Recording recording, CancellationToken cancellationToken)
            => PlaybackRecording(recording, () => cancellationToken.IsCancellationRequested);

        public virtual void PlaybackRecording(Recording recording, Func<bool> shouldStop = null)
        {
            var first = recording.Points[0].point;
            _mouseController.SetCursorPosition(first);
            Thread.Sleep(100);
            _mouseController.MouseEvent(MouseEvents.LeftDown);
            Thread.Sleep(100);
            foreach (var coord in recording.Points)
            {
                _mouseController.SetCursorPosition(coord.point);
                Thread.Sleep((int)coord.time.TotalMilliseconds);
                if (shouldStop?.Invoke() ?? false) break;
            }
            _mouseController.MouseEvent(MouseEvents.LeftUp);
        }

        public void DeleteRecording(Recording recording)
            => DeleteRecording(recording.Id);

        public virtual void DeleteRecording(Guid id)
        {
            _recordings.Remove(id);
        }
    }
}
