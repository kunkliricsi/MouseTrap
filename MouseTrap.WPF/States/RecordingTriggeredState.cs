using MouseTrap.WPF.States.Base;
using System;
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
            var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(Context.EnterCancellation.Token, Context.EscapeCancellation.Token).Token;

            Task.Run(() => Context.Recorder.StartRecording(linkedToken));
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
