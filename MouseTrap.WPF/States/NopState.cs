using MouseTrap.WPF.States.Base;

namespace MouseTrap.WPF.States
{
    public class NopState : RecordingStateBase
    {

        public NopState(RecordingStateBase previousState) : base(previousState)
        {
        }

        public NopState(StateContext context) : base(context)
        {
        }

        public override void OnEnter()
        {
            // nop
        }

        public override void OnEscape()
        {
            // nop
        }

        public override void OnPlayback()
        {
            if (Context.Recorder.Recordings.Count > 0)
            {
                SetCurrentState(new PlaybackTriggeredState(this));
            }
        }

        public override void OnRecording()
        {
            SetCurrentState(new RecordingTriggeredState(this));
        }

        public override void OnStateChange(MainWindow window)
        {
            window.buttonRecord.IsEnabled = true;
            window.buttonPlayback.IsEnabled = true;

            window.progressRecord.Visibility = System.Windows.Visibility.Collapsed;
            window.progressPlayback.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
