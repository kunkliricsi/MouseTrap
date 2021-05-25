using Microsoft.Extensions.Configuration;
using System.IO;

namespace MouseTrap.WPF
{
    public class RecordOptions
    {
        public int SleepMilliseconds { get; set; }
    }

    public class MinMaxOptions
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }

    public class PlaybackOptions
    {
        public MinMaxOptions NextTime { get; set; }
    }

    public class Configuration
    {
        public RecordOptions Recording { get; set; }
        public PlaybackOptions Playback { get; set; }

        public static Configuration Current { get; }

        static Configuration()
        {
            var configuration = new Configuration();

            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build()
                .Bind(configuration);

            Current = configuration;
        }
    }
}
