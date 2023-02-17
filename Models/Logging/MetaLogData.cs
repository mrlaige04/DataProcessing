using DataProcessing.Models.Logging.Abstract;

namespace DataProcessing.Models.Logging
{
    public class MetaLogData : ILogData, IDisposable
    {
        private int parsed_files { get; set; }
        private int parsed_lines { get; set; }
        private int found_errors { get; set; }
        private IEnumerable<string> invalid_files { get; set; }

        public MetaLogData()
        {
            invalid_files = new List<string>();
        }

        public override string ToString()
        {
            return "Parsed files: " + parsed_files + 
                "\nParsed lines: " + parsed_lines + 
                "\nFound errors: " + found_errors + 
                "\nInvalid files: " + invalid_files;
        }

        public void Dispose()
        {
            parsed_files = 0;
            parsed_lines = 0;
            found_errors = 0;
            invalid_files = null!;
        }
    }
}
