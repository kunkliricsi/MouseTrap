using MouseTrap.WPF.States;
using MouseTrap.WPF.States.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MouseTrap.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly GlobalKeyboardHook _hook;
        private readonly MouseControllerService _mouseController;
        private readonly ObservableMouseRecorderService _mouseRecorder;

        private IRecordingState currentState;

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly List<string> states = new();

        public IRecordingState CurrentState
        {
            get => currentState;
            set
            {
                var state = value.ToString().Split(".").Last();
                var index = states.IndexOf(state);
                if (index != -1)
                {
                    if (index != states.Count - 1)
                    {
                        states.RemoveRange(index + 1, states.Count - (index+1));
                    }
                }
                else
                {
                    states.Add(state);
                }

                label.Content = string.Join(" -> ", states);
                currentState = value;
            }
        }

        private ObservableCollection<RecordingView> recordings;
        public ObservableCollection<RecordingView> Recordings
        {
            get => recordings;
            set
            {
                recordings = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            _hook = new GlobalKeyboardHook();
            _hook.KeyboardPressed += OnKeyboardPressed;

            var sleep = int.Parse((Application.Current as App).Configuration["Recording:SleepMilliseconds"]);

            _mouseController = new MouseControllerService();
            _mouseRecorder = new ObservableMouseRecorderService(_mouseController, sleep);
            Recordings = _mouseRecorder.ObservableRecordings;

            RecordingStateBase.SetStateSetter(s => CurrentState = s);
            RecordingStateBase.SetWindow(this);
            CurrentState = new NopState(new StateContext(_mouseRecorder));
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnKeyboardPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (e.KeyboardState != GlobalKeyboardHook.KeyboardState.KeyDown)
                return;

            if (e.KeyboardData.VirtualCode == GlobalKeyboardHook.VkEnter)
            {
                CurrentState.OnEnter();
                e.Handled = true;
            }

            else if (e.KeyboardData.VirtualCode == GlobalKeyboardHook.VkEscape)
            {
                CurrentState.OnEscape();
                e.Handled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CurrentState.OnRecording();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CurrentState.OnPlayback();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && listbox.SelectedIndex > -1)
            {
                var items = listbox.SelectedItems.Cast<RecordingView>().ToList();
                foreach (var item in items)
                {
                    _mouseRecorder.DeleteRecording(item.Id);
                }
            }
        }
    }
}
