using DataProcessing.Services.Validate.Interfaces;

namespace DataProcessing.Services.Validate
{
    public class Validator : IValidator
    {
        readonly IValidationOption option;
        public Validator(IValidationOption _option)
        {
            option = _option;
        }

        public IValidationResult Validate(string input)
        {
            return option.Validate(input);
        }
    }
}
