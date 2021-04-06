using MouseTrap.WPF.States.Base;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MouseTrap.WPF.States
{
    public class RecordingTriggeredState : RecordingStateBase
    {
        public RecordingTriggeredState(RecordingStateBase previousState) : base(previousState)
        {
        }

        public override void OnEnter()
        {

            Task.Run(() =>
            {
                var enterToken = Context.EnterCancellation.Token;
                var escapeToken = Context.EscapeCancellation.Token;

                var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(enterToken, escapeToken).Token;
                Context.Recorder.StartRecording(linkedToken);

                if (escapeToken.IsCancellationRequested)
                {
                    var lastRecording = Context.Recorder.Recordings.LastOrDefault();
                    if (lastRecording is not null) Context.Recorder.DeleteRecording(lastRecording);
                }
            });

            SetCurrentState(new RecordingState(this));
        }

        public override void OnEscape()
        {
            SetCurrentState(new NopState(this));
        }

        public override void OnPlayback()
        {
            // nop
        }

        public override void OnRecording()
        {
            OnEscape();
        }

        public override void OnStateChange(MainWindow window)
        {
            window.buttonPlayback.IsEnabled = false;
            window.buttonRecord.IsEnabled = true;

            window.progressRecord.Visibility = System.Windows.Visibility.Collapsed;
            window.progressPlayback.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
