using DataProcessing.Services.Validate.Interfaces;
using System.Text.RegularExpressions;

namespace DataProcessing.Services.Validate
{
    public class RegexValidationResult : IValidationResult
    {
        public bool IsValid { get; set; }
        public GroupCollection Groups { get; set; }
    }
}
