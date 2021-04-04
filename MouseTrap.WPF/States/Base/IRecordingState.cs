namespace MouseTrap.WPF.States.Base
{
    public interface IRecordingState
    {
        void OnRecording();
        void OnPlayback();
        void OnEnter();
        void OnEscape();

        void OnStateChange(MainWindow window);
    }
}
