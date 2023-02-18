using DataProcessing.Models.Logging.Abstract;
using System.Text.Json;

namespace DataProcessing.Models.Logging
{
    public class MetaLogData : ILogData, IDisposable
    {
        public int parsed_files { get; set; }
        public int parsed_lines { get; set; }
        public int found_errors { get; set; }
        public IList<string> invalid_files { get; set; }

        public MetaLogData()
        {
            invalid_files = new List<string>();
        }

        public override string ToString()
        {
            return "Parsed files: " + parsed_files +
                "\nParsed lines: " + parsed_lines +
                "\nFound errors: " + found_errors +
                "\nInvalid files: " + JsonSerializer.Serialize(invalid_files);
        }

        public void Dispose()
        {
            parsed_files = 0;
            parsed_lines = 0;
            found_errors = 0;
            invalid_files.Clear();
        }
    }
}
