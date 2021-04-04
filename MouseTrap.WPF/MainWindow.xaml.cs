using MouseTrap.WPF.States;
using MouseTrap.WPF.States.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MouseTrap.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GlobalKeyboardHook _hook;
        private readonly MouseControllerService _mouseController;
        private readonly ObservableMouseRecorderService _mouseRecorder;

        private IRecordingState currentState;
        public IRecordingState CurrentState
        {
            get => currentState;
            set
            {
                label.Content = value.ToString().Split(".").Last();
                currentState = value;
            }
        }

        public ObservableCollection<Recording> Recordings { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _hook = new GlobalKeyboardHook();
            _hook.KeyboardPressed += OnKeyboardPressed;

            _mouseController = new MouseControllerService();
            _mouseRecorder = new ObservableMouseRecorderService(_mouseController);

            RecordingStateBase.SetStateSetter(s => CurrentState = s);
            RecordingStateBase.SetWindow(this);
            CurrentState = new NopState(new StateContext(_mouseRecorder));
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
            //if (e.Key == Key.Delete && listbox.SelectedIndex > 0)
            //{
            //    var items = listbox.SelectedItems.Cast<Recording>();
            //    foreach (var item in items)
            //    {
            //        _mouseRecorder.DeleteRecording(item);
            //    }
            //}
        }
    }
}
