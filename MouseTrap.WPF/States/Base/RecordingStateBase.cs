using System;

namespace MouseTrap.WPF.States.Base
{
    public abstract class RecordingStateBase : IRecordingState
    {
        protected StateContext Context { get; set; }

        protected RecordingStateBase(RecordingStateBase previousState)
            : this(previousState.Context) { }

        protected RecordingStateBase(StateContext context)
        {
            Context = context;
        }

        public abstract void OnRecording();
        public abstract void OnPlayback();
        public abstract void OnEnter();
        public abstract void OnEscape();

        private static Action<IRecordingState> setCurrentState;

        public static void SetStateSetter(Action<IRecordingState> setter)
        {
            setCurrentState = setter;
        }

        protected static void SetCurrentState(IRecordingState value)
        {
            setCurrentState(value);
            value.OnStateChange(window);
        }

        private static MainWindow window;

        public static void SetWindow(MainWindow mainWindow)
        {
            window = mainWindow;
        }

        public abstract void OnStateChange(MainWindow window);
    }
}
