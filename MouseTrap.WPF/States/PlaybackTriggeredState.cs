using MouseTrap.WPF.States.Base;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;

namespace MouseTrap.WPF.States
{
    public class PlaybackTriggeredState : RecordingStateBase
    {
        public PlaybackTriggeredState(RecordingStateBase previousState) : base(previousState)
        {
        }

        public override void OnEnter()
        {
            Task.Run(() =>
            {
                var random = new Random();
                var input = new InputSimulator();
                var records = Context.Recorder.Recordings;
                var token = Context.EscapeCancellation.Token;
                do
                {
                    var record = records.ElementAt(random.Next(records.Count));
                    input.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.SPACE);
                    Thread.Sleep(random.Next(200, 500));
                    Context.Recorder.PlaybackRecording(record, token);
                    Thread.Sleep(random.Next(200, 500));
                    input.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.BACK);
                    Thread.Sleep(random.Next(200, 1500));
                } while (!token.IsCancellationRequested);
            });


            SetCurrentState(new PlaybackState(this));
        }

        public override void OnEscape()
        {
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
            window.buttonPlayback.IsEnabled = true;
            window.buttonRecord.IsEnabled = false;

            window.progressRecord.Visibility = System.Windows.Visibility.Collapsed;
            window.progressPlayback.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
