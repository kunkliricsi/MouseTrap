using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MouseTrap
{
    static class Program
    {
        static bool stop = false;

        static void Main(string[] args)
        {
            var kh = new GlobalKeyboardHook();
            var mc = new MouseControllerService();
            var mr = new MouseRecorderService(mc);

            kh.KeyboardPressed += OnKeyboardPressed;

            Console.WriteLine("Press any key to start recording mouse movements.");
            Console.ReadKey();
            Console.WriteLine("Press any key to stop recording mouse movements.");
            mr.StartRecording(() => Console.KeyAvailable);

            Console.WriteLine("Press any key to play the recorded mouse positions.");
            Console.ReadKey();
            Console.WriteLine("Press any key to stop playing the recorded mouse positions.");
            var rec = mr.Recordings.First();
            mr.PlaybackRecording(rec, () => Console.KeyAvailable || stop);
        }

        private static void OnKeyboardPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (e.KeyboardData.VirtualCode != GlobalKeyboardHook.VkEscape)
                return;

            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                stop = true;
                e.Handled = true;
            }
        }
    }
}