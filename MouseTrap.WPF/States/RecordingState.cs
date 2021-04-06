using MouseTrap.WPF.States.Base;
using System.Linq;
using System.Threading;
using System.Windows.Media;

namespace MouseTrap.WPF.States
{
    public class RecordingState : RecordingStateBase
    {
        public RecordingState(RecordingStateBase previousState) : base(previousState)
        {
        }

        public override void OnEnter()
        {
            Context.EnterCancellation.Cancel();
            Context.EnterCancellation = new CancellationTokenSource();

            SetCurrentState(new RecordingTriggeredState(this));
        }

        public override void OnEscape()
        {
            Context.EscapeCancellation.Cancel();
            Context.EscapeCancellation = new CancellationTokenSource();

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
            window.buttonRecord.IsEnabled = false;

            window.progressRecord.Visibility = System.Windows.Visibility.Visible;
            window.progressPlayback.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
