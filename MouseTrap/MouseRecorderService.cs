using System;
using System.Collections.Generic;
using System.Threading;

namespace MouseTrap
{
    public record Recording(Guid Id, DateTime Date, List<Point> Points);

    public class MouseRecorderService
    {
        private readonly MouseControllerService _mouseController;

        public MouseRecorderService(MouseControllerService mouseController)
        {
            _mouseController = mouseController;
        }

        public ICollection<Recording> Recordings => _recordings.Values;
        private readonly Dictionary<Guid, Recording> _recordings = new ();

        public void StartRecording(CancellationToken cancellationToken)
            => StartRecording(() => cancellationToken.IsCancellationRequested);

        public virtual void StartRecording(Func<bool> shouldStop)
        {
            var previousPosition = new Point(0, 0);
            var points = new List<Point>();

            do
            {
                if (_mouseController.GetCursorPosition(out var currentPosition))
                {
                    if (currentPosition != previousPosition)
                    {
                        points.Add(currentPosition);
                    }

                    previousPosition = currentPosition;
                }

            } while (!shouldStop());

            var newRecording = new Recording(Guid.NewGuid(), DateTime.Now, points);
            _recordings.Add(newRecording.Id, newRecording);
        }

        public void PlaybackRecording(Recording recording, CancellationToken cancellationToken)
            => PlaybackRecording(recording, () => cancellationToken.IsCancellationRequested);

        public void PlaybackRecording(Recording recording, Func<bool> shouldStop = null)
        {
            var first = recording.Points[0];
            _mouseController.SetCursorPosition(first);
            _mouseController.MouseEvent(MouseEvents.LeftDown);
            foreach (var coord in recording.Points)
            {
                _mouseController.SetCursorPosition(coord);
                Thread.Sleep(10);
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
