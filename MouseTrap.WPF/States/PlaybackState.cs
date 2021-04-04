using MouseTrap.WPF.States.Base;
using System.Threading;

namespace MouseTrap.WPF.States
{
    public class PlaybackState : RecordingStateBase
    {
        public PlaybackState(RecordingStateBase previousState) : base(previousState)
        {
        }

        public override void OnEnter()
        {
            OnEscape();
        }

        public override void OnEscape()
        {
            Context.EscapeCancellation.Cancel();
            Context.EscapeCancellation = new CancellationTokenSource();

            SetCurrentState(new NopState(this));
        }

        public override void OnPlayback()
        {
            OnEscape();
        }

        public override void OnRecording()
        {
            // nop
        }

        public override void OnStateChange(MainWindow window)
        {
            window.buttonPlayback.IsEnabled = false;
            window.buttonRecord.IsEnabled = false;

            window.progressRecord.Visibility = System.Windows.Visibility.Collapsed;
            window.progressPlayback.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
