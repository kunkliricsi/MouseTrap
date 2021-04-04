using System.Threading;

namespace MouseTrap.WPF.States.Base
{
    public class StateContext
    {
        public MouseRecorderService Recorder { get; }

        public CancellationTokenSource EnterCancellation { get; set; }
        public CancellationTokenSource EscapeCancellation { get; set; }

        public StateContext(MouseRecorderService mouseRecorder)
        {
            Recorder = mouseRecorder;
            EnterCancellation = new CancellationTokenSource();
            EscapeCancellation = new CancellationTokenSource();
        }
    }
}
