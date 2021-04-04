using System;
using System.Linq;

namespace MouseTrap
{
    static class Program
    {
        static void Main(string[] args)
        {
            var mc = new MouseControllerService();
            var mr = new MouseRecorderService(mc);

            Console.WriteLine("Press any key to start recording mouse movements.");
            Console.ReadKey();
            Console.WriteLine("Press any key to stop recording mouse movements.");
            mr.StartRecording(() => Console.KeyAvailable);

            Console.WriteLine("Press any key to play the recorded mouse positions.");
            Console.ReadKey();
            Console.WriteLine("Press any key to stop playing the recorded mouse positions.");
            var rec = mr.Recordings.First();
            mr.PlaybackRecording(rec, () => Console.KeyAvailable);
        }
    }
}