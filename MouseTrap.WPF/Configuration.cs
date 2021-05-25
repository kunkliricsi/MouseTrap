using Microsoft.Extensions.Configuration;
using System.IO;

namespace MouseTrap.WPF
{
    public record RecordOptions(int SleepMilliseconds);
    public record MinMaxOptions(int Max, int Min);
    public record PlaybackOptions(MinMaxOptions Next);

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
