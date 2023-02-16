using DataProcessing.Services.Validate.Interfaces;
using System.Text.RegularExpressions;

namespace DataProcessing.Services.Validate
{
    public class RegexValidateOption : IValidationOption
    {
        readonly Regex regex;
        public RegexValidateOption(string pattern)
        {
            regex = new Regex(pattern);
        }

        public IValidationResult Validate(string input)
        {
            var str = input.Replace(" ", "");
            var match = regex.Match(str);

            return new RegexValidationResult()
            {
                IsValid = match.Success,
                Groups = match.Groups
            };
        }
    }
}
